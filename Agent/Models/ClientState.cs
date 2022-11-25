using System.Text.Json;

namespace BoincWatcher.Agent.Models;
public class ClientState {
    public HostInfo hostInfo;
    public Project[] projects;
    public App[] apps;

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
