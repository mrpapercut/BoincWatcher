using MySql.Data.MySqlClient;

using BoincManager.Server.Objects;

namespace BoincManager.Server;
public class DBConnection {
    private static MySqlConnection conn;

    public void OpenConnection() {
        string cs = @"server=localhost;userid=dcadmin;password=qt4i8+zD*z3r9J67z6514M45;database=dcmanager";

        conn = new MySqlConnection(cs);

        conn.Open();
    }

    public List<BoincTask> GetTasks() {
        string sql = "SELECT * FROM tasks";

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        List<BoincTask> tasks = new();

        using MySqlDataReader rdr = cmd.ExecuteReader();
        while (rdr.Read()) {
            BoincTask task = new BoincTask {
                Name = rdr.GetString(1),
                WUName = rdr.GetString(2),
                Received = DateTime.Parse(rdr.GetString(3)),
                ReportDeadline = DateTime.Parse(rdr.GetString(4)),
                ProjectId = rdr.GetInt32(5),
                State = rdr.GetInt32(6),
                ActiveTaskState = rdr.GetInt32(7),
                SchedulerState = rdr.GetInt32(8),
                ElapsedTime = rdr.GetFloat(9),
                RemainingTime = rdr.GetFloat(10),
                FinalElapsedTime = rdr.GetFloat(11),
                ExitStatus = rdr.GetInt32(12),
                FractionDone = rdr.GetFloat(13),
                Resources = rdr.GetString(14),
                SuspendedViaGui = rdr.GetInt32(15) == 1,
                IsFastDc = rdr.GetInt32(16) == 1,
            };

            tasks.Add(task);
        }

        return tasks;
    }
}
