using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models;
public class Result {
    public string? Name { get; set; }
    public string? WUName { get; set; }
    public string? Platform { get; set; }
    public int AppVersionNum { get; set; }
    public string? PlanClass { get; set; }
    public string? ProjectURL { get; set; }
    public float? FinalCPUTime { get; set; } = 0;
    public float? FinalElapsedTime { get; set; } = 0;
    public int? ExitStatus { get; set; } = -1;
    public int State { get; set; }
    public DateTime ReportDeadline { get; set; }
    public DateTime Received { get; set; }
    public float? EstimatedCPUTimeRemaining { get; set; } = 0;
    public string? Resources { get; set; }
    public bool ReportImmediately { get; set; }
    public int ActiveTaskState { get; set; }
    public int SchedulerState { get; set; }
    public bool? SuspendedViaGUI { get; set; } = false;
    public float? ElapsedTaskTime { get; set; } = 0;
    public int? Slot { get; set; } = -1;
    public int? PID { get; set; } = -1;
    public float? CPUTimeAtLastCheckpoint { get; set; } = 0;
    public float? CurrentCPUTime { get; set; } = 0;
    public float? FractionDone { get; set; } = 0;
    public string? SwapSize { get; set; } = string.Empty;
    public string? WorkingSetSize { get; set; } = string.Empty;
    public string? BytesSentReceived { get; set; } = string.Empty;
    public int? Signal { get; set; } = -1;
    public bool ReadyToReport { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
