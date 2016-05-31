#region USING DIRECTIVES

using nets.dataclass;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public static class ExceptionHandler
    {
        #region MAIN METHODS

        /// <summary>
        /// Handler for exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logger"></param>
        /// <param name="errorOccur"></param>
        public static void HandleException(string message, ref Logger logger, ref bool errorOccur)
        {
            logger.Log(LogType.ERROR, message);
            errorOccur = true;
        }

        #endregion
    }
}
