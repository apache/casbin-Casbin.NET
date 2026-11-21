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
using Casbin.Rbac;

namespace Casbin.Model;

public class RoleAssertion : PolicyAssertion
{
    public RoleAssertion() => Section = PermConstants.Section.RoleSection;

    public IRoleManager RoleManager { get; internal set; }

    internal int GroupPolicyCount
    {
        get
        {
            int paramsIndex = Value.IndexOf('(');
            string value = paramsIndex is -1 ? Value : Value.Substring(0, paramsIndex);
            return value.Count(c => c is '_');
        }
    }

    public void BuildRoleLinks()
    {
        int count = GroupPolicyCount;
        if (count < 2)
        {
            throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
        }

        foreach (IPolicyValues policy in PolicyManager.GetPolicy())
        {
            BuildRoleLink(count, PolicyOperation.AddPolicy, policy);
        }
    }

    internal void BuildIncrementalRoleLink(PolicyOperation policyOperation, IEnumerable<string> rule)
    {
        int count = GroupPolicyCount;
        if (count < 2)
        {
            throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
        }

        BuildRoleLink(count, policyOperation, rule);
    }

    internal void BuildIncrementalRoleLink(PolicyOperation policyOperation,
        IEnumerable<string> oldRule, IEnumerable<string> newRule)
    {
        int count = GroupPolicyCount;
        if (count < 2)
        {
            throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
        }

        BuildRoleLink(count, policyOperation, oldRule, newRule);
    }

    internal void BuildIncrementalRoleLinks(PolicyOperation policyOperation, IEnumerable<IEnumerable<string>> rules)
    {
        int count = GroupPolicyCount;
        if (count < 2)
        {
            throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
        }

        foreach (IEnumerable<string> rule in rules)
        {
            BuildRoleLink(count, policyOperation, rule);
        }
    }

    internal void BuildIncrementalRoleLinks(PolicyOperation policyOperation,
        IEnumerable<IEnumerable<string>> oldRules, IEnumerable<IEnumerable<string>> newRules)
    {
        int count = GroupPolicyCount;
        if (count < 2)
        {
            throw new InvalidOperationException("the number of \"_\" in role definition should be at least 2.");
        }

        IReadOnlyList<IEnumerable<string>> rulesList =
            oldRules as IReadOnlyList<IEnumerable<string>> ?? oldRules.ToList();
        IReadOnlyList<IEnumerable<string>> newRulesList =
            newRules as IReadOnlyList<IEnumerable<string>> ?? newRules.ToList();
        if (rulesList.Count != newRulesList.Count)
        {
            throw new InvalidOperationException(
                $"the length of oldPolices should be equal to the length of newPolices, but got the length of oldPolices is {rulesList.Count}, the length of newPolices is {newRulesList.Count}.");
        }

        for (int i = 0; i < rulesList.Count; i++)
        {
            BuildRoleLink(count, policyOperation, rulesList[i], newRulesList[i]);
        }
    }

    private void BuildRoleLink(int groupPolicyCount, PolicyOperation policyOperation,
        IEnumerable<string> rule, IEnumerable<string> newRule = null)
    {
        IRoleManager roleManager = RoleManager;
        List<string> ruleEnum = rule as List<string> ?? rule.ToList();
        int ruleCount = ruleEnum.Count;

        if (ruleCount < groupPolicyCount)
        {
            throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
        }

        if (ruleCount > groupPolicyCount)
        {
            ruleEnum = ruleEnum.GetRange(0, groupPolicyCount);
        }

        switch (policyOperation)
        {
            case PolicyOperation.AddPolicy:
            case PolicyOperation.AddPolicies:
                switch (groupPolicyCount)
                {
                    case 2:
                        roleManager.AddLink(ruleEnum[0], ruleEnum[1]);
                        break;
                    case 3:
                        roleManager.AddLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                }

                break;
            case PolicyOperation.UpdatePolicy:
            case PolicyOperation.UpdatePolicies:
                if (newRule == null)
                {
                    throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
                }

                List<string> newRuleEnum = newRule as List<string> ?? newRule.ToList();
                int newRuleCount = newRuleEnum.Count;
                if (newRuleCount < groupPolicyCount)
                {
                    throw new InvalidOperationException("Grouping policy elements do not meet role definition.");
                }

                if (newRuleCount > groupPolicyCount)
                {
                    newRuleEnum = newRuleEnum.GetRange(0, groupPolicyCount);
                }

                switch (groupPolicyCount)
                {
                    case 2:
                        roleManager.DeleteLink(ruleEnum[0], ruleEnum[1]);
                        roleManager.AddLink(newRuleEnum[0], newRuleEnum[1]);
                        break;
                    case 3:
                        roleManager.DeleteLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                        roleManager.AddLink(newRuleEnum[0], newRuleEnum[1], newRuleEnum[2]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                }

                break;
            case PolicyOperation.RemovePolicy:
            case PolicyOperation.RemovePolicies:
            case PolicyOperation.RemoveFilteredPolicy:
                switch (groupPolicyCount)
                {
                    case 2:
                        roleManager.DeleteLink(ruleEnum[0], ruleEnum[1]);
                        break;
                    case 3:
                        roleManager.DeleteLink(ruleEnum[0], ruleEnum[1], ruleEnum[2]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(groupPolicyCount), groupPolicyCount, null);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(policyOperation), policyOperation, null);
        }
    }

    internal static string GetNameWithDomain(string domain, string name) =>
        domain + PermConstants.SubjectPrioritySeparatorString + name;

    internal Dictionary<string, int> GetSubjectHierarchyMap()
    {
        Dictionary<string, int> refer = new();
        Dictionary<string, int> map = new();
        Dictionary<string, List<string>> policyChildrenMap = new();
        foreach (IPolicyValues policy in PolicyManager.GetPolicy())
        {
            string domain = policy.Count > 2 ? policy[2] : null;
            string child = GetNameWithDomain(domain, policy[0]);
            string parent = GetNameWithDomain(domain, policy[1]);
            if (policyChildrenMap.ContainsKey(parent))
            {
                policyChildrenMap[parent].Add(child);
            }
            else
            {
                policyChildrenMap[parent] = new List<string>(new[] { child });
            }

            if (refer.ContainsKey(parent) is false)
            {
                refer[parent] = 0;
            }

            refer[child] = 1;
        }

        Queue<string> queue = new();
        foreach (KeyValuePair<string, int> keyValuePair in refer)
        {
            if (keyValuePair.Value is not 0)
            {
                continue;
            }

            int level = 0;
            queue.Enqueue(keyValuePair.Key);
            while (queue.Count > 0)
            {
                int size = queue.Count;
                while (size-- > 0)
                {
                    string node = queue.Dequeue();
                    map[node] = level;
                    if (policyChildrenMap.ContainsKey(node) is false)
                    {
                        continue;
                    }

                    foreach (string child in policyChildrenMap[node])
                    {
                        queue.Enqueue(child);
                    }
                }

                level++;
            }
        }

        return map;
    }
}
