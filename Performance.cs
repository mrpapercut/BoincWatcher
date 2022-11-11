using System.Diagnostics;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]
namespace BoincWatcher {
    internal class Performance {
        float[]? cpuCoreUsage { get; set; }

        static int cpuCoreCount = Environment.ProcessorCount;
        static PerformanceCounter[] cpuCounters = new PerformanceCounter[cpuCoreCount];

        float gpuUsage { get; set; }
        List<PerformanceCounter> gpuCounter;

        public void InitWatchers() {
            InitCPUWatcher();
            InitGPUWatcher();
        }

        public void InitCPUWatcher() {
            cpuCounters = new PerformanceCounter[cpuCoreCount];
            for (int i = 0; i < cpuCoreCount; i++) {
                cpuCounters[i] = new PerformanceCounter("Processor", "% Processor Time", $"{i}");
            }

            // Perform twice to get accurate readings afterwards
            cpuCoreUsage = cpuCounters.Select(o => o.NextValue()).ToArray();
            cpuCoreUsage = cpuCounters.Select(o => o.NextValue()).ToArray();

            Console.WriteLine("CPU watcher initialized");
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

        public void InitGPUWatcher() {
            PerformanceCounterCategory category = new PerformanceCounterCategory("GPU Engine");
            string[] names = category.GetInstanceNames();

            gpuCounter = names.Where(counterName => counterName.EndsWith("engtype_3D"))
                              .SelectMany(counterName => category.GetCounters(counterName))
                              .Where(counter => counter.CounterName.Equals("Utilization Percentage"))
                              .ToList();

            gpuCounter.ForEach(x => x.NextValue());
        }

        public float GetGPUUsage() {
            gpuCounter.ForEach(x => x.NextValue());

            return gpuCounter.Sum(x => x.NextValue());
        }
    }
}
