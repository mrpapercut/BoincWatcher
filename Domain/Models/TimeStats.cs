using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models; 
public class TimeStats {
    public float UptimePercentage { get; set; }
    public float ConnectedPercentage { get; set; }
    public float CPUAndNetworkAvailablePercentage { get; set; }
    public float ActivePercentage { get; set; }
    public float GPUActivePercentage { get; set; }
    public DateTime ClientStartTime { get; set; }
    public DateTime TotalStartTime { get; set; }
    public float TotalDuration { get; set; }
    public float TotalActiveDuration { get; set; }
    public float TotalGPUActiveDuration { get; set; }
    public DateTime Now { get; set; }
    public float PreviousUptime { get; set; }
    public float SessionActiveDuration { get; set; }
    public float SessionGPUActiveDuration { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
