using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace QFXparser.Core
{
    public class StatementBuilder
    {
        private Statement _statement;
        private Transaction _currentTransaction;
        private string _fileText;

        public StatementBuilder(string fileNamePath)
        {
            using (StreamReader sr = new StreamReader(fileNamePath))
            {
                _fileText = sr.ReadToEnd();
            }

        }

        public StatementBuilder(Stream fileStream)
        {
            using (StreamReader sr = new StreamReader(fileStream))
            {
                _fileText = sr.ReadToEnd();
            }

        }

        public Statement Build()
        {

            MemberInfo currentMember = null;

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
                                _statement = new Statement();
                                break;
                            case NodeType.StatementClose:
                                return _statement;
                                break;
                            case NodeType.TransactionOpen:
                                _currentTransaction = new Transaction();
                                break;
                            case NodeType.TransactionClose:
                                _statement.Transactions.Add(_currentTransaction);
                                _currentTransaction = null;
                                break;
                            case NodeType.StatementProp:
                            case NodeType.TransactionProp:
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
                            case "Statement":
                                property.SetValue(_statement, token.Content);
                                break;
                            case "Transaction":
                                property.SetValue(_currentTransaction, token.Content);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return _statement;
        }

        private PropertyResult GetPropertyInfo(string token)
        {
            var propertyResult = new PropertyResult();

            if (typeof(Statement).GetCustomAttribute<NodeNameAttribute>().CloseTag == token)
            {
                propertyResult.Member = typeof(Statement);
                propertyResult.Type = NodeType.StatementClose;
                return propertyResult;
            }

            if (typeof(Transaction).GetCustomAttribute<NodeNameAttribute>().CloseTag == token)
            {
                propertyResult.Member = typeof(Transaction);
                propertyResult.Type = NodeType.TransactionClose;
                return propertyResult;
            }

            if (typeof(Statement).GetCustomAttribute<NodeNameAttribute>().OpenTag == token)
            {
                propertyResult.Member = typeof(Statement);
                propertyResult.Type = NodeType.StatementOpen;
                return propertyResult;
            }

            if (typeof(Transaction).GetCustomAttribute<NodeNameAttribute>().OpenTag == token)
            {
                propertyResult.Member = typeof(Transaction);
                propertyResult.Type = NodeType.TransactionOpen;
                return propertyResult;
            }


            var statementMember = typeof(Statement).GetProperties().FirstOrDefault(m => m.GetCustomAttribute<NodeNameAttribute>().OpenTag == token);

            if (statementMember != null)
            {
                propertyResult.Member = statementMember;
                propertyResult.Type = NodeType.StatementProp;
                return propertyResult;
            }

            var transactionMember = typeof(Transaction).GetProperties().Where(m => m.GetCustomAttribute<NodeNameAttribute>() != null)
                .FirstOrDefault(m => m.GetCustomAttribute<NodeNameAttribute>().OpenTag == token);

            if (transactionMember != null)
            {
                propertyResult.Member = transactionMember;
                propertyResult.Type = NodeType.TransactionProp;
                return propertyResult;
            }

            return null;

        }

    }

    internal class PropertyResult
    {
        public MemberInfo Member { get; set; }
        public NodeType Type { get; set; }
    }

    internal enum NodeType
    {
        StatementOpen,
        StatementClose,
        TransactionOpen,
        TransactionClose,
        StatementProp,
        TransactionProp
    }
}
