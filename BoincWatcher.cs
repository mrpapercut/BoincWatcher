using System.Timers;

using Timer = System.Timers.Timer;

namespace BoincWatcher {
    internal class BoincWatcher {
        static Timer taskTimer = new();
        static Timer stateTimer = new();

        static AppConfig AppConfig;

        static BoincActions BoincActions;

        static List<BoincTask> boincTasks = new();

        static void Main(string[] args) {
            AppConfig = new AppConfig();
            AppConfig.LoadConfig();

            BoincActions = new BoincActions();
            BoincActions.SetConfig(AppConfig);

            Console.WriteLine($"Watching Primegrid on path {AppConfig.PrimegridProjectFolder}");

            using var watcher = new FileSystemWatcher(AppConfig.PrimegridProjectFolder);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;

            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            taskTimer.Elapsed += new ElapsedEventHandler(OnTasksElapsedTime);
            taskTimer.Interval = 1000;
            taskTimer.Enabled = true;

            /*
            stateTimer.Elapsed += new ElapsedEventHandler(OnStateElapsedTime);
            stateTimer.Interval = 10000;
            stateTimer.Enabled = true;
            */

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void OnCreated(object sender, FileSystemEventArgs e) {
            if (e.Name == null
                || e.Name.StartsWith("slideshow")
                || e.Name.StartsWith("stat_icon")
                || e.Name.EndsWith("cert")
                || e.Name.StartsWith("platform_nvidia")
                || e.FullPath.EndsWith(".exe")) {
                return;
            }

            try {
                string filecontents = File.ReadAllText(e.FullPath);

                if (e.Name.StartsWith("genefer")) {
                    BoincActions.HandleGenefer(sender, e, filecontents);
                } else {
                    AppUtils.LogToFile($"Created: {e.FullPath} ({e.GetType()})");
                    if (filecontents.Length < 500) {
                        AppUtils.LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                    } else {
                        AppUtils.LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                    }
                }
            } catch (IOException err) {
                // File locked by other process
                // Continue
                AppUtils.LogToFile($"IOException: {err.Message}", "errors.txt");
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e) {
            if (e.ChangeType != WatcherChangeTypes.Changed) {
                return;
            }

            if (e.Name == null
                || e.Name.StartsWith("slideshow")
                || e.Name.StartsWith("stat_icon")
                || e.Name.StartsWith("platform_nvidia")
                || e.Name.EndsWith("cert")
                || e.FullPath.EndsWith(".exe")) {
                return;
            }

            try {
                string filecontents = File.ReadAllText(e.FullPath);

                if (e.Name.StartsWith("genefer")) {
                    BoincActions.HandleGenefer(sender, e, filecontents);
                } else {
                    AppUtils.LogToFile($"Changed: {e.FullPath} ({e.GetType()})");
                    if (filecontents.Length < 500) {
                        AppUtils.LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                    } else {
                        AppUtils.LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                    }
                }
            } catch (IOException err) {
                // File locked by other process
                // Continue
                AppUtils.LogToFile($"IOException: {err.Message}", "errors.txt");
            }
        }

        private static void OnTasksElapsedTime(object source, ElapsedEventArgs e) {
            BoincActions.GetTasks();
        }

        private static void OnStateElapsedTime(object source, ElapsedEventArgs e) {
            BoincActions.GetState();
        }
    }
}
