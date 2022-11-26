using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models;

public class NetStats {
    public float BandwidthUp { get; set; }
    public float BandwidthDown { get; set; }
    public float AverageUp { get; set; }
    public float AverageDown { get; set; }
    public float AverageTimeUp { get; set; }
    public float AverageTimeDown { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
