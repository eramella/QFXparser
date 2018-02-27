using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace QFXparser
{
    [NodeName("STMTTRN", "/STMTTRN")]
    internal class RawTransaction
    {
        [NodeName("TRNTYPE")]
        public string Type { get; set; }

        [NodeName("DTPOSTED")]
        public String PostedOn { get; set; } //20180119000000.000[-7:MST]

        [NodeName("TRNAMT")]
        public String StrAmount { get; set; }

        [NodeName("FITID")]
        public string TransactionId { get; set; }

        [NodeName("REFNUM")]
        public string RefNumber { get; set; }

        [NodeName("NAME")]
        public string Name { get; set; }

        [NodeName("MEMO")]
        public string Memo { get; set; }

        public DateTime DatePosted
        {
            get
            {
                var dateStr = PostedOn.Substring(0, 12) + "Z";
                Regex regex = new Regex(@"(?<=\[)([^)]+)(?=\])");
                var tzstr = regex.Match(PostedOn).Groups[0].Value;
                var tzstrSplit = tzstr.Split(':');
                var timeSpan = Convert.ToDouble(tzstrSplit[0]);
                var date = DateTimeOffset.ParseExact(dateStr, "yyyyMMddHHmmZ", CultureInfo.InvariantCulture);
                var tzi = TimeZoneInfo.CreateCustomTimeZone(tzstrSplit[1], TimeSpan.FromHours(timeSpan), tzstrSplit[1], tzstrSplit[1]);
                var newdate = TimeZoneInfo.ConvertTime(date, tzi);
                return newdate.DateTime;
            }
        }

        public Decimal Amount
        {
            get
            {
                decimal amount = 0;
                Decimal.TryParse(StrAmount, out amount);
                return amount;
            }
        }
    }
}
