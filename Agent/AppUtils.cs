using BoincWatcher.Domain.Models;
using System.Text.RegularExpressions;

namespace BoincWatcher.Agent;
public class AppUtils {
    public static void LogToFile(string message, string filename = "app_log.txt") {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + filename;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        if (!File.Exists(filepath)) {
            using StreamWriter sw = File.CreateText(filepath);
            sw.WriteLine($"[{timestamp}] {message}");
        } else {
            using StreamWriter sw = File.AppendText(filepath);
            sw.WriteLine($"[{timestamp}] {message}");
        }
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
