using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace QFXparser.Tests
{
    public class FileParserShould
    {
        [Fact]
        public void ReturnCorrectValuesWithInvariantCulture()
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
        public void ParseSpecialCharWithCharset1252()
        {
            Encoding encoding1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            using (var sr = new StreamReader("testSpecialChars.qfx", encoding1252))
            {
                var parser = new FileParser(sr, CultureInfo.InvariantCulture);
                var statement = parser.BuildStatement();
                var transactions = statement.Transactions.ToList();
                Assert.Equal("Retrait - Internet - Compte d'ép", transactions[0].Name);
            }
        }

        [Fact]
        public void ParseSpecialCharCorrectly()
        {
            var parser = new FileParser("testSpecialChars.qfx");
            var statement = parser.BuildStatement();
            var transactions = statement.Transactions.ToList();
            Assert.Equal("Retrait - Internet - Compte d'ép", transactions[0].Name);
        }
        [Fact]
        public void ParseSpecialCharWithInvariantCultureNotReturningCorrectChar()
        {
            using (var sr = new StreamReader("testSpecialChars.qfx"))
            {
                var parser = new FileParser(sr, CultureInfo.InvariantCulture);
                var statement = parser.BuildStatement();
                var transactions = statement.Transactions.ToList();
                Assert.NotEqual("Retrait - Internet - Compte d'ép", transactions[0].Name);
            }
        }
    }
}
