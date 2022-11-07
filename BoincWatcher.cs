using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Timers;

using Timer = System.Timers.Timer;

namespace BoincWatcher
{
    internal class BoincWatcher
    {
        // TODO: Read from Registry (Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Space Sciences Laboratory, U.C. Berkeley\BOINC Setup)
        // OR read from Config
        // Config overrides Registry, fail if neither provided
        static readonly string primegridProjectPath = @"C:\ProgramData\BOINC\projects\www.primegrid.com";

        static readonly string boincMgrPath = @"C:\Program Files\BOINC\boinccmd.exe";

        static Timer taskTimer = new();
        static Timer stateTimer = new();

        static List<BoincTask> boincTasks = new();

        static void Main(string[] args)
        {
            Console.WriteLine($"Watching Primegrid on path {primegridProjectPath}");

            using var fsWatcher = new FileSystemWatcher(primegridProjectPath);

            fsWatcher.NotifyFilter = NotifyFilters.Attributes
                                   | NotifyFilters.CreationTime
                                   | NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.Security
                                   | NotifyFilters.Size;

            fsWatcher.Changed += OnChanged;
            fsWatcher.Created += OnCreated;

            fsWatcher.EnableRaisingEvents = true;
            fsWatcher.IncludeSubdirectories = true;

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

        private static void LogToFile(string message, string filename = "app_log.txt")
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + filename;

            if (!File.Exists(filepath))
            {
                using StreamWriter sw = File.CreateText(filepath);
                sw.WriteLine(message);
            } else
            {
                using StreamWriter sw = File.AppendText(filepath);
                sw.WriteLine(message);
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.Name == null
                || e.Name.StartsWith("slideshow")
                || e.Name.StartsWith("stat_icon")
                || e.Name.EndsWith("cert")
                || e.Name.StartsWith("platform_nvidia")
                || e.FullPath.EndsWith(".exe"))
            {
                return;
            }

            try
            {
                string filecontents = File.ReadAllText(e.FullPath);

                if (e.Name.StartsWith("genefer"))
                {
                    HandleGenefer(sender, e, filecontents);
                }
                else
                {
                    LogToFile($"Created: {e.FullPath} ({e.GetType()})");
                    if (filecontents.Length < 500)
                    {
                        LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                    }
                    else
                    {
                        LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                    }
                }
            } catch (IOException err)
            {
                // File locked by other process
                // Continue
                LogToFile($"IOException: {err.Message}", "errors.txt");
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            if (e.Name == null
                || e.Name.StartsWith("slideshow")
                || e.Name.StartsWith("stat_icon")
                || e.Name.StartsWith("platform_nvidia")
                || e.Name.EndsWith("cert")
                || e.FullPath.EndsWith(".exe"))
            {
                return;
            }

            try
            {
                string filecontents = File.ReadAllText(e.FullPath);

                if (e.Name.StartsWith("genefer"))
                {
                    HandleGenefer(sender, e, filecontents);
                }
                else
                {
                    LogToFile($"Changed: {e.FullPath} ({e.GetType()})");
                    if (filecontents.Length < 500)
                    {
                        LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                    }
                    else
                    {
                        LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                    }
                }
            } catch (IOException err)
            {
                // File locked by other process
                // Continue
                LogToFile($"IOException: {err.Message}", "errors.txt");
            }
        }

        private static void OnTasksElapsedTime(object source, ElapsedEventArgs e)
        {
            GetBoincTasks();
        }

        private static void OnStateElapsedTime(object source, ElapsedEventArgs e)
        {
            GetBoincState();
        }

        private static Process GetBoincProcess()
        {
            Process p = new();
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = boincMgrPath;

            return p;
        }

        private static void GetBoincState()
        {
            string state = "";

            Process Boinc = GetBoincProcess();
            Boinc.StartInfo.Arguments = "--get_state";
            Boinc.Start();

            state += Boinc.StandardOutput.ReadToEnd();

            Boinc.WaitForExit();

            ProcessState(state);
        }

        private static void ProcessState(string state)
        {
            // Submit relevant info to central db
        }

        private static void GetBoincTasks()
        {
            string tasks = "";

            Process Boinc = GetBoincProcess();
            Boinc.StartInfo.Arguments = "--get_tasks";
            Boinc.Start();

            tasks += Boinc.StandardOutput.ReadToEnd();

            Boinc.WaitForExit();

            ProcessTasks(tasks);
        }

        private static void ProcessTasks(string alltasks)
        {
            // Loop through contents to parse individual BoincTasks
            // Then add them to a list
            // Remove finished (completed/aborted) tasks from list (cleanup in case they weren't removed already)
            // Submit to central server
            string[] tasks = Regex.Split(alltasks, @"\d+\)\s[\-]+");

            Regex lineRE = new Regex(@"^([A-Za-z_\s]+):\s(.*)");

            var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].StartsWith("\r\n=")) continue;

