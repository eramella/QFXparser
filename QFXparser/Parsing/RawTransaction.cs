using System;

namespace QFXparser.Parsing
{
    [NodeName("STMTTRN", "/STMTTRN")]
    internal class RawTransaction
    {
        [NodeName("TRNTYPE")]
        public string Type { get; set; }

        [NodeName("DTPOSTED")]
        public DateTime PostedOn { get; set; }

        [NodeName("TRNAMT")]
        public Decimal Amount { get; set; }

        [NodeName("FITID")]
        public string TransactionId { get; set; }

        [NodeName("REFNUM")]
        public string RefNumber { get; set; }

        [NodeName("NAME")]
        public string Name { get; set; }

        [NodeName("MEMO")]
        public string Memo { get; set; }
    }
}
