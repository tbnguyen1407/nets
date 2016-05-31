#region USING DIRECTIVES

using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace nets.dataclass
{
    /// <summary>
    ///   Represent the accepted and ignored patterns specified by the user
    ///   Author: Hoang Nguyen Nhat Tao
    /// </summary>
    public class FilterPattern
    {
        private const char Separator = ',';
        private List<string> excludeList;
        private List<string> includeList;

        public FilterPattern(string includePattern, string excludePattern)
        {
            IncludePattern = includePattern;
            InitializeIncludeList();

            ExcludePattern = excludePattern;
            InitializeExcludeList();
        }

        public string IncludePattern { get; private set; }
        public string ExcludePattern { get; private set; }

        /// <summary>
        ///   check to reject a file  or not
        /// </summary>
        /// <param name = "filePath"></param>
        /// <returns></returns>
        public bool IsExcluded(string filePath)
        {
            //check excludeList first
            if (excludeList.Any(excludeFile => ExpressionMatcher.IsMatch(Path.GetFileName(filePath), excludeFile)))
                return true;

            //check includePattern later
            if (includeList.Any(includeFile => ExpressionMatcher.IsMatch(Path.GetFileName(filePath), includeFile)))
                return false;

            return includeList.Count != 0;
        }

        private void InitializeExcludeList()
        {
            excludeList = new List<string>();
            if (ExcludePattern == null || ExcludePattern.Trim() == "")
                return;

            string[] excludeArray = ExcludePattern.Split(Separator);

            for (int i = 0; i < excludeArray.Length; i++)
                if (excludeArray[i].Trim() != "")
                    excludeList.Add(excludeArray[i].Trim());
        }

        private void InitializeIncludeList()
        {
            includeList = new List<string>();
            if (IncludePattern == null || IncludePattern.Trim() == "")
                return;

            string[] includeArray = IncludePattern.Split(Separator);

            for (int i = 0; i < includeArray.Length; i++)
                if (includeArray[i].Trim() != "")
                    includeList.Add(includeArray[i].Trim());
        }

        public override string ToString()
        {
            return IncludePattern + "|" + ExcludePattern;
        }
    }

    internal static class ExpressionMatcher
    {
        public static bool IsMatch(string str, string expr)
        {
            str = str.ToLower();
            expr = expr.ToLower();

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