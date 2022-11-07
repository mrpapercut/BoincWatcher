using Microsoft.Win32;
using Microsoft.Extensions.Configuration;

namespace BoincWatcher
{
    internal class AppConfig
    {
        public string BoincDataFolder { get; protected set; } // Contains projects
        public string BoincInstallFolder { get; protected set; } // Contains boinccmd.exe
        public string PrimegridProjectFolder { get; protected set; }
        public string BoincCmdPath { get; protected set; }

        public static string BoincDataFolderRegKey = "DATADIR";
        public static string BoincInstallFolderRegKey = "INSTALLDIR";

        public void LoadConfig()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("settings.json", optional: false);

            IConfiguration config = builder.Build();

            IConfigurationSection boincSection = config.GetSection("Boinc");
            
            BoincDataFolder = boincSection.GetValue<string>("DataFolder");
            if (BoincDataFolder == null)
            {
                BoincDataFolder = ReadBoincValueFromRegistry(BoincDataFolderRegKey);

                if (BoincDataFolder == null)
                {
                    throw new Exception("Error: Could not find Boinc Projects folder");
                }
            }

            BoincInstallFolder = boincSection.GetValue<string>("InstallFolder");
            if (BoincInstallFolder == null)
            {
                BoincInstallFolder = ReadBoincValueFromRegistry(BoincInstallFolderRegKey);

                if (BoincInstallFolder == null)
                {
                    throw new Exception("Error: Could not find Boinc Install folder");
                }
            }

            PrimegridProjectFolder = BoincDataFolder + @"\Projects\www.primegrid.com";
            if (!Directory.Exists(PrimegridProjectFolder))
            {
                throw new Exception($"Could not find {PrimegridProjectFolder}");
            }

            BoincCmdPath = BoincInstallFolder + @"\boinccmd.exe";
            if (!File.Exists(BoincCmdPath))
            {
                throw new Exception($"Could not find {BoincCmdPath}");
            }
        }

        private string ReadBoincValueFromRegistry(string key)
        {
            string regVal = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Space Sciences Laboratory, U.C. Berkeley\\BOINC Setup", key, string.Empty);

            return regVal;
        }
    }
}
