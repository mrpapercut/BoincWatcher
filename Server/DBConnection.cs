using MySql.Data.MySqlClient;

using BoincManager.Server.Objects;

namespace BoincManager.Server;
public class DBConnection {
    public MySqlConnection GetConnection() {
        string cs = @"server=localhost;port=3307;userid=dcadmin;password=1h1_uqx8DJGP709x;database=dcmanager";

        MySqlConnection conn = new MySqlConnection(cs);

        conn.Open();

        return conn;
    }

    public List<BoincTask> GetTasks() {
        string sql = "SELECT * FROM tasks";

        MySqlConnection conn = this.GetConnection();

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
        rdr.Close();
        conn.Close();

        return tasks;
    }
    
    public int UpsertTask(BoincTask task) {
        string sql = "INSERT INTO tasks (" +
            "name, wu_name, received, report_deadline, project_id, state, active_task_state, scheduler_state," +
            "elapsed_time, remaining_time, final_elapsed_time, exitstatus, fraction_done, resources, suspended_via_gui, is_fast_dc" +
            ") VALUES (" +
            "@name, @wu_name, @received, @report_deadline, @project_id, @state, @active_task_state, @scheduler_state," +
            "@elapsed_time, @remaining_time, @final_elapsed_time, @exitstatus, @fraction_done, @resources, @suspended_via_gui, @is_fast_dc" +
            ") ON DUPLICATE KEY UPDATE " +
            "state = @state, active_task_state = @active_task_state, scheduler_state = @scheduler_state," +
            "elapsed_time = @elapsed_time, remaining_time = @remaining_time, final_elapsed_time = @final_elapsed_time," +
            "exitstatus = @exitstatus, fraction_done = @fraction_done, suspended_via_gui = @suspended_via_gui;";

        MySqlConnection conn = this.GetConnection();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        int projectId = 0;
        switch (task.ProjectURL) {
            case "https://dev.primegrid.com/":
                projectId = 2;
                break;

            case "http://www.primegrid.com/":
            case "https://www.primegrid.com/":
            default:
                projectId = 1;
                break;
        }

        cmd.Parameters.AddWithValue("@name", task.Name);
        cmd.Parameters.AddWithValue("@wu_name", task.WUName);
        cmd.Parameters.AddWithValue("@received", task.Received);
        cmd.Parameters.AddWithValue("@report_deadline", task.ReportDeadline);
        cmd.Parameters.AddWithValue("@project_id", projectId);
        cmd.Parameters.AddWithValue("@state", task.State);
        cmd.Parameters.AddWithValue("@active_task_state", task.ActiveTaskState);
        cmd.Parameters.AddWithValue("@scheduler_state", task.SchedulerState);
        cmd.Parameters.AddWithValue("@elapsed_time", task.ElapsedTime);
        cmd.Parameters.AddWithValue("@remaining_time", task.RemainingTime);
        cmd.Parameters.AddWithValue("@final_elapsed_time", task.FinalElapsedTime);
        cmd.Parameters.AddWithValue("@exitstatus", task.ExitStatus);
        cmd.Parameters.AddWithValue("@fraction_done", task.FractionDone);
        cmd.Parameters.AddWithValue("@resources", task.Resources);
        cmd.Parameters.AddWithValue("@suspended_via_gui", task.SuspendedViaGui);
        cmd.Parameters.AddWithValue("@is_fast_dc", task.IsFastDc);
        // cmd.Prepare();

        try {
            MySqlDataReader result = cmd.ExecuteReader();

            return result.RecordsAffected;
        } catch (Exception ex) {
            Console.WriteLine(task.ToJSON());

            throw ex;
        } finally {
            conn.Close();
        }
    }
}
