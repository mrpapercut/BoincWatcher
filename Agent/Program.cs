using System.Threading.Tasks;

namespace BoincWatcher.Agent;

class Program {
    public static Task Main(string[] args) 
        => Startup.RunAsync(args);
}
