using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Timers;
using System.Xml.Linq;
using BoincWatcher.Domain.Models;
using BoincWatcher.Agent.Logger;

using Timer = System.Timers.Timer;

namespace BoincWatcher.Agent;

public class StateJson {
    public float gpuUsage { get; set; }
    public Dictionary<int, float>? cpuUsage { get; set; }
}

public class TasksJson {

}

public class Startup {
    public readonly AppConfig AppConfig = new();
    
    public readonly Performance Performance = new();
    
    public readonly BoincActions BoincActions;

    public readonly FolderWatcher FolderWatcher;

    private Timer taskTimer = new();
    private Timer stateTimer = new();

    /*
    static List<BoincTask> boincTasks = new();
    */

    public Startup(string[] args) {
        this.AppConfig = new AppConfig();
        this.AppConfig.LoadConfig();

        this.BoincActions = new BoincActions(this.AppConfig);

        this.FolderWatcher = new FolderWatcher(this.AppConfig, this.BoincActions);
    }

    public static async Task RunAsync(string[] args) {
        var startup = new Startup(args);

        await startup.RunAsync();
    }

    private async Task RunAsync() {
        Console.WriteLine(this.AppConfig.PrimegridProjectFolder);

        // Start Performance watchers
        await this.Performance.InitWatchers();

        // Start FolderWatcher
        this.FolderWatcher.StartWatcher();

        // Start interval times
        this.StartTimers();

        await Task.Delay(-1);
    }

    private void StartTimers() {
        this.taskTimer.Elapsed += new ElapsedEventHandler(this.OnTasksElapsedTime);
        this.taskTimer.Interval = 3000;
        this.taskTimer.Enabled = true;

        this.stateTimer.Elapsed += new ElapsedEventHandler(this.OnStateElapsedTime);
        this.stateTimer.Interval = 10000;
        this.stateTimer.Enabled = true;
    }

    private async void OnTasksElapsedTime(object? source, ElapsedEventArgs e) {
        /*this.BoincActions.GetResults();*/
        /*this.BoincActions.GetProjects();*/
        /*this.BoincActions.GetOldResults();*/
    }

    private async void OnStateElapsedTime(object? source, ElapsedEventArgs e) {
        Console.WriteLine("OnStateElapsedTime");
        /*
        Dictionary<int, float> cpuUsage = this.Performance.GetCPUUsage();
        // float gpuUsage = Performance.GetGPUUsage();

        var stateJson = new StateJson {
            cpuUsage = cpuUsage,
            // gpuUsage = gpuUsage,
        };

        string stateJsonString = JsonSerializer.Serialize<StateJson>(stateJson);

        Console.WriteLine(stateJsonString);
        */

        try {
            ClientState clientState = await this.BoincActions.GetState();

            Console.WriteLine("{");
            Console.WriteLine($"\"hostInfo\": {clientState.globalPreferences.ToJSON()},");
            Console.WriteLine($"\"netStats\": {clientState.netStats.ToJSON()},");
            Console.WriteLine($"\"timeStats\": {clientState.timeStats.ToJSON()},");

            Console.Write($"\"projects\": [");
            List<string> projects = new List<string>();
            foreach (Project p in clientState.projects) {
                projects.Add(p.ToJSON());
            }
            Console.Write(string.Join(",\n", projects.ToArray()));
            Console.WriteLine("],");

            Console.Write($"\"apps\": [");
            List<string> apps = new List<string>();
            foreach (App a in clientState.apps) {
                apps.Add(a.ToJSON());
            }
            Console.Write(string.Join(",\n", apps.ToArray()));
            Console.WriteLine("],");

            Console.Write($"\"app_versions\": [");
            List<string> appversions = new List<string>();
            foreach (AppVersion av in clientState.appVersions) {
                appversions.Add(av.ToJSON());
            }
            Console.Write(string.Join(",\n", appversions.ToArray()));
            Console.WriteLine("],");

            Console.Write($"\"workunits\": [");
            List<string> wus = new List<string>();
            foreach (WorkUnit wu in clientState.workUnits) {
                wus.Add(wu.ToJSON());
            }
            Console.Write(string.Join(",\n", wus.ToArray()));
            Console.WriteLine("],");

            Console.WriteLine($"\"globalPreferences\": {clientState.globalPreferences.ToJSON()}");

            Console.WriteLine("}");

        } catch (Exception ex) {
            Console.WriteLine($"Error: {ex}");
        }
    }
}
