using System;

namespace QFXparser
{
    public class Transaction
    {
        public string Type { get; set; }
        public DateTime PostedOn { get; set; }
        public Decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string RefNumber { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
        public Decimal? Balance { get; set; }
    }
}
