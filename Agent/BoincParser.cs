using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

using BoincWatcher.Domain.Models;
using BoincWatcher.Domain.Utils;

namespace BoincWatcher.Agent;

public class BoincParser {
    public static ClientState ParseClientState(XElement clientState) {
        /*
        public HostInfo hostInfo;
        public NetStats netStats;
        public TimeStats timeStats;
        public Project[] projects;
        public App[] apps;
        public AppVersion[] appVersions;
        public WorkUnit[] workUnits;
        public GlobalPreferences globalPreferences;
        */

        ClientState state = new ClientState {
            hostInfo = ParseStateHostInfo(clientState),
            netStats = ParseStateNetStats(clientState),
            timeStats = ParseStateTimeStats(clientState),
            projects = ParseProjects(clientState),
            apps = ParseApps(clientState),
            appVersions = ParseAppVersions(clientState),
            workUnits = ParseWorkUnits(clientState),
            globalPreferences = ParseGlobalPreferences(clientState)
        };

        return state;
    }

    public static HostInfo ParseStateHostInfo(XElement clientState) {
        XElement hostInfo = clientState.Element("host_info");

        if (hostInfo != null) {
            HostInfo parsedInfo = new HostInfo {
                Timezone = Convert.ToInt32(hostInfo.Element("timezone")?.Value),
                DomainName = hostInfo.Element("domain_name")?.Value,
                IPAddress = hostInfo.Element("ip_addr")?.Value,
                HostCPID = hostInfo.Element("host_cpid")?.Value,
                ProcessorNumCPUs = Convert.ToInt32(hostInfo.Element("p_ncpus")?.Value),
                ProcessorVendor = hostInfo.Element("p_vendor")?.Value,
                ProcessorModel = hostInfo.Element("p_model")?.Value,
                PlatformName = clientState.Element("platform_name")?.Value,
                OSName = hostInfo.Element("os_name")?.Value,
                OSVersion = hostInfo.Element("os_version")?.Value,
                ClientVersion = $"{clientState.Element("core_client_major_version")?.Value ?? "x"}.{clientState.Element("core_client_minor_version")?.Value ?? "x"}.{clientState.Element("core_client_release")?.Value ?? "x"}"
            };

            return parsedInfo;
        } else {
            throw new Exception("No host_info found");
        }
    }

    public static NetStats ParseStateNetStats(XElement clientState) {
        XElement netStats = clientState.Element("net_stats");

        if (netStats != null) {
            NetStats parsedNetStats = new NetStats {
                BandwidthUp = ConvertUtils.ParseFloat(netStats.Element("bwup")?.Value),
                BandwidthDown = ConvertUtils.ParseFloat(netStats.Element("bwdown")?.Value),
                AverageUp = ConvertUtils.ParseFloat(netStats.Element("avg_up")?.Value),
                AverageDown = ConvertUtils.ParseFloat(netStats.Element("avg_down")?.Value),
                AverageTimeUp = ConvertUtils.ParseFloat(netStats.Element("avg_time_up")?.Value),
                AverageTimeDown = ConvertUtils.ParseFloat(netStats.Element("avg_time_down")?.Value)
            };

            return parsedNetStats;
        } else {
            throw new Exception("No net_stats found");
        }
    }

