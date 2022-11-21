using System.Text.Json;

namespace BoincManager.BoincStateModels; 
public class ClientState {
    public HostInfo hostInfo;
    public Project[] projects;
    public App[] apps;
}
