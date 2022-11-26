using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models;
public class ClientState {
    public HostInfo hostInfo;
    public NetStats netStats;
    public TimeStats timeStats;
    public Project[] projects;
    public App[] apps;
    public AppVersion[] appVersions;
    public WorkUnit[] workUnits;
    public GlobalPreferences globalPreferences;

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
