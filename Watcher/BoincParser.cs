using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

using BoincManager.BoincStateModels;

namespace BoincManager.Watcher;

public class BoincParser {
    public static ClientState ParseClientState(XElement clientState) {
        ClientState state = new ClientState {
            hostInfo = ParseStateHostInfo(clientState),
            projects = ParseProjects(clientState),
            apps = ParseApps(clientState)
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

    public static void ParseStateNetStats(XElement clientState) { }

    public static void ParseStateTimeStats(XElement clientState) { }

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
                UserTotalCredit = float.Parse(project.Element("user_total_credit")?.Value ?? "0"),
                UserExpectedAverageCredit = float.Parse(project.Element("user_expavg_credit")?.Value ?? "0"),
                UserCreateTime = (int)float.Parse(project.Element("user_create_time")?.Value ?? "0"),
                TeamId = Convert.ToInt32(project.Element("teamid")?.Value),
                TeamName = project.Element("team_name")?.Value,
                HostId = Convert.ToInt32(project.Element("hostid")?.Value),
                HostVenue = project.Element("host_venue")?.Value,
                HostTotalCredit = float.Parse(project.Element("host_total_credit")?.Value ?? "0"),
                HostExpectedAverageCredit = float.Parse(project.Element("host_expavg_credit")?.Value ?? "0"),
                HostCreateTime = (int)float.Parse(project.Element("host_create_time")?.Value ?? "0"),
                CrossProjectId = project.Element("cross_project_id")?.Value,
                ExternalCrossProjectId = project.Element("external_cpid")?.Value,
                DontRequestMoreWork = project.Element("dont_request_more_work") != null,
                DurationCorrectionFactor = float.Parse(project.Element("duration_correction_factor")?.Value ?? "0"),
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
}
