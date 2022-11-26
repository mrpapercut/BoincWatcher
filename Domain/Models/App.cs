using System.Text.Json;

namespace BoincWatcher.Domain.Models;
public class App {
    public string? Name { get; set; } = string.Empty;
    public string? UserFriendlyName { get; set; } = string.Empty;
    public bool NonCPUIntensive { get; set; } = false;

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
