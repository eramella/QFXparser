using System;
using System.Collections.Generic;
using System.Text;

namespace QFXparser.Core
{
    [NodeName("CCSTMTRS")]
    public class Statement
    {
        [NodeName("ACCTID")]
        public string AccountNum { get; set; }

        [NodeName("BANKTRANLIST")]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
