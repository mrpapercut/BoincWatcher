using System.Diagnostics;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
namespace BoincManager.Watcher;

public class Performance {
    float[]? cpuCoreUsage { get; set; }

    static int cpuCoreCount = Environment.ProcessorCount;
    static PerformanceCounter[] cpuCounters = new PerformanceCounter[cpuCoreCount];

    float gpuUsage { get; set; }
    List<PerformanceCounter> gpuCounter;

    public async Task InitWatchers() {
        await InitCPUWatcher();
        await InitGPUWatcher();
    }

    public Task InitCPUWatcher() {
        cpuCounters = new PerformanceCounter[cpuCoreCount];
        for (int i = 0; i < cpuCoreCount; i++) {
            cpuCounters[i] = new PerformanceCounter("Processor", "% Processor Time", $"{i}");
        }

        // Perform twice to get accurate readings afterwards
        cpuCoreUsage = cpuCounters.Select(o => o.NextValue()).ToArray();
        cpuCoreUsage = cpuCounters.Select(o => o.NextValue()).ToArray();

        Console.WriteLine("CPU watcher initialized");

        return Task.CompletedTask;
    }

    public Dictionary<int, float> GetCPUUsage() {
        cpuCoreUsage = cpuCounters.Select(o => o.NextValue()).ToArray();

        var cores = new Dictionary<int, float>();

        for (int i = 0; i < cpuCoreUsage.Length; i++) {
            cores.Add(i, cpuCoreUsage[i]);
        }

        // foreach (KeyValuePair<int, float> item in cores) {
        //     Console.WriteLine($"{item.Key} => {item.Value}");
        // }

        return cores;
    }

    public Task InitGPUWatcher() {
        PerformanceCounterCategory category = new PerformanceCounterCategory("GPU Engine");
        string[] names = category.GetInstanceNames();

        gpuCounter = names.Where(counterName => counterName.EndsWith("engtype_3D"))
                            .SelectMany(counterName => category.GetCounters(counterName))
                            .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                            .ToList();

        Console.WriteLine("GPU watcher initialized");

        return Task.CompletedTask;
    }

    public float GetGPUUsage() {
        try {
            gpuCounter.ForEach(x => x.NextValue());
            Thread.Sleep(100);

            gpuCounter.ForEach(x => {
                float nextVal = x.NextValue();

                if (nextVal > 0) {
                    Console.WriteLine($"{x.InstanceName}: {nextVal}");
                }
            });
        } catch (Exception ex) {
            InitGPUWatcher();
        }

        return gpuCounter.Sum(x => x.NextValue());
    }
}