    public static TimeStats ParseStateTimeStats(XElement clientState) {
        XElement timeStats = clientState.Element("time_stats");

        if (timeStats != null) {
            TimeStats parsedTimeStats = new TimeStats {
                UptimePercentage = ConvertUtils.ParseFloat(timeStats.Element("on_frac")?.Value),
                ConnectedPercentage = ConvertUtils.ParseFloat(timeStats.Element("connected_frac")?.Value),
                CPUAndNetworkAvailablePercentage = ConvertUtils.ParseFloat(timeStats.Element("cpu_and_network_available_frac")?.Value),
                ActivePercentage = ConvertUtils.ParseFloat(timeStats.Element("active_frac")?.Value),
                GPUActivePercentage = ConvertUtils.ParseFloat(timeStats.Element("gpu_active_frac")?.Value),
                ClientStartTime = AppUtils.ParseDateFloat(ConvertUtils.ParseFloat(timeStats.Element("client_start_time")?.Value)),
                TotalStartTime = AppUtils.ParseDateFloat(ConvertUtils.ParseFloat(timeStats.Element("total_start_time")?.Value)),
                TotalDuration = ConvertUtils.ParseFloat(timeStats.Element("total_duration")?.Value),
                TotalActiveDuration = ConvertUtils.ParseFloat(timeStats.Element("total_active_duration")?.Value),
                TotalGPUActiveDuration = ConvertUtils.ParseFloat(timeStats.Element("total_gpu_active_duration")?.Value),
                Now = AppUtils.ParseDateFloat(ConvertUtils.ParseFloat(timeStats.Element("now")?.Value)),
                PreviousUptime = ConvertUtils.ParseFloat(timeStats.Element("previous_uptime")?.Value),
                SessionActiveDuration = ConvertUtils.ParseFloat(timeStats.Element("session_active_duration")?.Value),
                SessionGPUActiveDuration = ConvertUtils.ParseFloat(timeStats.Element("session_gpu_active_duration")?.Value)
            };

            return parsedTimeStats;
        } else {
            throw new Exception("No time_stats found");
        }
    }

    public static Project[] ParseProjects(XElement clientState) {
        IEnumerable<XElement> projects = clientState.Elements("project");

        Project[] parsedProjects = new Project[projects.Count()];

        int idx = 0;
        foreach (XElement project in projects) {
            Project parsedProject = new Project {
                MasterUrl = project.Element("master_url")?.Value,
                ProjectName = project.Element("project_name")?.Value,
                ProjectDirectory = project.Element("project_dir")?.Value,
                UserId = Convert.ToInt32(project.Element("userid")?.Value),
                UserName = project.Element("user_name")?.Value,
                UserTotalCredit = ConvertUtils.ParseFloat(project.Element("user_total_credit")?.Value),
                UserExpectedAverageCredit = ConvertUtils.ParseFloat(project.Element("user_expavg_credit")?.Value),
                UserCreateTime = (int)ConvertUtils.ParseFloat(project.Element("user_create_time")?.Value),
                TeamId = Convert.ToInt32(project.Element("teamid")?.Value),
                TeamName = project.Element("team_name")?.Value,
                HostId = Convert.ToInt32(project.Element("hostid")?.Value),
                HostVenue = project.Element("host_venue")?.Value,
                HostTotalCredit = ConvertUtils.ParseFloat(project.Element("host_total_credit")?.Value),
                HostExpectedAverageCredit = ConvertUtils.ParseFloat(project.Element("host_expavg_credit")?.Value),
                HostCreateTime = (int)ConvertUtils.ParseFloat(project.Element("host_create_time")?.Value),
                CrossProjectId = project.Element("cross_project_id")?.Value,
                ExternalCrossProjectId = project.Element("external_cpid")?.Value,
                DontRequestMoreWork = project.Element("dont_request_more_work") != null,
                DurationCorrectionFactor = ConvertUtils.ParseFloat(project.Element("duration_correction_factor")?.Value),
                NumJobsSuccess = Convert.ToInt32(project.Element("njobs_success")?.Value),
                NumJobsError = Convert.ToInt32(project.Element("njobs_error")?.Value)
            };

            parsedProjects[idx] = parsedProject;
            idx++;
        }

        return parsedProjects;
    }

    public static App[] ParseApps(XElement clientState) {
        IEnumerable<XElement> apps = clientState.Elements("app");

        App[] parsedApps = new App[apps.Count()];

        int idx = 0;
        foreach (XElement app in apps) {
            App parsedApp = new App {
                Name = app.Element("name")?.Value,
                UserFriendlyName = app.Element("user_friendly_name")?.Value,
                NonCPUIntensive = app.Element("non_cpu_intensive")?.Value == "1"
            };

            parsedApps[idx] = parsedApp;
            idx++;
        }

        return parsedApps;
    }

