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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Casbin.Config;
using Casbin.Util;

namespace Casbin.Model;

public class DefaultSections : ISections
{
    private static readonly IDictionary<string, string> s_sectionNameMap = new Dictionary<string, string>
    {
        { PermConstants.Section.RequestSection, PermConstants.Section.RequestSectionName },
        { PermConstants.Section.PolicySection, PermConstants.Section.PolicySectionName },
        { PermConstants.Section.RoleSection, PermConstants.Section.RoleSectionName },
        { PermConstants.Section.PolicyEffectSection, PermConstants.Section.PolicyEffectSectionName },
        { PermConstants.Section.MatcherSection, PermConstants.Section.MatcherSectionName }
    };

    private readonly IDictionary<string, IDictionary<string, IReadOnlyAssertion>> _assertionsMap
        = new Dictionary<string, IDictionary<string, IReadOnlyAssertion>>();

    public bool ContainsSection(string section) => _assertionsMap.ContainsKey(section);

    public bool AddSection(string section, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Assertion assertion = section switch
        {
            PermConstants.Section.RequestSection => new RequestAssertion { Key = key, Value = value },
            PermConstants.Section.PolicySection => new PolicyAssertion { Key = key, Value = value },
            PermConstants.Section.RoleSection => new RoleAssertion { Key = key, Value = value },
            PermConstants.Section.PolicyEffectSection => new PolicyEffectAssertion { Key = key, Value = value },
            PermConstants.Section.MatcherSection => new MatcherAssertion { Key = key, Value = value },
            _ => throw new ArgumentOutOfRangeException(nameof(section), section, null)
        };

        if (section.Equals(PermConstants.Section.RequestSection)
            || section.Equals(PermConstants.Section.PolicySection)
            || section.Equals(PermConstants.Section.RoleSection))
        {
            string[] tokens = assertion.Value.Split(PermConstants.PolicySeparatorChar)
                .Select(t => t.Trim()).ToArray();
            if (tokens.Length is not 0)
            {
                Dictionary<string, int> tokenDic = new();
                for (int i = 0; i < tokens.Length; i++)
                {
                    string token = tokens[i].Trim();
                    if (token is "_")
                    {
                        token = i.ToString();
                    }

                    tokenDic.Add(token, i);
                }

                assertion.Tokens = tokenDic;
            }
        }
        else
        {
            // ReSharper disable once InvokeAsExtensionMethod
            assertion.Value = StringUtil.RemoveComments(assertion.Value);
        }


        if (_assertionsMap.TryGetValue(section, out IDictionary<string, IReadOnlyAssertion>? assertions))
        {
            // TryAdd is not supported in .NET 4.5
            if (assertions.ContainsKey(key))
            {
                return false;
            }

            assertions.Add(key, assertion);
            return true;
        }

        Dictionary<string, IReadOnlyAssertion> assertionMap = new() { [key] = assertion };

        _assertionsMap.Add(section, assertionMap);
        return true;
    }

    public T GetAssertion<T>(string section, string type)
        where T : Assertion
    {
        if (_assertionsMap.TryGetValue(section, out IDictionary<string, IReadOnlyAssertion>? assertions) is false)
        {
            throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }

        if (assertions.TryGetValue(type, out IReadOnlyAssertion? assertion) is false)
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        return (T)assertion;
    }

    public bool TryGetAssertion<T>(string section, string type, out T? outAssertion)
        where T : Assertion
    {
        if (_assertionsMap.TryGetValue(section, out IDictionary<string, IReadOnlyAssertion>? assertions) is false)
        {
            outAssertion = default;
            return false;
        }

        if (assertions.TryGetValue(type, out IReadOnlyAssertion? assertion) is false)
        {
            outAssertion = default;
            return false;
        }

        outAssertion = assertion as T;
        return outAssertion is not null;
    }

    public IDictionary<string, T> GetAssertions<T>(string section)
        where T : Assertion
    {
        if (_assertionsMap.TryGetValue(section, out IDictionary<string, IReadOnlyAssertion>? assertions) is false)
        {
            throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }

        return assertions.ToDictionary(kv => kv.Key, kv => (T)kv.Value);
    }

    public void LoadSection(IConfig config, string section)
    {
        int i = 1;
        while (true)
        {
            string key = string.Concat(section, GetKeySuffix(i));
            if (LoadAssertion(config, section, key) is false)
            {
                break;
            }

            i++;
        }
    }


    private bool LoadAssertion(IConfig config, string section, string key)
    {
        if (s_sectionNameMap.TryGetValue(section, out string? sectionName) is false)
        {
            return false;
        }

        string value = config.GetString($"{sectionName}::{key}");
        return !string.IsNullOrWhiteSpace(value) && AddSection(section, key, value);
    }

    private static string GetKeySuffix(int i) => i == 1 ? string.Empty : i.ToString();
}
