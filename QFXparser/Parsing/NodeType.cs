using System;
using System.Collections.Generic;
using System.Text;

namespace QFXparser.Parsing
{
    internal enum NodeType
    {
        StatementOpen,
        StatementClose,
        TransactionOpen,
        TransactionClose,
        StatementProp,
        TransactionProp,
        LedgerBalanceOpen,
        LedgerBalanceClose,
        LedgerBalanceProp
    }
}
