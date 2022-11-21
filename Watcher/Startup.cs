using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Timers;
using System.Xml.Linq;
using BoincManager.Watcher.Logger;

using Timer = System.Timers.Timer;

namespace BoincManager.Watcher;

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

    private void OnTasksElapsedTime(object source, ElapsedEventArgs e) {
        // XElement boincState = this.BoincActions.GetRPCState();

        // Console.WriteLine(boincState.ToString());

        this.BoincActions.CallSocket();
    }

    private void OnStateElapsedTime(object source, ElapsedEventArgs e) {
        Console.WriteLine("OnStateElapsedTime");
        Dictionary<int, float> cpuUsage = this.Performance.GetCPUUsage();
        // float gpuUsage = Performance.GetGPUUsage();

        var stateJson = new StateJson {
            cpuUsage = cpuUsage,
            // gpuUsage = gpuUsage,
        };

        string stateJsonString = JsonSerializer.Serialize<StateJson>(stateJson);

        Console.WriteLine(stateJsonString);
        /*

        // BoincActions.GetState();
        */
    }
}
