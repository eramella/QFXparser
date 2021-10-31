using QFXparser.Parsing;
using System;
using Xunit;

namespace QFXparser.Tests
{
    public class ParsingHelperShould
    {
        [Fact]
        public void ParseDateWithNoTimeZoneToUtc()
        {
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180525000000"));
        }

        [Fact]
        public void ParseUtcDateWithCorrectTimeZone()
        {
            Assert.Equal(new DateTime(2018, 5, 25, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180525000000[0:UTC]"));
        }

        [Fact]
        public void ParseMstDateWithCorrectTimeZone()
        {
            Assert.Equal(new DateTime(2018, 1, 19, 0, 0, 0, DateTimeKind.Utc), ParsingHelper.ParseDate("20180119000000.000[-7:MST]"));
        }
    }
}
