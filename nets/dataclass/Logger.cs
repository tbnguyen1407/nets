using System.Text;

namespace nets.dataclass
{
    /// <summary>
    /// Author: Nguyen Hoang Hai
    /// </summary>
    public class Logger
    {
        private StringBuilder LogInfo;
        private readonly string LoggerName;

        public Logger(string loggerName)
        {
            LoggerName = loggerName;
            LogInfo = new StringBuilder();
            LogInfo.Append(LoggerName + "\n\n");
        }

        public string GetName()
        {
            return LoggerName;
        }

        public string GetInfo()
        {
            return LogInfo.ToString();
        }

        public void LogSyncJob(string srcFolder, string desFolder)
        {
            LogInfo = new StringBuilder();
            LogInfo.Append("*******************************************************************\n");
            LogInfo.Append("Src: " + srcFolder + "\n");
            LogInfo.Append("Des: " + desFolder + "\n");
            LogInfo.Append("*******************************************************************\n\n");
        }

        public void Log(LogType logType, string message)
        {
            LogInfo.Append("Type: " + logType + "\n");
            LogInfo.Append("Msg: " + message + "\n\n");
        }

        public void EndLog()
        {
            LogInfo.Append("*******************************************************************\n\n");
        }
    }
}