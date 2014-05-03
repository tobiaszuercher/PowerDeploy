namespace PowerDeploy.Server.ServiceModel
{
    public class DeployLog
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
    }

    public enum LogLevel
    {
        Fatal,
        Error,
        Warning,
        Info,
        Debug,
    }
}