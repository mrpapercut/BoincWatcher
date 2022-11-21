using System.Text.Json;

namespace BoincManager.BoincStateModels; 
public class HostInfo {
    public int Timezone { get; set; }
    public string? DomainName { get; set; } = string.Empty;
    public string? IPAddress { get; set; } = string.Empty;
    public string? HostCPID { get; set; } = string.Empty;
    public int ProcessorNumCPUs { get; set; }
    public string? ProcessorVendor { get; set; } = string.Empty;
    public string? ProcessorModel { get; set; } = string.Empty;
    // p_features, p_fpops, p_iops, p_membw, p_calculated, p_vm_extensions_disabled, m_nbytes, m_cache, m_swap, d_total, d_free
    public string? OSName { get; set; } = string.Empty;
    public string? OSVersion { get; set; } = string.Empty;
    // n_usable_coprocs, wsl_available, wsl (obj), coprocs (obj)
    public string? PlatformName { get; set; } = string.Empty;
    public string? ClientVersion { get; set; } = string.Empty;

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
