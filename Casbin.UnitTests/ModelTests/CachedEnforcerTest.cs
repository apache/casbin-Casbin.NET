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
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class CachedEnforcerTest
{
    private readonly TestModelFixture _testModelFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public CachedEnforcerTest(ITestOutputHelper testOutputHelper, TestModelFixture testModelFixture)
    {
        _testOutputHelper = testOutputHelper;
        _testModelFixture = testModelFixture;
    }

    [Fact]
    public void TestEnforceWithCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);
        e.EnableAutoCleanEnforceCache(false);

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // Now we invalidate the cache, then all first-coming Enforce() has to be evaluated in real-time.
        // The decision for ("alice", "data1", "read") will be false now.
        e.EnforceCache.Clear();

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public void TestAutoCleanCache()
    {
#if !NET452
        Enforcer e = new(TestModelFixture.GetBasicTestModel())
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };
#else
            var e = new Enforcer(TestModelFixture.GetBasicTestModel());
#endif
        e.EnableCache(true);

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));

        // The cache is enabled, so even if we remove a policy rule, the decision
        // for ("alice", "data1", "read") will still be true, as it uses the cached result.
        _ = e.RemovePolicy("alice", "data1", "read");

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public void TestCacheDropsResultWhenPolicyChangesDuringEnforce()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(@"
[request_definition]
r = sub, obj, act

[policy_definition]
p = sub, obj, act

[role_definition]
g = _, _

[policy_effect]
e = some(where (p.eft == allow))

[matchers]
m = g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act && removeRole()
", TestModelFixture.RbacPolicyText));
        e.BuildRoleLinks();
        bool removeRole = true;
        e.AddFunction("removeRole", new Func<bool>(() =>
        {
            if (removeRole)
            {
                removeRole = false;
                e.RemoveGroupingPolicy("alice", "data2_admin");
            }
            return true;
        }));

        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "read"));
    }

    [Fact]
    public void TestCacheKeysDoNotCollide()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        e.AddPolicy("alice$$data", "x", "read");

        Assert.True(e.Enforce("alice$$data", "x", "read"));
        Assert.False(e.Enforce("alice", "data$$x", "read"));
        Assert.False(e.Enforce("alice$$data$$x", "", "read"));
    }

    [Fact]
    public void TestGFunctionCacheKeysDoNotCollide()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.EnableCache(false);
        e.AddPolicy("x:data2_admin", "data3", "read");
        e.AddGroupingPolicy("bob:x", "data2_admin");

        Assert.True(e.Enforce("bob:x", "data2", "read"));
        Assert.False(e.Enforce("bob", "data3", "read"));
    }
}
