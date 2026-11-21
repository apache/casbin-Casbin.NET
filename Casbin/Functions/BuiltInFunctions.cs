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
using System.Net;
using System.Text.RegularExpressions;
using Casbin.Caching;
using Casbin.Rbac;
using DotNet.Globbing;

namespace Casbin.Functions
{
    public static class BuiltInFunctions
    {
        private static readonly Regex s_keyMatch2Regex = new(@":[^/]+");
        private static readonly Regex s_keyMatch3Regex = new(@"\{[^/]+\}");
        private static readonly Regex s_keyMatch4Regex = new(@"\{([^/]+)\}");
        private static readonly Regex s_keyMatch5Regex = new(@"\{[^/]+\}");
        private static readonly Regex s_keyGet2Regex = new(@":[^/]+");
        private static readonly Regex s_keyGet3Regex = new(@"\{[^/]+?\}");

        /// <summary>
        /// KeyGet returns the matched part
        /// For example, "/foo/bar/foo" matches "/foo/*"
        /// "bar/foo" will been returned
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static string KeyGet(string key1, string key2)
        {
            int m = key1.Length, n = key2.Length;
            if (m < n) return "";
            var span1 = key1.AsSpan();
            var span2 = key2.AsSpan();
            for (int i = 0; i < n; i++)
            {
                if (span1[i] != span2[i])
                    return span2[i] == '*' ? span1.Slice(i, m - i).ToString() : "";
            }

            return "";
        }

        /// <summary>
        /// KeyGet2 returns value matched pattern
        /// For example, "/resource1" matches "/:resource"
        /// if the pathVar == "resource", then "resource1" will be returned
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// /// <param name="pathVar">The path variable.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static string KeyGet2(string key1, string key2, string pathVar)
        {
            var replacedKey2 = key2.Replace("/*", "/.*");
            var keys = s_keyGet2Regex.Matches(replacedKey2);
            replacedKey2 = s_keyGet2Regex.Replace(replacedKey2, @"([^/]+)");
            replacedKey2 = "^" + replacedKey2 + "$";
            var regex = new Regex(replacedKey2);
            var values = regex.Matches(key1);
            if (values.Count == 0)
            {
                return "";
            }

            var pathVarSpan = pathVar.AsSpan();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Value.AsSpan(1).SequenceEqual(pathVarSpan))
                {
                    return values[0].Groups[i + 1].Value;
                }
            }

