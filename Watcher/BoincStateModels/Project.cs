using System.Text.Json;

namespace BoincManager.BoincStateModels; 
public class Project {
    public string? MasterUrl { get; set; } = string.Empty;
    public string? ProjectName { get; set; } = string.Empty;
    public string? ProjectDirectory { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string? UserName { get; set; } = string.Empty;
    public float UserTotalCredit { get; set; }
    public float UserExpectedAverageCredit { get; set; }
    public int UserCreateTime { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; } = string.Empty;
    public int HostId { get; set; }
    public string? HostVenue { get; set; } = string.Empty;
    public float HostTotalCredit { get; set; }
    public float HostExpectedAverageCredit { get; set; }
    public int HostCreateTime { get; set; }
    public string? CrossProjectId { get; set; } = string.Empty;
    public string? ExternalCrossProjectId { get; set; } = string.Empty;
    public bool DontRequestMoreWork { get; set; }
    public float DurationCorrectionFactor { get; set; }
    public int NumJobsSuccess { get; set; }
    public int NumJobsError { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
