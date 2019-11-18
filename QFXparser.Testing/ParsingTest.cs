using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace QFXparser.Testing
{
    public class ParsingTest
    {
        public ParsingTest()
        {
            var culture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [Fact]
        public void TestSimple()
        {
            var parser = new FileParser("test.qfx");
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal(4, transactions.Count);
            Assert.Equal(-768.33m, statement.LedgerBalance.Amount);
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), statement.LedgerBalance.AsOf);
            Assert.Equal(-27.18m, transactions[0].Amount);
            Assert.Equal("AMAZON.COM AMZN.COM/BILL AMZN.CO", transactions[0].Memo);
            Assert.Equal(new DateTime(2018, 4, 27, 16, 0, 0, DateTimeKind.Utc), transactions[0].PostedOn);
            Assert.Equal(0m, transactions[2].Amount); // test for default value if invalid in .qfx
            
            Assert.Equal("133", transactions[3].CheckNum);
            Assert.Equal("CHECK", transactions[3].Type);
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
