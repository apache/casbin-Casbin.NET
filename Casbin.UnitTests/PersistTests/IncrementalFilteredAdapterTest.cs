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

using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

public class IncrementalFilteredAdapterTest
{
    [Fact]
    public void TestLoadIncrementalFilteredPolicy()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1 (from p policy)
        Assert.True(e.Enforce("alice", "data1", "read"));
        // bob cannot write data2 (not loaded yet)
        Assert.False(e.Enforce("bob", "data2", "write"));

        // Incrementally load p policies for data2_admin role
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["data2_admin"])));

        // alice still can only read data1 (no role link yet)
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data2", "read"));

        // Incrementally load g policies for alice
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(["alice"])));

        // Now alice can read data2 through role inheritance
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        // bob still cannot write data2 (not loaded)
        Assert.False(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadIncrementalFilteredPolicyAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1 (from p policy)
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // bob cannot write data2 (not loaded yet)
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));

        // Incrementally load p policies for data2_admin role
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["data2_admin"])));

        // alice still can only read data1 (no role link yet)
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));

        // Incrementally load g policies for alice
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultRoleType, 0, Policy.ValuesFrom(["alice"])));

        // Now alice can read data2 through role inheritance
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        // bob still cannot write data2 (not loaded)
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestLoadIncrementalFilteredPolicyMultiplePTypes()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1
        Assert.True(e.Enforce("alice", "data1", "read"));
        // bob cannot write data2
        Assert.False(e.Enforce("bob", "data2", "write"));

        // Incrementally load p policies for bob
        e.LoadIncrementalFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice still can read data1
        Assert.True(e.Enforce("alice", "data1", "read"));
        // Now bob can write data2
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadIncrementalFilteredPolicyMultiplePTypesAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));

        // alice can read data1
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // bob cannot write data2
        Assert.False(await e.EnforceAsync("bob", "data2", "write"));

        // Incrementally load p policies for bob
        await e.LoadIncrementalFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice still can read data1
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        // Now bob can write data2
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestLoadFilteredPolicyClearsExistingPolicies()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));
        Assert.True(e.Enforce("alice", "data1", "read"));

        // Load filtered policy again (should clear previous policies)
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice can no longer read data1 (previous policies cleared)
        Assert.False(e.Enforce("alice", "data1", "read"));
        // bob can write data2 (new filtered policies loaded)
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestLoadFilteredPolicyClearsExistingPoliciesAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        FileAdapter a = new("Examples/rbac_policy.csv");
        e.SetAdapter(a);

        // Load only p policies for alice
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["alice"])));
        Assert.True(await e.EnforceAsync("alice", "data1", "read"));

        // Load filtered policy again (should clear previous policies)
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        // alice can no longer read data1 (previous policies cleared)
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));
        // bob can write data2 (new filtered policies loaded)
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }
}
