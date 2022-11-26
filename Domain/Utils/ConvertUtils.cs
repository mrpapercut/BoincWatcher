using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoincWatcher.Domain.Utils;

public class ConvertUtils {
    public static float ParseFloat(string? flt) {
        return float.Parse(flt ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    public static DateTime ParseDateString(string datestr) {
        Regex re = new Regex(@"^\w+\s(\w+)\s*(\d{1,2})\s(\d{2}):(\d{2}):(\d{2})\s(\d{4})");

        string[] months = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        MatchCollection matches = re.Matches(datestr);
        Match match = matches[0];

        int month = Array.IndexOf(months, match.Groups[1].Value) + 1;
        string day = match.Groups[2].Value;
        string hour = match.Groups[3].Value;
        string minutes = match.Groups[4].Value;
        string seconds = match.Groups[5].Value;
        string year = match.Groups[6].Value;

        string dateInput = $"{year}-{month}-{day} {hour}:{minutes}:{seconds}";

        return DateTime.Parse(dateInput);
    }

    public static DateTime ParseDateFloat(float dateflt) {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((int)dateflt);

        DateTime dateTime = dateTimeOffset.UtcDateTime;

        return dateTime;
    }

    public static DateTimeOffset ParsePartialDateFloat(float dateflt) {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((int)dateflt);

        return dateTimeOffset;
    }
}
