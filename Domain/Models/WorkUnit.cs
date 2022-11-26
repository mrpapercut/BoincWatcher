using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models; 
public class WorkUnit {
    public string? Name { get; set; } = string.Empty;
    public string? AppName { get; set; } = string.Empty;
    public int AppVersionNumber { get; set; }
    public float ResourceFPOpsEstimate { get; set; }
    public float ResourceFPOpsBound { get; set; }
    public float ResourceMemoryBound { get; set; }
    public float ResourceDiskBound { get; set; }
    public string? CmdLine { get; set; } = string.Empty;
    public FileRef[]? FileRefs { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