                string[] tasklines = tasks[i].Split("\r\n");

                BoincTask boincTask = new();

                foreach (string line in tasklines)
                {
                    string cleanStr = line.Trim();

                    Match match = lineRE.Match(cleanStr);

                    string value = match.Groups[2].Value;

                    switch (match.Groups[1].Value)
                    {
                        case "name":
                            boincTask.Name = value;
                            break;

                        case "WU name":
                            boincTask.WUName = value;
                            break;

                        case "project URL":
                            boincTask.ProjectURL = value;
                            break;

                        case "received":
                            boincTask.Received = ParseDateString(value);
                            break;

                        case "report deadline":
                            boincTask.ReportDeadline = ParseDateString(value);
                            break;

                        case "ready to report":
                            boincTask.ReadyToReport = value == "yes";
                            break;

                        case "state":
                            boincTask.State = Array.IndexOf(BoincStrings.ResultClientState, value);
                            break;

                        case "scheduler state":
                            boincTask.SchedulerState = Array.IndexOf(BoincStrings.ResultSchedulerState, value);
                            break;

                        case "active_task_state":
                            boincTask.ActiveTaskState = Array.IndexOf(BoincStrings.ResultActiveTaskState, value);
                            break;

                        case "app version num":
                            boincTask.AppVersionNum = int.Parse(value);
                            break;

                        case "resources":
                            boincTask.Resources = value;
                            break;

                        case "suspended via GUI":
                            boincTask.SuspendedViaGUI = value == "yes";
                            break;

                        case "estimated CPU time remaining":
                            boincTask.EstimatedCPUTimeRemaining = float.Parse(value, culture);
                            break;

                        case "elapsed task time":
                            boincTask.ElapsedTaskTime = float.Parse(value, culture);
                            break;

                        case "slot":
                            boincTask.Slot = int.Parse(value);
                            break;

                        case "PID":
                            boincTask.PID = int.Parse(value);
                            break;

                        case "CPU time at last checkpoint":
                            boincTask.CPUTimeAtLastCheckpoint = float.Parse(value, culture);
                            break;

                        case "current CPU time":
                            boincTask.CurrentCPUTime = float.Parse(value, culture);
                            break;

                        case "fraction done":
                            boincTask.FractionDone = float.Parse(value, culture);
                            break;

                        case "swap size":
                            boincTask.SwapSize = value;
                            break;

                        case "working set size":
                            boincTask.WorkingSetSize = value;
                            break;

                        case "bytes sent":
                            boincTask.BytesSentReceived = cleanStr;
                            break;

                        case "final CPU time":
                            boincTask.FinalCPUTime = float.Parse(value, culture);
                            break;

                        case "final elapsed time":
                            boincTask.FinalElapsedTime = float.Parse(value, culture);
                            break;

                        case "exit_status":
                            boincTask.ExitStatus = int.Parse(value);
                            break;

                        case "signal":
                            boincTask.Signal = int.Parse(value);
                            break;
                    }
                }

                Console.WriteLine(boincTask.ToJSON());
                Console.WriteLine();
            }

            // If a task fails to parse, log it to a file per workunit
        }

        private static DateTime ParseDateString(string datestr)
        {
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

        private static void HandleGenefer(object sender, FileSystemEventArgs e, string filecontents)
        {
            string createdOrChanged = (e.ChangeType == WatcherChangeTypes.Created) ? "created" : "changed";

            LogToFile($"Genefer file {e.Name} {createdOrChanged}\nContents:\n{filecontents}", "genefer.txt");

            // Find corresponding BoincTask
            // Submit updated task to central server
            // Remove from list
        }
    }
}