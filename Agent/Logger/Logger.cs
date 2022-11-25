using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoincManager.Watcher.Logger;
public class Logger {
    public void Log(string message, string filename = "app_log.txt") {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + filename;
        string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        using var file = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using var sw = new StreamWriter(file);

        sw.WriteLine($"[{timestamp}] {message}");
    }
}
