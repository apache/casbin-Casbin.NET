using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Casbin.Util
{
    internal static class StringUtil
    {
        private static readonly Regex s_evalRegex = new(@"\beval\((?<rule>[^)]*)\)");
        private static readonly Regex s_inOperatorRegex = new(@"([a-zA-Z0-9_.()""]+) in ([a-zA-Z0-9_.()""]+)");
        private static readonly Regex s_inListOperatorRegex = new(@"([a-zA-Z0-9_.]+)\s+in\s+(?:\(((?:""(?:[^""\\]|\\.)*""|[^()""])*)\)|\[((?:""(?:[^""\\]|\\.)*""|[^\[\]""])*)\])");

        /// <summary>
        /// Removes the comments starting with # in the text.
        /// </summary>
        /// <param name="line">a line in the model.</param>
        /// <returns>The line without comments.</returns>
        internal static string RemoveComments(this string line)
        {
            int pos = line.IndexOf("#", StringComparison.Ordinal);
            if (pos is -1)
            {
                return line;
            }
            return line.Substring(0, pos).Trim();
        }

        /// <summary>
        /// Determines whether matcher contains eval function
        /// </summary>
        /// <param name="expressString"></param>
        /// <returns></returns>
        internal static bool HasEval(string expressString)
        {
            return s_evalRegex.IsMatch(expressString);
        }

        /// <summary>
        /// Tries get rule names of eval function
        /// </summary>
        /// <param name="expressString"></param>
        /// <param name="evalRuleNames"></param>
        /// <returns></returns>
        internal static bool TryGetEvalRuleNames(string expressString, out IEnumerable<string> evalRuleNames)
        {
            MatchCollection matches = s_evalRegex.Matches(expressString);
            int matchCount = matches.Count;
            if (matchCount is 0)
            {
                evalRuleNames = null;
                return false;
            }
            string[] rules = new string[matchCount];
            for (int i = 0; i < matchCount; i++)
            {
                GroupCollection group = matches[i].Groups;
                if (group.Count < 2)
                {
                    evalRuleNames = null;
                    return false;
                }
                rules[i] = group[1].Value;
            }
            evalRuleNames = rules;
            return true;
        }

        /// <summary>
        /// Replace eval function with the value of its eval rule
        /// </summary>
        /// <param name="expressStringWithEvalRule"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static string ReplaceEval(string expressStringWithEvalRule, IDictionary<string, string> rules)
        {
            if (rules is null)
            {
                return expressStringWithEvalRule;
            }

            return s_evalRegex.Replace(expressStringWithEvalRule, match =>
            {
                GroupCollection matchGroups = match.Groups;
                int subMatchCount = matchGroups.Count - 1;
                if (subMatchCount is 0 || rules.TryGetValue(
                    matchGroups[1].Value, out string rule) is false)
                {
                    return match.Value;
                }
                return "(" + rule + ")";
            });
        }

        /// <summary>
        /// Replace in operator with function "Contains".
        /// </summary>
        /// <param name="expressString"></param>
        /// <returns></returns>
        internal static string ReplaceInOperator(string expressString)
        {
            var res = s_inOperatorRegex.Replace(expressString, match =>
            {
                GroupCollection matchGroups = match.Groups;
                int subMatchCount = matchGroups.Count - 1;
                if(subMatchCount == 2)
                {
                    return matchGroups[2].Value + ".Contains(" + matchGroups[1].Value + ")";
                }
                return matchGroups[0].Value;
            });
            return res;
        }

        /// <summary>
        /// Replace in operator with a literal list, such as "a in ('b', 'c')", with "(a == 'b' || a == 'c')".
        /// </summary>
        /// <param name="expressString"></param>
        /// <returns></returns>
        internal static string ReplaceInListOperator(string expressString)
        {
            return s_inListOperatorRegex.Replace(expressString, match =>
            {
                string item = match.Groups[1].Value;
                string list = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;
                List<string> values = SplitListValues(list);
                if (values.Count < 2 && (values.Count is 0 || values[0].StartsWith("\"") is false))
                {
                    return match.Value;
                }

                return "(" + string.Join(" || ", values.Select(value => item + " == " + value)) + ")";
            });
        }

        private static List<string> SplitListValues(string list)
        {
            List<string> values = new();
            int start = 0;
            bool inQuotes = false;
            for (int i = 0; i < list.Length; i++)
            {
                switch (list[i])
                {
                    case '\\' when inQuotes:
                        i++;
                        break;
                    case '"':
                        inQuotes = !inQuotes;
                        break;
                    case ',' when inQuotes is false:
                        AddValue(list.Substring(start, i - start));
                        start = i + 1;
                        break;
                }
            }

            AddValue(list.Substring(start));
            return values;

            void AddValue(string value)
            {
                value = value.Trim();
                if (value.Length is not 0)
                {
                    values.Add(value);
                }
            }
        }
    }
}
