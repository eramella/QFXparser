using System;
using System.Linq;
using Xunit;

namespace QFXparser.Testing
{
    public class ParsingTest
    {
        [Fact]
        public void TestCreditCard()
        {
            var parser = new FileParser("CreditCard.qfx");
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal(3, transactions.Count);
            Assert.Equal(-768.33m, statement.LedgerBalance.Amount);
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), statement.LedgerBalance.AsOf);
            Assert.Equal(-27.18m, transactions[0].Amount);
            Assert.Equal(-768.33m, transactions[0].Balance);
            Assert.Equal("AMAZON.COM AMZN.COM/BILL AMZN.CO", transactions[0].Memo);
            Assert.Equal(new DateTime(2018, 4, 27, 16, 0, 0, DateTimeKind.Utc), transactions[0].PostedOn);
            Assert.Equal(-25m, transactions[1].Amount);
            Assert.Equal(-741.15m, transactions[1].Balance);
            Assert.Equal("NEW JERSEY E-ZPASS 888-288-6865", transactions[1].Memo);
            Assert.Equal(0m, transactions[2].Amount); // test for default value if invalid in .qfx
        }

        [Fact]
        public void TestCreditCardNoLedgerBalance()
        {
            var parser = new FileParser("CreditCardNoLedgerBalance.qfx");
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal(2, transactions.Count);
            Assert.Null(statement.LedgerBalance);
            Assert.Equal(-27.18m, transactions[0].Amount);
            Assert.Null(transactions[0].Balance);
            Assert.Equal("AMAZON.COM AMZN.COM/BILL AMZN.CO", transactions[0].Memo);
            Assert.Equal(new DateTime(2018, 4, 27, 16, 0, 0, DateTimeKind.Utc), transactions[0].PostedOn);
            Assert.Equal(-25m, transactions[1].Amount);
            Assert.Null(transactions[1].Balance);
            Assert.Equal("NEW JERSEY E-ZPASS 888-288-6865", transactions[1].Memo);
        }


        [Fact]
        public void TestDateTimeNoTimeZoneAssumesUtc()
        {
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180525000000"));
        }

        [Fact]
        public void TestDateTimeWithUtcTimeZone()
        {
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180525000000[0:UTC]"));
        }

        [Fact]
        public void TestDateTimeWithMstTimeZone()
        {
            Assert.Equal(new DateTime(2018, 1, 19, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180119000000.000[-7:MST]"));
        }
    }
}
