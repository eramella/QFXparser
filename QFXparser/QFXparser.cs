using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QFXparser
{
    public class FileParser
    {
        private string _fileText;
        private RawLedgerBalance _ledgerBalance;

        public FileParser(string fileNamePath)
        {
            using (var sr = new StreamReader(fileNamePath, true))
            {
                _fileText = sr.ReadToEnd();
            }
        }

        public FileParser(Stream fileStream)
        {
            using (var sr = new StreamReader(fileStream, true))
            {
                _fileText = sr.ReadToEnd();
            }
        }

        public Statement BuildStatement()
        {
            var rawStatement = BuildRaw();

            var statement = new Statement
            {
                AccountNum = rawStatement.AccountNum
            };

            var rawLedgerBalance = rawStatement.LedgerBalance;
            var currBalance = rawLedgerBalance != null ? rawLedgerBalance.Amount : (decimal?)null;
            foreach (var rawTrans in rawStatement.Transactions)
            {
                var trans = new Transaction
                {
                    Amount = rawTrans.Amount,
                    Memo = rawTrans.Memo,
                    Name = rawTrans.Name,
                    PostedOn = rawTrans.PostedOn,
                    RefNumber = rawTrans.RefNumber,
                    TransactionId = rawTrans.TransactionId,
                    Type = rawTrans.Type,
                    Balance = currBalance
                };
                statement.Transactions.Add(trans);
                currBalance -= rawTrans.Amount;
            }
 
            if (rawLedgerBalance != null)
            {
                statement.LedgerBalance = new LedgerBalance
                {
                    Amount = rawLedgerBalance.Amount,
                    AsOf = rawLedgerBalance.AsOf
                };
            }

            return statement;
        }

        private RawStatement BuildRaw()
        {
            RawStatement _statement = null;
            MemberInfo currentMember = null;
            RawTransaction _currentTransaction = null;

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
                                _statement.LedgerBalance = _ledgerBalance;
                                break;
                            case NodeType.LedgerBalanceClose:
                                _statement.LedgerBalance.Amount = _ledgerBalance.Amount;
                                _statement.LedgerBalance.AsOf = _ledgerBalance.AsOf;
                                break;
                            case NodeType.LedgerBalanceProp:
                                if (_ledgerBalance == null)
                                {
                                    _ledgerBalance = new RawLedgerBalance();
                                    _statement.LedgerBalance = _ledgerBalance;
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
                    result = Convert.ChangeType(content, targetType);
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
    }    
}