    public static AppVersion[] ParseAppVersions(XElement clientState) {
        IEnumerable<XElement> appVersions = clientState.Elements("app_version");

        AppVersion[] parsedAppVersions = new AppVersion[appVersions.Count()];

        int idx = 0;
        foreach (XElement appVersion in appVersions) {
            AppVersion parsedAppVersion = new AppVersion {
                AppName = appVersion.Element("app_name")?.Value,
                Version = Convert.ToInt32(appVersion.Element("version_num")?.Value),
                Platform = appVersion.Element("Platform")?.Value,
                AverageNumberCPUs = ConvertUtils.ParseFloat(appVersion.Element("avg_ncpus")?.Value),
                Flops = ConvertUtils.ParseFloat(appVersion.Element("flops")?.Value),
                PlanClass = appVersion.Element("plan_class")?.Value,
                CmdLine = appVersion.Element("cmdline")?.Value,
                FileRefs = ParseFileRefs(appVersion),
                CoProcessors = ParseCoProcessors(appVersion)
            };

            parsedAppVersions[idx] = parsedAppVersion;
            idx++;
        }

        return parsedAppVersions;
    }

    private static FileRef[] ParseFileRefs(XElement xelement) {
        IEnumerable<XElement> fileRefs = xelement.Elements("file_ref");

        FileRef[] parsedFileRefs = new FileRef[fileRefs.Count()];

        int idx = 0;
        foreach (XElement fileRef in fileRefs) {
            FileRef parsedFileRef = new FileRef {
                FileName = fileRef.Element("file_name")?.Value,
                OpenName = fileRef.Element("open_name")?.Value,
                MainProgram = fileRef.Element("main_program") != null,
                CopyFile = fileRef.Element("copy_file") != null
            };

            parsedFileRefs[idx] = parsedFileRef;
            idx++;
        }

        return parsedFileRefs;
    }

    private static CoProcessor[] ParseCoProcessors(XElement xelement) {
        IEnumerable<XElement> coProcessors = xelement.Elements("coproc");

        CoProcessor[] parsedCoProcessors = new CoProcessor[coProcessors.Count()];

        int idx = 0;
        foreach (XElement coProcessor in coProcessors) {
            CoProcessor parsedCoProcessor = new CoProcessor {
                Type = coProcessor.Element("type")?.Value,
                Count = ConvertUtils.ParseFloat(coProcessor.Element("count")?.Value),
            };

            parsedCoProcessors[idx] = parsedCoProcessor;
            idx++;
        }

        return parsedCoProcessors;
    }

    public static WorkUnit[] ParseWorkUnits(XElement clientState) {
        IEnumerable<XElement> workunits = clientState.Elements("workunit");

        WorkUnit[] parsedWorkunits = new WorkUnit[workunits.Count()];

        int idx = 0;
        foreach (XElement wu in workunits) {
            WorkUnit parsedWu = new WorkUnit {
                Name = wu.Element("name")?.Value,
                AppName = wu.Element("app_name")?.Value,
                AppVersionNumber = Convert.ToInt32(wu.Element("version_num")?.Value),
                ResourceFPOpsEstimate = ConvertUtils.ParseFloat(wu.Element("rsc_fpops_est")?.Value),
                ResourceFPOpsBound = ConvertUtils.ParseFloat(wu.Element("rsc_fpops_bound")?.Value),
                ResourceMemoryBound = ConvertUtils.ParseFloat(wu.Element("rsc_memory_bound")?.Value),
                ResourceDiskBound = ConvertUtils.ParseFloat(wu.Element("rsc_disk_bound")?.Value),
                CmdLine = wu.Element("command_line")?.Value?.Trim(),
                FileRefs = ParseFileRefs(wu)
            };

            parsedWorkunits[idx] = parsedWu;
            idx++;
        }

        return parsedWorkunits;
    }

