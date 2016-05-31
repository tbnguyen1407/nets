using nets_wpf.DataStructures;

namespace nets_wpf.Utility
{
    public static class ExceptionHandler
    {
        public static void HandleException(string message, ref Logger logger, ref bool errorOccur)
        {
            logger.Log(LogType.ERROR, message);
            errorOccur = true;
        }
    }
}
