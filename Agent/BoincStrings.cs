namespace BoincWatcher.Agent;

public class BoincStrings {
    public static string[] ResultClientState = {
        "new",
        "downloading",
        "downloaded",
        "compute error",
        "uploading",
        "uploaded",
        "aborted",
        "upload failed"
    };

    public static string[] ResultSchedulerState = {
        "uninitialized",
        "preempted",
        "scheduled"
    };

    public static string[] ResultActiveTaskState = {
        "UNINITIALIZED",
        "EXECUTING",
        "SUSPENDED",
        "ABORT_PENDING",
        "EXITED",
        "WAS_SIGNALED",
        "EXIT_UNKNOWN",
        "ABORTED",
        "COULDNT_START",
        "QUIT_PENDING",
        "COPY_PENDING"
    };
}
