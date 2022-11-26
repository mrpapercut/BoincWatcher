using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models;

public class AppVersion {
    public string? AppName { get; set; } = string.Empty;
    public int Version { get; set; }
    public string? Platform { get; set; } = string.Empty;
    public float AverageNumberCPUs { get; set; }
    public float Flops { get; set; }
    public string? PlanClass { get; set; } = string.Empty;
    public string? ApiVersion { get; set; } = string.Empty;
    public string? CmdLine { get; set; } = string.Empty;
    public FileRef[]? FileRefs { get; set; }
    public CoProcessor[]? CoProcessors { get; set; }
    public float GPURam { get; set; }
    public bool DontThrottle { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
