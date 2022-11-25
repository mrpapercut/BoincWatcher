using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BoincManager.Server.Models; 
public class Task {
    public int Id { get; set; }
    public string Name { get; set; }

    public string WUName { get; set; }

    public DateTime Received { get; set; }

    public DateTime ReportDeadline { get; set; }

    public int ProjectId { get; set; }

    public string ProjectURL { get; set; }

    public int State { get; set; }

    public int ActiveTaskState { get; set; }

    public int SchedulerState { get; set; }

    public float ElapsedTime { get; set; }

    public float RemainingTime { get; set; }

    public float FinalElapsedTime { get; set; }

    public int ExitStatus { get; set; }

    public float FractionDone { get; set; }

    public string Resources { get; set; }

    public bool SuspendedViaGui { get; set; }

    public bool IsFastDc { get; set; }

    public Instance instance { get; set; }
}
