using System.Text.Json;

using BoincManager.Server.Objects;

namespace BoincManager.Server;
public class Program {
    static DBConnection dbConn = new();
    static void Main(string[] args) {
        HttpServer server = new HttpServer();
        server.Start();

        dbConn.OpenConnection();

        List<BoincTask> tasks = dbConn.GetTasks();

        tasks.ForEach(t => {
            string taskJsonStr = JsonSerializer.Serialize(t);

            Console.WriteLine(taskJsonStr);
        });

        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }
}
