// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

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
        /// Replaces the matches of regex that are not inside a quoted string literal.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        internal static string ReplaceOutsideStringLiterals(string text, Regex regex, string replacement)
        {
            List<KeyValuePair<int, int>> literals = GetStringLiteralRanges(text);
            if (literals.Count is 0)
            {
                return regex.Replace(text, replacement);
            }

            return regex.Replace(text, match =>
                literals.Any(literal => match.Index >= literal.Key && match.Index < literal.Value)
                    ? match.Value
                    : replacement);
        }

        /// <summary>
        /// Gets the start and the end (exclusive) of each quoted string literal in the text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static List<KeyValuePair<int, int>> GetStringLiteralRanges(string text)
        {
            List<KeyValuePair<int, int>> ranges = new();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] is not '"' and not '\'')
                {
                    continue;
                }

                int end = GetStringLiteralEnd(text, i);
                ranges.Add(new KeyValuePair<int, int>(i, end));
                i = end - 1;
            }
            return ranges;
        }

        /// <summary>
        /// Gets the index just past the string literal that starts at the given index,
        /// or the length of the text when the literal is not closed.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        internal static int GetStringLiteralEnd(string text, int start)
        {
            char quote = text[start];
            for (int i = start + 1; i < text.Length; i++)
            {
                if (text[i] is '\\')
                {
                    i++;
                    continue;
                }

                if (text[i] == quote)
                {
                    return i + 1;
                }
            }
            return text.Length;
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
