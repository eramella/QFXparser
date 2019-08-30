using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QFXparser
{
    internal static class ParsingHelper
    {
        private static readonly Regex _dateTimeRegex = new Regex(
         "^(?<year>\\d{4})(?<month>\\d{2})(?<day>\\d{2})(?<hour>\\d{2})(?<min>\\d{2})(?<sec>\\d{2})(\\[(?<tzId>\\d+):(?<timezone>\\w{3})\\])?");

        public static DateTime? ParseDate(string content)
        {
            DateTime? result;
            var match = _dateTimeRegex.Match(content);
            if (!match.Success)
            {
                result = null;
            }
            else
            {
                var groups = match.Groups;
                var year = int.Parse(groups["year"].Value);
                var month = int.Parse(groups["month"].Value);
                var day = int.Parse(groups["day"].Value);
                var hour = int.Parse(groups["hour"].Value);
                var minute = int.Parse(groups["min"].Value);
                var second = int.Parse(groups["sec"].Value);
                var timeZone = groups["timezone"].Success ? groups["timezone"].Value : "UTC";

                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                result = new DateTimeOffset(year, month, day, hour, minute, second, timeZoneInfo.BaseUtcOffset).ToUniversalTime().DateTime;
            }
            return result;
        }
    }
}
