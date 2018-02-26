using System;
using System.Globalization;

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
                var dateStr = PostedOn.Substring(0, 18) + "Z";
                var tzstr = PostedOn.Substring(19).Split(':');
                var timeZoneName = tzstr[1].TrimEnd(']');
                var timeSpan = Convert.ToDouble(tzstr[0]);
                var date = DateTimeOffset.ParseExact(dateStr, "yyyyMMddHHmmss.fffZ", CultureInfo.InvariantCulture);
                var tzi = TimeZoneInfo.CreateCustomTimeZone(timeZoneName, TimeSpan.FromHours(timeSpan), timeZoneName, timeZoneName);
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
