using System;
using System.Text.Json;

namespace BoincWatcher.Domain.Models; 
public class GlobalPreferences {
    public float ModTime { get; set; }
    public float BatteryChargeMinimumPercentage { get; set; }
    public float BatteryMaxTemperature { get; set; }
    public bool RunOnBatteries { get; set; }
    public bool RunIfUserActive { get; set; }
    public bool RunGPUIfUserActive { get; set; }
    public float SuspendIfNoRecentInputInLastNSeconds { get; set; }
    public float SuspendCPUUsage { get; set; }
    public float StartHour { get; set; }
    public float EndHour { get; set; }
    public float NetStartHour { get; set; }
    public float NetEndHour { get; set; }
    public bool LeaveAppsInMemory { get; set; }
    public bool ConfirmBeforeConnecting { get; set; }
    public bool HangupIfDialed { get; set; }
    public bool DontVerifyImages { get; set; }
    public float WorkBufferMinimumDays { get; set; }
    public float WorkBufferAdditionalDays { get; set; }
    public float MaxNumberCPUsPercentage { get; set; }
    public float CPUSchedulingPeriodMinutes { get; set; }
    public float DiskInterval { get; set; }
    public float DiskMaxUsedGB { get; set; }
    public float DiskMaxUsedPercentage { get; set; }
    public float DiskMinFreeGB { get; set; }
    public float VMMaxUsedPercentage { get; set; }
    public float RamMaxUsedBusyPercentage { get; set; }
    public float RamMaxUsedIdlePercentage { get; set; }
    public float IdleTimeToRun { get; set; }
    public float MaxBytesUp { get; set; }
    public float MaxBytesDown { get; set; }
    public float CPUUsageLimit { get; set; }
    public float DailyTransferLimitMB { get; set; }
    public int DailyTransferPeriodDays { get; set; }
    public bool OverrideFilePresent { get; set; }
    public bool NetworkWifiOnly { get; set; }

    public string ToJSON() {
        string jsonStr = JsonSerializer.Serialize(this);

        return jsonStr;
    }
}
