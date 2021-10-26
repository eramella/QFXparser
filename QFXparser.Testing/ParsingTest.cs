using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace QFXparser.Testing
{
    public class ParsingTest
    {
        [Fact]
        public void TestSimple()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var parser = new FileParser("test.qfx");
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal(3, transactions.Count);
            Assert.Equal(-768.33m, statement.LedgerBalance.Amount);
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), statement.LedgerBalance.AsOf);
            Assert.Equal(-27.18m, transactions[0].Amount);
            Assert.Equal("AMAZON.COM AMZN.COM/BILL AMZN.CO", transactions[0].Memo);
            Assert.Equal(new DateTime(2018, 4, 27, 16, 0, 0, DateTimeKind.Utc), transactions[0].PostedOn);
            Assert.Equal(0m, transactions[2].Amount); // test for default value if invalid in .qfx
        }

        [Fact]
        public void TestCp1252()
        {
            Encoding encoding1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            var parser = new FileParser(new StreamReader("testSpecialChars.qfx", encoding1252), CultureInfo.InvariantCulture);
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal("Retrait - Internet - Compte d'ép", transactions[0].Name);
        }

        [Fact]
        public void TestUtf8WithSpecialChars()
        {
            var parser = new FileParser(new StreamReader("testSpecialChars.qfx"), CultureInfo.InvariantCulture);
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.NotEqual("Retrait - Internet - Compte d'ép", transactions[0].Name);
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
