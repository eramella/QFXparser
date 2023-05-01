﻿using System.Collections.Generic;

namespace QFXparser
{
    public class Statement
    {
        public string AccountNum { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public LedgerBalance LedgerBalance { get; set; } = new LedgerBalance();
    //    public AvailableBalance AvailableBalance { get; set; } = new AvailableBalance();
    }
}
