using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using BoincManager.BoincStateModels;

namespace BoincManager.Watcher;
public class BoincActions {
    private AppConfig AppConfig;

    public BoincActions(AppConfig appConfig) {
        this.AppConfig = appConfig;
    }

    public async Task<string> CallSocket(string command) {
        string cleanResponse = "";

        IPEndPoint ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 31416);
        using (Socket client = new(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)) {
            await client.ConnectAsync(ipEndPoint);

            string commandToSend = $"<boinc_gui_rpc_request>\n{command}</boinc_gui_rpc_request>\n\u0003";
            byte[] cmdBytes = Encoding.UTF8.GetBytes(commandToSend);

            _ = await client.SendAsync(cmdBytes, SocketFlags.None);

            byte[] buff = new byte[client.ReceiveBufferSize * 2];

            string response = "";
            int received = await client.ReceiveAsync(buff, SocketFlags.None);
            response += Encoding.UTF8.GetString(buff, 0, received);
            while (client.Available > 0) {
                buff = new byte[client.ReceiveBufferSize * 2];
                received = await client.ReceiveAsync(buff, SocketFlags.None);
                response += Encoding.UTF8.GetString(buff, 0, received);
            }

            // Clean response
            Regex re = new Regex(@"/boinc_gui_rpc_reply>\n.*$", RegexOptions.Multiline);
            cleanResponse = re.Replace(response, "/boinc_gui_rpc_reply>");
        }

        return cleanResponse;
    }

    public async Task<ClientState> GetState() {
        string response = await this.CallSocket("<get_state>\n");

        XElement resXml = XElement.Parse(response);

        XElement clientState = resXml.Element("client_state");

        if (clientState == null) {
            throw new Exception("Error: client_state is null");
        }

        try {
            // Console.WriteLine(resXml.ToString());
            ClientState parsedState = BoincParser.ParseClientState(clientState);

            return parsedState;
        } catch (Exception ex) {
            throw ex;
        }
    }

    private void ProcessState(string state) {
        // Submit relevant info to central db
    }

    public void GetTasks() {
        /*
        string tasks = "";

        Process Boinc = GetBoincProcess();
        Boinc.StartInfo.Arguments = "--get_tasks";
        Boinc.Start();

        tasks += Boinc.StandardOutput.ReadToEnd();

        Boinc.WaitForExit();

        ProcessTasks(tasks);
        */
    }

    private void ProcessTasks(string alltasks) {
        // Loop through contents to parse individual BoincTasks
        // Then add them to a list
        // Remove finished (completed/aborted) tasks from list (cleanup in case they weren't removed already)
        // Submit to central server
        string[] tasks = Regex.Split(alltasks, @"\d+\)\s[\-]+");

        Regex lineRE = new Regex(@"^([A-Za-z_\s]+):\s(.*)");

        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.NumberFormat.NumberDecimalSeparator = ".";

        for (int i = 0; i < tasks.Length; i++) {
            if (tasks[i].StartsWith("\r\n=")) continue;

            string[] tasklines = tasks[i].Split("\r\n");

            BoincTask boincTask = new();

            foreach (string line in tasklines) {
                string cleanStr = line.Trim();

                Match match = lineRE.Match(cleanStr);

                string value = match.Groups[2].Value;

                switch (match.Groups[1].Value) {
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
                        boincTask.Received = AppUtils.ParseDateString(value);
                        break;

                    case "report deadline":
                        boincTask.ReportDeadline = AppUtils.ParseDateString(value);
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

            // Console.WriteLine(boincTask.ToJSON());
            // Console.WriteLine();
            PostToServer("/task", boincTask.ToJSON());
        }

        // If a task fails to parse, log it to a file per workunit
    }

    public void HandleGenefer(object sender, FileSystemEventArgs e, string filecontents) {
        string createdOrChanged = (e.ChangeType == WatcherChangeTypes.Created) ? "created" : "changed";

        AppUtils.LogToFile($"Genefer file {e.Name} {createdOrChanged}\nContents:\n{filecontents}", "genefer.txt");

        // Find corresponding BoincTask
        // Submit updated task to central server
        // Remove from list
    }  

    public void PostToServer(string endpoint, string message) {
        string url = $"http://localhost:8080{endpoint}";

        using var client = new HttpClient();
        var result = client.PostAsync(url, new StringContent(message, Encoding.UTF8, "application/json")).Result;

        // Console.WriteLine($"Posted to server: {result}");
    }
}
