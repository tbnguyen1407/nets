using System;
using System.Collections.Generic;
using System.Linq;

namespace nets_wpf.Utility
{
    class StringFormatPattern
    {
        string _pattern;

        // Number of data fields in the commandPattern.
        public int NumberOfFields;

        // List of all strings between any two consecutive {x} and {y}.
        private readonly List<string> _barriers;

        // List of the sequence of field numbers
        private readonly List<int> _fieldId;

        public StringFormatPattern(string pattern)
        {
            _pattern = pattern;
            NumberOfFields = 0;
            _barriers = new List<string>();
            _fieldId = new List<int>();
            Extract();
        }

        /// <summary>
        /// Extract information from the _pattern.
        /// </summary>
        private void Extract()
        {
            // Adding end of string markers. So we do not have to consider the
            // special case when } is at the end of the commandPattern.
            _pattern += "$";

            // Extract the barriers and the number of fields from the commandPattern
            int k = 0, oldK = -1;    // temporary variables to store the index of the characters { and } in commandPattern.
            while (true)
            {
                int t = _pattern.IndexOf('{', k);    // temporary variables to store the index of the characters { and } in commandPattern.
                if (t != -1)
                {
                    string bar = _pattern.Substring(oldK + 1, t - oldK - 1);
                    _barriers.Add(bar);
                    //Console.WriteLine("Barrier string:${0}$", bar);

                    k = _pattern.IndexOf('}', t);
                    var fieldNumber = int.Parse(_pattern.Substring(t + 1, k - t - 1));            // field number = number between {x}, {y}
                    _fieldId.Add(fieldNumber);
                    NumberOfFields = Math.Max(fieldNumber, NumberOfFields);
                    oldK = k;
                }
                else break;
            }

            NumberOfFields++;
            
            // Add the remaining string to the barriers.
            if (k < _pattern.Length - 1)
            {
                _barriers.Add(_pattern.Substring(k + 1, _pattern.Length - k - 1));
            }
            
            // Console.WriteLine("Number of fields: {0}", NumberOfFields);
            // We also need to remove the $ at the end of the commandPattern but this is unnecessary.
        }

        /// <summary>
        /// Match an input string against this commandPattern and extract the data.
        /// </summary>
        /// <param tag="input">Input string compatible with the commandPattern.</param>
        /// <returns>Array of data strings parsed from the input.</returns>
        public string[] MatchInput(string input)
        {
            input += "$";
            var data = new string[NumberOfFields];

            int p2 = 0;

            // Getting the correct information at place
            for (var i = 0; i < _barriers.Count - 1; i++)
            {
                int p1 = input.IndexOf(_barriers.ElementAt(i), p2) + _barriers.ElementAt(i).Length;
                p2 = input.IndexOf(_barriers.ElementAt(i+1), p1 + 1);
                data[_fieldId.ElementAt(i)] = input.Substring(p1, p2 - p1);
            }

            return data;
        }

        /// <summary>
        /// Find the first occurence of this commandPattern in the input string
        /// starting from startIndex.
        /// TO BE IMPLEMETED
        /// </summary>
        /// <param tag="input"></param>
        /// <param tag="startIndex"></param>
        /// <returns></returns>
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private string FindFirstOccurence(string input, int startIndex)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
        {
            return null;
        }

        /// <summary>
        /// Find all occurences of this string patterns
        /// TO BE IMPLEMETED
        /// </summary>
        /// <param tag="input"></param>
        /// <returns></returns>
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private string[] FindAllOccurences(string input)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
        {
            return null;
        }

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
        private string[][] ParseInputForData(string input)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
        {
            return null;
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
            var format = new StringFormatPattern(pattern);
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
            var occurences = new List<string>();
            return occurences.ToArray();
        }

        // TEST PARAMS COMMAND
        // string[] A = StringFormatPattern.ExtractStringData("My tag is An Hoa", "My tag is {0}");
        // Console.WriteLine(A[0]);
    }
}
