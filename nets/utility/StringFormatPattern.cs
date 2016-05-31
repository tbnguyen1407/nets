#region USING DIRECTIVES

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace nets.utility
{
    /// <summary>
    /// Author: Vu An Hoa
    /// </summary>
    class StringFormatPattern
    {
        #region FIELD DECLARATION

        private string Pattern;
        private int NumberOfFields; // Number of data fields in the commandPattern.
        private readonly List<string> Barriers; // List of all strings between any two consecutive {x} and {y}.
        private readonly List<int> FieldId; // List of the sequence of field numbers

        #endregion

        #region CONSTRUCTORS

        public StringFormatPattern(string pattern)
        {
            Pattern = pattern;
            NumberOfFields = 0;
            Barriers = new List<string>();
            FieldId = new List<int>();
            Extract();
        }

        #endregion

        #region MAIN METHODS

        /// <summary>
        /// Match an input string against this commandPattern and extract the data.
        /// </summary>
        /// <param tag="input">Input string compatible with the commandPattern.</param>
        /// <returns>Array of data strings parsed from the input.</returns>
        public string[] MatchInput(string input)
        {
            input += "$";
            string[] data = new string[NumberOfFields];

            int p2 = 0;

            // Getting the correct information at place
            for (int i = 0; i < Barriers.Count - 1; i++)
            {
                int p1 = input.IndexOf(Barriers.ElementAt(i), p2) + Barriers.ElementAt(i).Length;
                p2 = input.IndexOf(Barriers.ElementAt(i+1), p1 + 1);
                data[FieldId.ElementAt(i)] = input.Substring(p1, p2 - p1);
            }

            return data;
        }

        /// <summary>
        /// A version of SSCANF.
        /// Scan the input string and match it against the commandPattern.
        /// String at the specific locations are extracted.
        /// For instance, ("Hello An Hoa","Hello {0}",tag)
        /// -> tag = An Hoa.
        /// There is one requirement: between any two positioning
        /// format, say {0} and {1}, there should be a "non-trivial"
        /// commandPattern. In case one put "{0} {1}", the first token will
        /// be return in place of {0} and the second for {1}.
        /// </summary>
        /// <param tag="input">Input string with data.</param>
        /// <param tag="pattern">Input formatted commandPattern.</param>
        public static string[] ExtractStringData(string input, string pattern)
        {
            StringFormatPattern format = new StringFormatPattern(pattern);
            return format.MatchInput(input);
        }

        /// <summary>
        /// Find the occurences of a commandPattern within an input string.
        /// Use this method to find patterns and ExtractStringData 
        /// to get the data from the patterns.
        /// TODO: IMPLEMENT.
        /// </summary>
        /// <param tag="input"></param>
        /// <param tag="pattern"></param>
        /// <returns></returns>
        public string[] FindPattern(string input, string pattern)
        {
            List<string> occurences = new List<string>();
            return occurences.ToArray();
        }

        #endregion

        #region HELPERS

        /// <summary>
        /// Extract information from the _pattern.
        /// </summary>
        private void Extract()
        {
            // Adding end of string markers. So we do not have to consider the
            // special case when } is at the end of the commandPattern.
            Pattern += "$";

            // Extract the barriers and the number of fields from the commandPattern
            int k = 0, oldK = -1;    // temporary variables to store the index of the characters { and } in commandPattern.
            while (true)
            {
                int t = Pattern.IndexOf('{', k);    // temporary variables to store the index of the characters { and } in commandPattern.

                if (t == -1)
                    break;

                string bar = Pattern.Substring(oldK + 1, t - oldK - 1);
                Barriers.Add(bar);
                //Console.WriteLine("Barrier string:${0}$", bar);

                k = Pattern.IndexOf('}', t);
                var fieldNumber = int.Parse(Pattern.Substring(t + 1, k - t - 1)); // field number = number between {x}, {y}
                FieldId.Add(fieldNumber);
                NumberOfFields = Math.Max(fieldNumber, NumberOfFields);
                oldK = k;
            }

            NumberOfFields++;

            // Add the remaining string to the barriers.
            if (k < Pattern.Length - 1)
                Barriers.Add(Pattern.Substring(k + 1, Pattern.Length - k - 1));

            // Console.WriteLine("Number of fields: {0}", NumberOfFields);
            // We also need to remove the $ at the end of the commandPattern but this is unnecessary.
        }

        /// <summary>
        /// Find the first occurence of this commandPattern in the input string
        /// starting from startIndex.
        /// TO BE IMPLEMETED
        /// </summary>
        /// <param tag="input"></param>
        /// <param tag="startIndex"></param>
        /// <returns></returns>
        private string FindFirstOccurence(string input, int startIndex)
        {
            return null;
        }

        /// <summary>
        /// Find all occurences of this string patterns
        /// TO BE IMPLEMETED
        /// </summary>
        /// <param tag="input"></param>
        /// <returns></returns>
        private string[] FindAllOccurences(string input)
        {
            return null;
        }
        
        /// <summary>
        /// Parse data from input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string[][] ParseInputForData(string input)
        {
            return null;
        }

        #endregion
    }
}
