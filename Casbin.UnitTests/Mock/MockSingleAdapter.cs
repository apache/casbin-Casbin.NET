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

using System.Collections.Generic;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;

namespace Casbin.UnitTests.Mock;

public class MockSingleAdapter : FileAdapter, ISingleAdapter
{
    public List<string> SavedPolicies { get; } = new();

    public MockSingleAdapter(string filePath) : base(filePath)
    {
    }

    public void AddPolicy(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"AddPolicy: {section}.{policyType} {rule.ToText()}");
    }

    public Task AddPolicyAsync(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"AddPolicyAsync: {section}.{policyType} {rule.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule)
    {
        SavedPolicies.Add($"UpdatePolicy: {section}.{policyType} {oldRule.ToText()} -> {newRule.ToText()}");
    }

    public Task UpdatePolicyAsync(string section, string policyType, IPolicyValues oldRules, IPolicyValues newRules)
    {
        SavedPolicies.Add($"UpdatePolicyAsync: {section}.{policyType} {oldRules.ToText()} -> {newRules.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void RemovePolicy(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"RemovePolicy: {section}.{policyType} {rule.ToText()}");
    }

    public Task RemovePolicyAsync(string section, string policyType, IPolicyValues rule)
    {
        SavedPolicies.Add($"RemovePolicyAsync: {section}.{policyType} {rule.ToText()}");
#if NET452
        return Task.FromResult(0);
#else
        return Task.CompletedTask;
#endif
    }

    public void ClearSavedPolicies()
    {
        SavedPolicies.Clear();
    }
}
