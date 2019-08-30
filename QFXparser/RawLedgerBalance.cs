using System;

namespace QFXparser
{
    [NodeName("LEDGERBAL", "/LEDGERBAL")]
    public class RawLedgerBalance
    {
        [NodeName("BALAMT")]
        public decimal Amount { get; set; }

        [NodeName("DTASOF")]
        public DateTime AsOf { get; set; }
    }
}