            return "";
        }

        /// <summary>
        /// KeyGet3 returns value matched pattern
        /// For example, "project/proj_project1_admin/" matches "project/proj_{project}_admin/"
        /// if the pathVar == "project", then "project1" will be returned
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// /// <param name="pathVar">The path variable.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static string KeyGet3(string key1, string key2, string pathVar)
        {
            var replacedKey2 = key2.Replace("/*", "/.*");
            var keys = s_keyGet3Regex.Matches(replacedKey2);
            replacedKey2 = s_keyGet3Regex.Replace(replacedKey2, @"([^/]+?)");
            replacedKey2 = "^" + replacedKey2 + "$";
            var regex = new Regex(replacedKey2);
            var values = regex.Matches(key1);
            if (values.Count == 0)
            {
                return "";
            }

            var pathVarSpan = pathVar.AsSpan();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Value.AsSpan(1, keys[i].Value.Length - 2).SequenceEqual(pathVarSpan))
                {
                    return values[0].Groups[i + 1].Value;
                }
            }

            return "";
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to RESTful path),
        /// key2 can contain a *. For example, "/foo/bar" matches "/foo/*".
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch(string key1, string key2)
        {
            int index = key2.IndexOf('*');

            if (index is 0)
            {
                return true;
            }

            if (index < 0)
            {
                return key1.Equals(key2);
            }

            key2 = key2.Substring(0, index);

            return index < key1.Length
                ? key1.Substring(0, index).Equals(key2)
                : key1.Equals(key2);
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to
        /// RESTful path), key2 can contain a*. For example, "/foo/bar" matches
        /// "/foo/*", "/resource1" matches "/:resource"
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch2(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");
            key2 = s_keyMatch2Regex.Replace(key2, "[^/]+");
            return RegexMatch(key1, $"^{key2}$");
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to
        /// RESTful path), key2 can contain a *. For example, "/foo/bar" matches
        ///  "/foo/*", "/resource1" matches "/{resource}"
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch3(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");
            key2 = s_keyMatch3Regex.Replace(key2, "[^/]+");
            return RegexMatch(key1, $"^{key2}$");
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 (similar to RESTful path), key2 can contain a *.
        /// Besides what KeyMatch3 does, KeyMatch4 can also match repeated patterns:
        /// "/parent/123/child/123" matches "/parent/{id}/child/{id}"
        /// "/parent/123/child/456" does not match "/parent/{id}/child/{id}"
        /// But KeyMatch3 will match both.
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool KeyMatch4(string key1, string key2)
        {
            key2 = key2.Replace("/*", "/.*");

            var tokens = new List<string>();

            key2 = s_keyMatch4Regex.Replace(key2, match =>
            {
                tokens.Add(match.Value.Substring(1, match.Value.Length - 2));
                return "([^/]+)";
            });

            var valueRegex = new Regex($"^{key2}$");
            var key1Match = valueRegex.Match(key1);

            if (!key1Match.Success)
            {
                return false;
            }

            int key1GroupMatchCount = key1Match.Groups.Count - 1;
            if (key1GroupMatchCount != tokens.Count)
            {
                throw new ArgumentException("KeyMatch4: number of tokens is not equal to number of values");
            }

            var group = key1Match.Groups;
            var valueDictionary = new Dictionary<string, string>();

            for (int i = 0; i < key1GroupMatchCount; i++)
            {
                string token = tokens[i];
                if (valueDictionary.ContainsKey(token) is false)
                {
                    valueDictionary.Add(token, group[i + 1].Value);
                    continue;
                }

                if (valueDictionary[token] != group[i + 1].Value)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// KeyMatch5 determines whether key1 matches the pattern of key2 (similar to RESTful path),
        /// key2 can contain a * or {param}. Query strings in key1 are ignored.
        /// For example, "/foo/bar?status=1&amp;type=2" matches "/foo/bar", "/foo/bar" matches "/foo/*",
        /// "/proxy/myid/res" matches "/proxy/{id}/*".
        /// </summary>
        /// <param name="key1"> The first argument. </param>
        /// <param name="key2"> The second argument. </param>
        /// <returns></returns>
        public static bool KeyMatch5(string key1, string key2)
        {
            int queryIndex = key1.IndexOf('?');
            if (queryIndex != -1)
            {
                key1 = key1.Substring(0, queryIndex);
            }

            key2 = key2.Replace("/*", "/.*");
            key2 = s_keyMatch5Regex.Replace(key2, "([^/]+)");

            return RegexMatch(key1, $"^{key2}$");
        }

        /// <summary>
        /// Determines whether IP address ip1 matches the pattern of IP address ip2,
        /// ip2 can be an IP address or a CIDR pattern. For example, "192.168.2.123"
        /// matches "192.168.2.0/24"
        /// </summary>
        /// <param name="ip1">The first argument.</param>
        /// <param name="ip2">The second argument.</param>
        /// <returns>Whether ip1 matches ip2.</returns>
        // ReSharper disable once InconsistentNaming
        public static bool IPMatch(string ip1, string ip2)
        {
            if (IPAddress.TryParse(ip1, out var ipAddress1) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip1)} is not an IP address.");
            }

            var ipSpan2 = ip2.AsSpan();
            int index = ipSpan2.IndexOf('/');
            IPAddress ipAddress2;

            if (index < 0)
            {
                if (IPAddress.TryParse(ip2, out ipAddress2) is false)
                {
                    throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
                }

                return ipAddress1.Equals(ipAddress2);
            }

            if (IPAddress.TryParse(ipSpan2.Slice(0, index).ToString(), out ipAddress2) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} is not an IP address.");
            }

            if (ipAddress1.AddressFamily != ipAddress2.AddressFamily)
            {
                return false;
            }

            if (byte.TryParse(ipSpan2.Slice(index + 1).ToString(), out byte cidrMask) is false)
            {
                throw new ArgumentException($"The argument {nameof(ip2)} has invalid CIDR mask.");
            }

            return ipAddress2.Match(ipAddress1, cidrMask);
        }

        /// <summary>
        /// Determines whether key1 matches the pattern of key2 in regular
        /// expression.
        /// </summary>
        /// <param name="key1">The first argument.</param>
        /// <param name="key2">The second argument.</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool RegexMatch(string key1, string key2)
        {
            return Regex.Match(key1, key2).Success;
        }

        /// <summary>
        /// Determines whether key1 matches the globbing pattern of key2
        /// </summary>
        /// <param name="key1">The keyword to match</param>
        /// <param name="key2">The Globbing pattern</param>
        /// <returns>Whether key1 matches key2.</returns>
        public static bool GlobMatch(string key1, string key2)
        {
            return Glob.Parse(key2).IsMatch(key1);
        }

        /// <summary>
        /// GenerateGFunction is the factory method of the g(_, _) function.
        /// </summary>
        /// <param name="roleManager">The role manager used by the function.</param>
        /// <param name="cache">Result cache</param>
        /// <returns>The function.</returns>
        internal static Delegate GenerateGFunction(IRoleManager roleManager, IGFunctionCache cache)
        {
            bool GFunction(string name1, string name2, string domain = null)
            {
                if (cache.TryGet(name1, name2, out bool result, domain))
                {
                    return result;
                }

                result = roleManager.HasLink(name1, name2, domain);
                cache.Set(name1, name2, result, domain);
                return result;
            }

            return (GFunction)GFunction;
        }

        private delegate bool GFunction(string subject1, string subject2, string domain = null);
    }
}
