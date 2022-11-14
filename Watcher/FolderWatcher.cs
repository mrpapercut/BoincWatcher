using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoincManager.Watcher;
public class FolderWatcher {
    private AppConfig AppConfig;

    private BoincActions BoincActions;

    private FileSystemWatcher? watcher;

    public FolderWatcher(AppConfig appConfig, BoincActions boincActions) {
        this.AppConfig = appConfig;
        this.BoincActions = boincActions;
    }

    public void StartWatcher() {
        this.watcher = new FileSystemWatcher(AppConfig.PrimegridProjectFolder);

        this.watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        this.watcher.Changed += this.OnChanged;
        this.watcher.Created += this.OnCreated;

        this.watcher.EnableRaisingEvents = true;
        this.watcher.IncludeSubdirectories = true;

        Console.WriteLine($"Watching Primegrid on path {AppConfig.PrimegridProjectFolder}");
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
        if (e.Name == null
            || e.Name.StartsWith("slideshow")
            || e.Name.StartsWith("stat_icon")
            || e.Name.EndsWith("cert")
            || e.Name.StartsWith("platform_nvidia")
            || e.FullPath.EndsWith(".exe")) {
            return;
        }

        Console.WriteLine($"OnCreated {e.Name}");

        try {
            string filecontents = File.ReadAllText(e.FullPath);

            if (e.Name.StartsWith("genefer")) {
                this.BoincActions.HandleGenefer(sender, e, filecontents);
            } else {
                AppUtils.LogToFile($"Created: {e.FullPath} ({e.GetType()})");
                if (filecontents.Length < 500) {
                    AppUtils.LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                } else {
                    AppUtils.LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                }
            }
        } catch (IOException err) {
            // File locked by other process
            // Continue
            AppUtils.LogToFile($"IOException: {err.Message}", "errors.txt");
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
        if (e.ChangeType != WatcherChangeTypes.Changed) {
            return;
        }

        if (e.Name == null
            || e.Name.StartsWith("slideshow")
            || e.Name.StartsWith("stat_icon")
            || e.Name.StartsWith("platform_nvidia")
            || e.Name.EndsWith("cert")
            || e.FullPath.EndsWith(".exe")) {
            return;
        }

        Console.WriteLine($"OnChanged {e.Name}");

        try {
            string filecontents = File.ReadAllText(e.FullPath);

            if (e.Name.StartsWith("genefer")) {
                this.BoincActions.HandleGenefer(sender, e, filecontents);
            } else {
                AppUtils.LogToFile($"Changed: {e.FullPath} ({e.GetType()})");
                if (filecontents.Length < 500) {
                    AppUtils.LogToFile($"Contents of {e.Name}:\n\t {filecontents}");
                } else {
                    AppUtils.LogToFile($"Contents too large to show ({filecontents.Length} bytes)");
                }
            }
        } catch (IOException err) {
            // File locked by other process
            // Continue
            AppUtils.LogToFile($"IOException: {err.Message}", "errors.txt");
        }
    }
}
