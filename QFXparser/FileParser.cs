using QFXparser.Parsing;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QFXparser
{
    public class FileParser
    {
        private string _fileText;
        private readonly CultureInfo _cultureInfo = CultureInfo.CurrentCulture;

        /// <summary>
        /// Initialize a FileParser with encoding set by the .qfx file or detected by the stream reader
        /// and with specific culture info.
        /// </summary>
        /// <param name="fileNamePath"></param>
        public FileParser(string fileNamePath, CultureInfo cultureInfo) : this(fileNamePath)
        {
            _cultureInfo = cultureInfo;        
        }
        /// <summary>
        /// Initialize a FileParser with encoding set by the .qfx file or detected by the stream reader
        /// with current culture info.
        /// </summary>
        /// <param name="fileNamePath"></param>
        public FileParser(string fileNamePath)
        {
            var encoding = GetEncodingFromHeaders(fileNamePath);
            if (encoding != null)
            {
                using (StreamReader sr = new StreamReader(fileNamePath, encoding))
                {
                    _fileText = sr.ReadToEnd();
                }
            }
            else 
            {
                using (StreamReader sr = new StreamReader(fileNamePath, true))
                {
                    _fileText = sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Initialize a FileParser with encoding set by the .qfx file or detected by the stream reader
        /// and with specific culture info.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="cultureInfo"></param>
        public FileParser(Stream fileStream, CultureInfo cultureInfo) : this(fileStream)
        {
            _cultureInfo = cultureInfo;
        }

        /// <summary>
        /// Initialize a FileParser with UTF-8 encoding and current culture info.
        /// </summary>
        /// <param name="fileStream"></param>
        public FileParser(Stream fileStream)
        {
            var encoding = GetEncodingFromHeaders(fileStream);
            if (encoding != null)
            {
                using (StreamReader sr = new StreamReader(fileStream, encoding))
                {
                    _fileText = sr.ReadToEnd();
                }
            }
            else
            {
                using (StreamReader sr = new StreamReader(fileStream, true))
                {
                    _fileText = sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Initialize a FileParser with invariant culture info.
        /// </summary>
        /// <remarks>Depricated</remarks>
        /// <param name="streamReader"></param>
        public FileParser(StreamReader streamReader):this(streamReader, CultureInfo.InvariantCulture)
        {
        }

        /// <summary>
        /// Initialize a FileParser
        /// </summary>
        /// <remarks>Depricated</remarks>
        /// <param name="streamReader"></param>
        /// <param name="cultureInfo"></param>
        public FileParser(StreamReader streamReader, CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
            _fileText = streamReader.ReadToEnd();
        }

        public Statement BuildStatement()
        {
            RawStatement rawStatement = BuildRaw();

            Statement statement = new Statement
            {
                AccountNum = rawStatement.AccountNum
            };

            foreach (var rawTrans in rawStatement.Transactions)
            {
                Transaction trans = new Transaction
                {
                    Amount = rawTrans.Amount,
                    Memo = rawTrans.Memo,
                    Name = rawTrans.Name,
                    PostedOn = rawTrans.PostedOn,
                    RefNumber = rawTrans.RefNumber,
                    TransactionId = rawTrans.TransactionId,
                    Type = rawTrans.Type
                };
                statement.Transactions.Add(trans);
            }

            statement.LedgerBalance = new LedgerBalance
            {
                Amount = rawStatement.LedgerBalance.Amount,
                AsOf = rawStatement.LedgerBalance.AsOf
            };

            return statement;
        }

        private RawStatement BuildRaw()
        {
            RawStatement _statement = null;
            MemberInfo currentMember = null;
            RawTransaction _currentTransaction = null;
            RawLedgerBalance _ledgerBalance = null;

            foreach (var token in Parser.Parse(_fileText))
            {
                if (token.IsElement)
                {
                    var result = GetPropertyInfo(token.Content);
                    if (result != null)
                    {
                        switch (result.Type)
                        {
                            case NodeType.StatementOpen:
                                _statement = new RawStatement();
                                break;
                            case NodeType.StatementClose:
                                return _statement;
                                break;
                            case NodeType.TransactionOpen:
                                _currentTransaction = new RawTransaction();
                                break;
                            case NodeType.TransactionClose:
                                _statement.Transactions.Add(_currentTransaction);
                                _currentTransaction = null;
                                break;
                            case NodeType.StatementProp:
                                if (_statement == null)
                                {
                                    _statement = new RawStatement();
                                }
                                currentMember = result.Member;
                                break;
                            case NodeType.TransactionProp:
                                currentMember = result.Member;
                                break;
                            case NodeType.LedgerBalanceOpen:
                                _ledgerBalance = new RawLedgerBalance();
                                break;
                            case NodeType.LedgerBalanceClose:
                                _statement.LedgerBalance.Amount = _ledgerBalance.Amount;
                                _statement.LedgerBalance.AsOf = _ledgerBalance.AsOf;
                                break;
                            case NodeType.LedgerBalanceProp:
                                if (_ledgerBalance == null)
                                {
                                    _ledgerBalance = new RawLedgerBalance();
                                }
                                currentMember = result.Member;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        currentMember = null;
                    }
                }
                else
                {
                    if (currentMember != null && currentMember is PropertyInfo)
                    {
                        var property = (PropertyInfo)currentMember;
                        switch (property.DeclaringType.Name)
                        {
                            case "RawStatement":
                                property.SetValue(_statement, ConvertQfxType(token.Content, property.PropertyType));
                                break;
                            case "RawTransaction":
                                property.SetValue(_currentTransaction, ConvertQfxType(token.Content, property.PropertyType));
                                break;
                            case "RawLedgerBalance":
                                property.SetValue(_ledgerBalance, ConvertQfxType(token.Content, property.PropertyType));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return _statement;
        }

        private object ConvertQfxType(string content, Type targetType)
        {
            object result;
            if (targetType == typeof(DateTime))
            {
                result = ParsingHelper.ParseDate(content);
            }
            else
            {
                try
                {
                    result = Convert.ChangeType(content, targetType, _cultureInfo);
                }
                catch (Exception)
                {
                    result = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                }
            }
            return result;
        }

        private PropertyResult GetPropertyInfo(string token)
        {
            var propertyResult = new PropertyResult();

            if (typeof(RawStatement).GetCustomAttribute<NodeNameAttribute>().CloseTag == token)
            {
                propertyResult.Member = typeof(RawStatement);
                propertyResult.Type = NodeType.StatementClose;
                return propertyResult;
            }

            if (typeof(RawTransaction).GetCustomAttribute<NodeNameAttribute>().CloseTag == token)
            {
                propertyResult.Member = typeof(RawTransaction);
                propertyResult.Type = NodeType.TransactionClose;
                return propertyResult;
            }

            if (typeof(RawStatement).GetCustomAttribute<NodeNameAttribute>().OpenTag == token)
            {
                propertyResult.Member = typeof(RawStatement);
                propertyResult.Type = NodeType.StatementOpen;
                return propertyResult;
            }

            if (typeof(RawTransaction).GetCustomAttribute<NodeNameAttribute>().OpenTag == token)
            {
                propertyResult.Member = typeof(RawTransaction);
                propertyResult.Type = NodeType.TransactionOpen;
                return propertyResult;
            }

            if (typeof(RawLedgerBalance).GetCustomAttribute<NodeNameAttribute>().OpenTag == token)
            {
                propertyResult.Member = typeof(RawLedgerBalance);
                propertyResult.Type = NodeType.LedgerBalanceOpen;
                return propertyResult;
            }

            if (typeof(RawLedgerBalance).GetCustomAttribute<NodeNameAttribute>().CloseTag == token)
            {
                propertyResult.Member = typeof(RawLedgerBalance);
                propertyResult.Type = NodeType.LedgerBalanceClose;
                return propertyResult;
            }

            var statementMember = typeof(RawStatement).GetProperties().FirstOrDefault(m => m.GetCustomAttribute<NodeNameAttribute>().OpenTag == token);

            if (statementMember != null)
            {
                propertyResult.Member = statementMember;
                propertyResult.Type = NodeType.StatementProp;
                return propertyResult;
            }

            var transactionMember = typeof(RawTransaction).GetProperties().Where(m => m.GetCustomAttribute<NodeNameAttribute>() != null)
                .FirstOrDefault(m => m.GetCustomAttribute<NodeNameAttribute>().OpenTag == token);

            if (transactionMember != null)
            {
                propertyResult.Member = transactionMember;
                propertyResult.Type = NodeType.TransactionProp;
                return propertyResult;
            }

            var balanceMember = typeof(RawLedgerBalance).GetProperties().Where(m => m.GetCustomAttribute<NodeNameAttribute>() != null)
                .FirstOrDefault(m => m.GetCustomAttribute<NodeNameAttribute>().OpenTag == token);

            if (balanceMember != null)
            {
                propertyResult.Member = balanceMember;
                propertyResult.Type = NodeType.LedgerBalanceProp;
                return propertyResult;
            }

            return null;
        }

        private Encoding GetEncodingFromHeaders(string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                return GetEncodingFromHeaders(stream);
            }
        }

        private Encoding GetEncodingFromHeaders(Stream fileStrem)
        {
            int? code = null;
            using (StreamReader sr = new StreamReader(fileStrem, true))
            {
                string[] separator = { ":" };
                string currentLine = sr.ReadLine();
                
                while (!currentLine.StartsWith("<") && !sr.EndOfStream)
                {
                    if (currentLine.StartsWith("CHARSET"))
                    {
                        var split = currentLine.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        int result;
                        if (int.TryParse(split[1], out result))
                        {
                            code = result;
                        }
                    }

                    currentLine = sr.ReadLine();
                }
            }

            int pageCode = code == null ? 1252 : code.GetValueOrDefault();
            Encoding encoding;
            try
            {
                encoding = CodePagesEncodingProvider.Instance.GetEncoding(pageCode);
            }
            finally
            {
            }
            return encoding;
        }
    }    
}
