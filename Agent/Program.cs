using System.Threading.Tasks;

namespace BoincManager.Watcher;

class Program {
    public static Task Main(string[] args) 
        => Startup.RunAsync(args);
}