    public static GlobalPreferences ParseGlobalPreferences(XElement clientState) {
        XElement globalPreferences = clientState.Element("global_preferences");

        if (globalPreferences != null) {
            GlobalPreferences parsedGlobalPreferences = new GlobalPreferences {
                ModTime = ConvertUtils.ParseFloat(globalPreferences.Element("mod_time")?.Value),
                BatteryChargeMinimumPercentage = ConvertUtils.ParseFloat(globalPreferences.Element("battery_charge_min_pct")?.Value),
                BatteryMaxTemperature = ConvertUtils.ParseFloat(globalPreferences.Element("battery_max_temperature")?.Value),
                RunOnBatteries = globalPreferences.Element("run_on_batteries")?.Value == "1",
                RunIfUserActive = globalPreferences.Element("run_if_user_active")?.Value == "1",
                SuspendIfNoRecentInputInLastNSeconds = ConvertUtils.ParseFloat(globalPreferences.Element("suspend_if_no_recent_input")?.Value),
                SuspendCPUUsage = ConvertUtils.ParseFloat(globalPreferences.Element("suspend_cpu_usage")?.Value),
                StartHour = ConvertUtils.ParseFloat(globalPreferences.Element("start_hour")?.Value),
                EndHour = ConvertUtils.ParseFloat(globalPreferences.Element("end_hour")?.Value),
                NetStartHour = ConvertUtils.ParseFloat(globalPreferences.Element("net_start_hour")?.Value),
                NetEndHour = ConvertUtils.ParseFloat(globalPreferences.Element("net_end_hour")?.Value),
                LeaveAppsInMemory = globalPreferences.Element("leave_apps_in_memory")?.Value == "1",
                ConfirmBeforeConnecting = globalPreferences.Element("confirm_before_connecting")?.Value == "1",
                HangupIfDialed = globalPreferences.Element("hangup_if_dialed")?.Value == "1",
                DontVerifyImages = globalPreferences.Element("dont_verify_images")?.Value == "1",
                WorkBufferMinimumDays = ConvertUtils.ParseFloat(globalPreferences.Element("work_buf_min_days")?.Value),
                WorkBufferAdditionalDays = ConvertUtils.ParseFloat(globalPreferences.Element("work_buf_additional_days")?.Value),
                MaxNumberCPUsPercentage = ConvertUtils.ParseFloat(globalPreferences.Element("max_ncpus_pct")?.Value),
                CPUSchedulingPeriodMinutes = ConvertUtils.ParseFloat(globalPreferences.Element("cpu_scheduling_period_minutes")?.Value),
                DiskInterval = ConvertUtils.ParseFloat(globalPreferences.Element("disk_interval")?.Value),
                DiskMaxUsedGB = ConvertUtils.ParseFloat(globalPreferences.Element("disk_max_used_gb")?.Value),
                DiskMaxUsedPercentage = ConvertUtils.ParseFloat(globalPreferences.Element("disk_max_used_pct")?.Value),
                DiskMinFreeGB = ConvertUtils.ParseFloat(globalPreferences.Element("disk_min_free_gb")?.Value),
                VMMaxUsedPercentage = ConvertUtils.ParseFloat(globalPreferences.Element("vm_max_used_pct")?.Value),
                RamMaxUsedBusyPercentage = ConvertUtils.ParseFloat(globalPreferences.Element("ram_max_used_busy_pct")?.Value),
                RamMaxUsedIdlePercentage = ConvertUtils.ParseFloat(globalPreferences.Element("ram_max_used_idle_pct")?.Value),
                IdleTimeToRun = ConvertUtils.ParseFloat(globalPreferences.Element("idle_time_to_run")?.Value),
                MaxBytesUp = ConvertUtils.ParseFloat(globalPreferences.Element("max_bytes_sec_up")?.Value),
                MaxBytesDown = ConvertUtils.ParseFloat(globalPreferences.Element("max_bytes_sec_down")?.Value),
                CPUUsageLimit = ConvertUtils.ParseFloat(globalPreferences.Element("cpu_usage_limit")?.Value),
                DailyTransferLimitMB = ConvertUtils.ParseFloat(globalPreferences.Element("daily_xfer_limit_mb")?.Value),
                DailyTransferPeriodDays = Convert.ToInt32(globalPreferences.Element("daily_xfer_period_days")?.Value),
                OverrideFilePresent = globalPreferences.Element("override_file_present")?.Value == "1",
                NetworkWifiOnly = globalPreferences.Element("network_wifi_only")?.Value == "1"
            };

            return parsedGlobalPreferences;
        } else {
            throw new Exception("No time_stats found");
        }
    }
}
