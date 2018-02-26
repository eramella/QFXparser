using System;
using System.Collections.Generic;
using System.Text;

namespace QFXparser
{
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
