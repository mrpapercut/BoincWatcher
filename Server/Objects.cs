using System.Runtime.Serialization;
using System.Text.Json;

namespace BoincManager.Server.Objects; 

[DataContract]
public class BoincTask {
    [DataMember]
    public string Name { get; set; }
        
    [DataMember]
    public string WUName { get; set; }
        
    [DataMember]
    public DateTime Received { get; set; }
        
    [DataMember]
    public DateTime ReportDeadline { get; set; }
        
    [DataMember]
    public int ProjectId { get; set; }
        
    [DataMember]
    public int State { get; set; }
        
    [DataMember]
    public int ActiveTaskState { get; set; }
        
    [DataMember]
    public int SchedulerState { get; set; }
        
    [DataMember]
    public float ElapsedTime { get; set; }
        
    [DataMember]
    public float RemainingTime { get; set; }
        
    [DataMember]
    public float FinalElapsedTime { get; set; }
        
    [DataMember]
    public int ExitStatus { get; set; }
        
    [DataMember]
    public float FractionDone { get; set; }
        
    [DataMember]
    public string Resources { get; set; }
        
    [DataMember]
    public bool SuspendedViaGui { get; set; }

    [DataMember]
    public bool IsFastDc { get; set; }

    public BoincTask() { }

    public BoincTask(string name, string wuName, string received, string deadline, int projectId, int state, int activeTaskState, int schedulerState, float elapsedTime, float remainingTime, float finalElapsedTime, int exitStatus, float fractionDone, string resources, bool suspendedViaGui, bool isFastDc) {
        this.Name = name;
        this.WUName = wuName;
        this.Received = DateTime.Parse(received);
        this.ReportDeadline = DateTime.Parse(deadline);
        this.ProjectId = projectId;
        this.State = state;
        this.ActiveTaskState = activeTaskState;
        this.SchedulerState = schedulerState;
        this.ElapsedTime = elapsedTime;
        this.RemainingTime = remainingTime;
        this.FinalElapsedTime = finalElapsedTime;
        this.ExitStatus = exitStatus;
        this.FractionDone = fractionDone;
        this.Resources = resources;
        this.SuspendedViaGui = suspendedViaGui;
        this.IsFastDc = isFastDc;
    }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
