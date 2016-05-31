using System.IO;

namespace nets_wpf.DataStructures
{
    /// <summary>
    /// Represent the accepted and ignored patterns specified by the user
    /// </summary>
    public class FilterPattern
    {
        private string[] includeList;
        private string[] excludeList;
        private const char Separator = ',';
        
        public FilterPattern(string includePattern, string excludePattern)
        {
            this.IncludePattern = includePattern;
            InitializeIncludeList();

            this.ExcludePattern = excludePattern;
            InitializeExcludeList();
        }

        public string IncludePattern { get; private set; }
        public string ExcludePattern { get; private set; }

        /// <summary>
        /// check to reject a file  or not
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsExcluded(string filePath)
        {
            //check excludeList first
            foreach (string excludeFile in excludeList)
                if (ExpressionMatcher.IsMatch(Path.GetFileName(filePath), excludeFile))
                    return true;
                
            //check includePattern later
            foreach (string includeFile in includeList)
                if (ExpressionMatcher.IsMatch(Path.GetFileName(filePath), includeFile))
                    return false;

            return includeList.Length != 0;
        }
        
        private void InitializeExcludeList()
        {
            if (ExcludePattern == null || ExcludePattern.Trim() == "")
                excludeList = new string[0];
            else
                excludeList = ExcludePattern.Split(Separator);
            
            for (int i = 0; i < excludeList.Length; i++)
                excludeList[i] = excludeList[i].Trim();
        }

        private void InitializeIncludeList()
        {
            if (IncludePattern == null || IncludePattern.Trim() == "")
                includeList = new string[0];
            else
                includeList = IncludePattern.Split(Separator);
            
            for (int i = 0; i < includeList.Length; i++)
                includeList[i] = includeList[i].Trim();
        }
         
        public override string ToString()
        {
            return IncludePattern + "|" + ExcludePattern;
        }
    }

    static class ExpressionMatcher
    {
        public static bool IsMatch(string str, string expr)
        {
            string[] patternList = expr.Split('*');
            int i = 0;
            for (; i < patternList.Length && str != ""; i++)
            {
                int index = str.IndexOf(patternList[i]);
                if (index == -1)
                    return false;
                str = str.Substring(index + patternList[i].Length);
            }
            return (i == patternList.Length);
        }
    }
}
