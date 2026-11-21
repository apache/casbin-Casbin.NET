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
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Extensions;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class ManagementApiTest
{
    [Fact]
    public void TestGetList()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        Assert.True(e.GetAllSubjects().DeepEquals(new[] { "alice", "bob", "data2_admin" }));
        Assert.True(e.GetAllObjects().DeepEquals(new[] { "data1", "data2" }));
        Assert.True(e.GetAllActions().DeepEquals(new[] { "read", "write" }));
        Assert.True(e.GetAllRoles().DeepEquals(new[] { "data2_admin" }));
    }

    [Fact]
    public void TestGetPolicyApi()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        e.TestGetPolicy(
            [
                ["alice", "data1", "read"],
                ["bob", "data2", "write"],
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"]
            ]
        );

        TestGetFilteredPolicy(e, 0, [["alice", "data1", "read"]], "alice");
        TestGetFilteredPolicy(e, 0, [["bob", "data2", "write"]], "bob");
        TestGetFilteredPolicy(e, 0,
            [["data2_admin", "data2", "read"], ["data2_admin", "data2", "write"]], "data2_admin");
        TestGetFilteredPolicy(e, 1, [["alice", "data1", "read"]], "data1");
        TestGetFilteredPolicy(e, 1,
        [
            ["bob", "data2", "write"], ["data2_admin", "data2", "read"],
            ["data2_admin", "data2", "write"]
        ], "data2");
        TestGetFilteredPolicy(e, 2,
            [["alice", "data1", "read"], ["data2_admin", "data2", "read"]], "read");
        TestGetFilteredPolicy(e, 2,
            [["bob", "data2", "write"], ["data2_admin", "data2", "write"]], "write");

        TestGetFilteredPolicy(e, 0,
            [["data2_admin", "data2", "read"], ["data2_admin", "data2", "write"]], "data2_admin",
            "data2");
        // Note: "" (empty string) in fieldValues means matching all values.
        TestGetFilteredPolicy(e, 0, [["data2_admin", "data2", "read"]], "data2_admin", "", "read");
        TestGetFilteredPolicy(e, 1,
            [["bob", "data2", "write"], ["data2_admin", "data2", "write"]], "data2", "write");

        Assert.True(e.HasPolicy(["alice", "data1", "read"]));
        Assert.True(e.HasPolicy(["bob", "data2", "write"]));
        Assert.False(e.HasPolicy(["alice", "data2", "read"]));
        Assert.False(e.HasPolicy(["bob", "data3", "write"]));

        Assert.True(e.GetGroupingPolicy().DeepEquals([["alice", "data2_admin"]]));

        TestGetFilteredGroupingPolicy(e, 0, [["alice", "data2_admin"]], "alice");
        TestGetFilteredGroupingPolicy(e, 0, [], "bob");
        TestGetFilteredGroupingPolicy(e, 1, [], "data1_admin");
        TestGetFilteredGroupingPolicy(e, 1, [["alice", "data2_admin"]], "data2_admin");
        // Note: "" (empty string) in fieldValues means matching all values.
        TestGetFilteredGroupingPolicy(e, 0, [["alice", "data2_admin"]], "", "data2_admin");

        Assert.True(e.HasGroupingPolicy(["alice", "data2_admin"]));
        Assert.False(e.HasGroupingPolicy(["bob", "data2_admin"]));
    }

    [Fact]
    public void TestModifyPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        e.TestGetPolicy([
            ["alice", "data1", "read"],
            ["bob", "data2", "write"],
            ["data2_admin", "data2", "read"],
            ["data2_admin", "data2", "write"]
        ]);

        e.RemovePolicy("alice", "data1", "read");
        e.RemovePolicy("bob", "data2", "write");
        e.RemovePolicy("alice", "data1", "read");
        e.AddPolicy("eve", "data3", "read");
        e.AddPolicy("eve", "data3", "read");

        List<List<string>> rules =
        [
            ["jack", "data4", "read"],
            ["jack", "data4", "read"],
            ["jack", "data4", "read"],
            ["katy", "data4", "write"],
            ["leyo", "data4", "read"],
            ["katy", "data4", "write"],
            ["katy", "data4", "write"],
            ["ham", "data4", "write"]
        ];

        _ = e.AddPolicies(rules);
        _ = e.AddPolicies(rules);

        e.TestGetPolicy([
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"],
                ["eve", "data3", "read"],
                ["jack", "data4", "read"],
                ["katy", "data4", "write"],
                ["leyo", "data4", "read"],
                ["ham", "data4", "write"]
            ]
        );

        _ = e.RemovePolicies(rules);
        _ = e.RemovePolicies(rules);

        List<string> namedPolicy = ["eve", "data3", "read"];
        e.RemoveNamedPolicy("p", namedPolicy);
        e.AddNamedPolicy("p", namedPolicy);

        e.TestGetPolicy([
            ["data2_admin", "data2", "read"],
            ["data2_admin", "data2", "write"],
            ["eve", "data3", "read"]
        ]);

        e.RemoveFilteredPolicy(1, "data2");
        e.TestGetPolicy([["eve", "data3", "read"]]);

        e.RemoveFilteredPolicy(1);
        e.TestGetPolicy([["eve", "data3", "read"]]);

        e.RemoveFilteredPolicy(1, "");
        e.TestGetPolicy([["eve", "data3", "read"]]);

        bool res = e.UpdatePolicy(["eve", "data3", "read"], ["eve", "data3", "write"]);
        e.TestGetPolicy([["eve", "data3", "write"]]);
        Assert.True(res);

        // This test shows that a non-existent policy will not be updated.
        res = e.UpdatePolicy(["non_exist", "data3", "write"], ["non_exist", "data3", "read"]);
        e.TestGetPolicy([["eve", "data3", "write"]]);
        Assert.False(res);

        e.AddPolicies(rules);
        e.TestGetPolicy([
            ["eve", "data3", "write"],
            ["jack", "data4", "read"],
            ["katy", "data4", "write"],
            ["leyo", "data4", "read"],
            ["ham", "data4", "write"]
        ]);
        res = e.UpdatePolicies(
            [
                ["eve", "data3", "write"],
                ["leyo", "data4", "read"],
                ["katy", "data4", "write"]
            ],
            [
                ["eve", "data3", "read"],
                ["leyo", "data4", "write"],
                ["katy", "data1", "write"]
            ]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.True(res);

        // This test shows that a non-existent policy in oldParameters will not be updated, so other existent ones
        // will be ignored and the return value will be False.
        res = e.UpdatePolicies(
            [["eve", "data3", "read"], ["non_exist", "data4", "read"]],
            [["eve", "data3", "write"], ["non_exist", "data4", "write"]]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.False(res);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = e.UpdatePolicies(
            [["eve", "data3", "read"], ["leyo", "data4", "write"]],
            [["eve", "data3", "write"]]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.False(res);
    }

    [Fact]
    public async Task TestModifyPolicyAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        e.TestGetPolicy(
            [
                ["alice", "data1", "read"],
                ["bob", "data2", "write"],
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"]
            ]
        );

        await e.RemovePolicyAsync("alice", "data1", "read");
        await e.RemovePolicyAsync("bob", "data2", "write");
        await e.RemovePolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("eve", "data3", "read");
        await e.AddPolicyAsync("eve", "data3", "read");

        List<List<string>> rules =
        [
            ["jack", "data4", "read"],
            ["jack", "data4", "read"],
            ["jack", "data4", "read"],
            ["katy", "data4", "write"],
            ["leyo", "data4", "read"],
            ["katy", "data4", "write"],
            ["katy", "data4", "write"],
            ["ham", "data4", "write"]
        ];

        _ = await e.AddPoliciesAsync(rules);
        _ = await e.AddPoliciesAsync(rules);

        e.TestGetPolicy(
            [
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"],
                ["eve", "data3", "read"],
                ["jack", "data4", "read"],
                ["katy", "data4", "write"],
                ["leyo", "data4", "read"],
                ["ham", "data4", "write"]
            ]
        );

        _ = await e.RemovePoliciesAsync(rules);
        _ = await e.RemovePoliciesAsync(rules);

        List<string> namedPolicy = ["eve", "data3", "read"];
        await e.RemoveNamedPolicyAsync("p", namedPolicy);
        await e.AddNamedPolicyAsync("p", namedPolicy);

        e.TestGetPolicy(
            [
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"],
                ["eve", "data3", "read"]
            ]
        );

        await e.RemoveFilteredPolicyAsync(1, "data2");

        e.TestGetPolicy(
            [
                ["eve", "data3", "read"]
            ]
        );

        bool res = await e.UpdatePolicyAsync(["eve", "data3", "read"], ["eve", "data3", "write"]);
        e.TestGetPolicy(
            [
                ["eve", "data3", "write"]
            ]
        );

        Assert.True(res);

        // This test shows that a non-existent policy will not be updated.
        res = await e.UpdatePolicyAsync(["non_exist", "data3", "write"],
            ["non_exist", "data3", "read"]);
        e.TestGetPolicy([["eve", "data3", "write"]]);
        Assert.False(res);

        await e.AddPoliciesAsync(rules);
        e.TestGetPolicy([
            ["eve", "data3", "write"],
            ["jack", "data4", "read"],
            ["katy", "data4", "write"],
            ["leyo", "data4", "read"],
            ["ham", "data4", "write"]
        ]);

        res = await e.UpdatePoliciesAsync(
            [
                ["eve", "data3", "write"],
                ["leyo", "data4", "read"],
                ["katy", "data4", "write"]
            ],
            [
                ["eve", "data3", "read"],
                ["leyo", "data4", "write"],
                ["katy", "data1", "write"]
            ]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.True(res);

        // This test shows that a non-existent policy in oldParameters will not be updated, so other existent ones
        // will be ignored and the return value will be False.
        res = await e.UpdatePoliciesAsync(
            [["eve", "data3", "read"], ["non_exist", "data4", "read"]],
            [["eve", "data3", "write"], ["non_exist", "data4", "write"]]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.False(res);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = await e.UpdatePoliciesAsync(
            [["eve", "data3", "read"], ["leyo", "data4", "write"]],
            [["eve", "data3", "write"]]);
        e.TestGetPolicy([
            ["eve", "data3", "read"],
            ["jack", "data4", "read"],
            ["katy", "data1", "write"],
            ["leyo", "data4", "write"],
            ["ham", "data4", "write"]
        ]);
        Assert.False(res);
    }

    private static List<List<string>> GenerateGroupingRules(int numberOfItems)
    {
        var groupingRules = new List<List<string>>();

        for (int i = 1; i <= numberOfItems; i++)
        {
            string parent = $"Parent{i}";
            string child = $"Child{i}";
            groupingRules.Add([parent, child]);
        }

        return groupingRules;
    }

    [Fact]
    public void TestConcurrentModifyGroupingPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewSaaRbacTestModel());
        e.BuildRoleLinks();

        // Arrange
        var policiesBeforeAct = e.GetNamedGroupingPolicy(PermConstants.GroupingPolicyType2).ToArray();
        Assert.Empty(policiesBeforeAct);

        var groupingRules = GenerateGroupingRules(1000);

        Task.WaitAll(groupingRules.Select(rule => Task.Run(() =>
        {
            bool result =
                e.AddNamedGroupingPolicies(PermConstants.GroupingPolicyType2, new List<List<string>> { rule });
            Assert.True(result);
        })).ToArray());

        // Assert
        var policiesAfterAct = e.GetNamedGroupingPolicy(PermConstants.GroupingPolicyType2).ToArray();
        foreach (var policy in groupingRules)
        {
            Assert.Contains(policy, policiesAfterAct);
        }
    }

    [Fact]
    public void TestModifyGroupingPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", ["data2_admin"]);
        TestGetRoles(e, "bob", []);
        TestGetRoles(e, "eve", []);
        TestGetRoles(e, "non_exist", []);

        e.RemoveGroupingPolicy("alice", "data2_admin");
        e.AddGroupingPolicy("bob", "data1_admin");
        e.AddGroupingPolicy("eve", "data3_admin");

        List<List<string>> groupingRules =
        [
            ["ham", "data4_admin"],
            ["jack", "data5_admin"]
        ];

        _ = e.AddGroupingPolicies(groupingRules);
        TestGetRoles(e, "ham", ["data4_admin"]);
        TestGetRoles(e, "jack", ["data5_admin"]);
        _ = e.RemoveGroupingPolicies(groupingRules);

        TestGetRoles(e, "alice", []);

        List<string> namedGroupingPolicy = ["alice", "data2_admin"];
        TestGetRoles(e, "alice", []);
        e.AddNamedGroupingPolicy("g", namedGroupingPolicy);
        TestGetRoles(e, "alice", ["data2_admin"]);
        e.RemoveNamedGroupingPolicy("g", namedGroupingPolicy);

        e.AddNamedGroupingPolicies("g", groupingRules);
        e.AddNamedGroupingPolicies("g", groupingRules);
        TestGetRoles(e, "ham", ["data4_admin"]);
        TestGetRoles(e, "jack", ["data5_admin"]);
        e.RemoveNamedGroupingPolicies("g", groupingRules);
        e.RemoveNamedGroupingPolicies("g", groupingRules);

        TestGetRoles(e, "alice", []);
        TestGetRoles(e, "bob", ["data1_admin"]);
        TestGetRoles(e, "eve", ["data3_admin"]);
        TestGetRoles(e, "non_exist", []);

        TestGetUsers(e, "data1_admin", ["bob"]);
        TestGetUsers(e, "data2_admin", []);
        TestGetUsers(e, "data3_admin", ["eve"]);

        e.RemoveFilteredGroupingPolicy(0, "bob");

        TestGetRoles(e, "alice", []);
        TestGetRoles(e, "bob", []);
        TestGetRoles(e, "eve", ["data3_admin"]);
        TestGetRoles(e, "non_exist", []);

        TestGetUsers(e, "data1_admin", []);
        TestGetUsers(e, "data2_admin", []);
        TestGetUsers(e, "data3_admin", ["eve"]);

        e.AddGroupingPolicy("data3_admin", "data4_admin");
        bool res = e.UpdateGroupingPolicy(["eve", "data3_admin"], ["eve", "admin"]);
        Assert.True(res);
        res = e.UpdateGroupingPolicy(["data3_admin", "data4_admin"], ["admin", "data4_admin"]);
        Assert.True(res);
        TestGetUsers(e, "data4_admin", ["admin"]);
        TestGetUsers(e, "admin", ["eve"]);
        TestGetRoles(e, "eve", ["admin"]);
        TestGetRoles(e, "admin", ["data4_admin"]);

        res = e.UpdateGroupingPolicy(["non_exist", "data4_admin"], ["non_exist2", "data4_admin"]);
        Assert.False(res);
        TestGetUsers(e, "data4_admin", ["admin"]);

        res = e.UpdateGroupingPolicies(
            [
                ["eve", "admin"],
                ["admin", "data4_admin"]
            ],
            [
                ["eve", "admin_groups"],
                ["admin", "data5_admin"]
            ]);

        Assert.True(res);
        TestGetUsers(e, "data5_admin", ["admin"]);
        TestGetUsers(e, "admin_groups", ["eve"]);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);

        res = e.UpdateGroupingPolicies(
            [
                ["admin", "data5_admin"],
                ["non_exist", "admin_groups"]
            ],
            [
                ["admin", "data6_admin"],
                ["non_exist2", "admin_groups"]
            ]);
        Assert.False(res);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = e.UpdateGroupingPolicies(
            [
                ["admin", "data5_admin"],
                ["eve", "admin2_groups"]
            ],
            [["admin", "data6_admin"]]);
        Assert.False(res);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);
    }

    [Fact]
    public async Task TestModifyGroupingPolicyAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        TestGetRoles(e, "alice", ["data2_admin"]);
        TestGetRoles(e, "bob", []);
        TestGetRoles(e, "eve", []);
        TestGetRoles(e, "non_exist", []);

        await e.RemoveGroupingPolicyAsync("alice", "data2_admin");
        await e.AddGroupingPolicyAsync("bob", "data1_admin");
        await e.AddGroupingPolicyAsync("eve", "data3_admin");

        List<List<string>> groupingRules =
        [
            ["ham", "data4_admin"],
            ["jack", "data5_admin"]
        ];

        _ = await e.AddGroupingPoliciesAsync(groupingRules);
        TestGetRoles(e, "ham", ["data4_admin"]);
        TestGetRoles(e, "jack", ["data5_admin"]);
        _ = await e.RemoveGroupingPoliciesAsync(groupingRules);

        TestGetRoles(e, "alice", []);
        List<string> namedGroupingPolicy = ["alice", "data2_admin"];
        TestGetRoles(e, "alice", []);
        await e.AddNamedGroupingPolicyAsync("g", namedGroupingPolicy);
        TestGetRoles(e, "alice", ["data2_admin"]);
        await e.RemoveNamedGroupingPolicyAsync("g", namedGroupingPolicy);

        await e.AddNamedGroupingPoliciesAsync("g", groupingRules);
        await e.AddNamedGroupingPoliciesAsync("g", groupingRules);
        TestGetRoles(e, "ham", ["data4_admin"]);
        TestGetRoles(e, "jack", ["data5_admin"]);
        await e.RemoveNamedGroupingPoliciesAsync("g", groupingRules);
        await e.RemoveNamedGroupingPoliciesAsync("g", groupingRules);

        TestGetRoles(e, "alice", []);
        TestGetRoles(e, "bob", ["data1_admin"]);
        TestGetRoles(e, "eve", ["data3_admin"]);
        TestGetRoles(e, "non_exist", []);

        TestGetUsers(e, "data1_admin", ["bob"]);
        TestGetUsers(e, "data2_admin", []);
        TestGetUsers(e, "data3_admin", ["eve"]);

        await e.RemoveFilteredGroupingPolicyAsync(0, "bob");

        TestGetRoles(e, "alice", []);
        TestGetRoles(e, "bob", []);
        TestGetRoles(e, "eve", ["data3_admin"]);
        TestGetRoles(e, "non_exist", []);

        TestGetUsers(e, "data1_admin", []);
        TestGetUsers(e, "data2_admin", []);
        TestGetUsers(e, "data3_admin", ["eve"]);

        await e.AddGroupingPolicyAsync("data3_admin", "data4_admin");
        bool res = await e.UpdateGroupingPolicyAsync(["eve", "data3_admin"], ["eve", "admin"]);
        Assert.True(res);
        res = await e.UpdateGroupingPolicyAsync(["data3_admin", "data4_admin"],
            ["admin", "data4_admin"]);
        Assert.True(res);
        TestGetUsers(e, "data4_admin", ["admin"]);
        TestGetUsers(e, "admin", ["eve"]);
        TestGetRoles(e, "eve", ["admin"]);
        TestGetRoles(e, "admin", ["data4_admin"]);

        res = await e.UpdateGroupingPolicyAsync(["non_exist", "data4_admin"],
            ["non_exist2", "data4_admin"]);
        Assert.False(res);
        TestGetUsers(e, "data4_admin", ["admin"]);

        res = await e.UpdateGroupingPoliciesAsync(
            [
                ["eve", "admin"],
                ["admin", "data4_admin"]
            ],
            [
                ["eve", "admin_groups"],
                ["admin", "data5_admin"]
            ]);

        Assert.True(res);
        TestGetUsers(e, "data5_admin", ["admin"]);
        TestGetUsers(e, "admin_groups", ["eve"]);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);

        res = await e.UpdateGroupingPoliciesAsync(
            [
                ["admin", "data5_admin"],
                ["non_exist", "admin_groups"]
            ],
            [
                ["admin", "data6_admin"],
                ["non_exist2", "admin_groups"]
            ]);
        Assert.False(res);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);

        // If oldRules' length is not the same as newRules', no rules will be updated.
        res = await e.UpdateGroupingPoliciesAsync(
            [
                ["admin", "data5_admin"],
                ["eve", "admin2_groups"]
            ],
            [["admin", "data6_admin"]]);
        Assert.False(res);
        TestGetRoles(e, "admin", ["data5_admin"]);
        TestGetRoles(e, "eve", ["admin_groups"]);
    }

    [Fact]
    public void TestModifySpecialPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.RbacModelText));

        e.AddPolicy("alice", "data1");
        e.AddPolicy("alice", "data1", "read");
        e.AddPolicy("alice", "data1", "read", "dump1");

        e.TestGetPolicy(Policy.ValuesListFrom(new[]
            {
                Policy.CreateValues("alice", "data1", ""), Policy.CreateValues("alice", "data1", "read")
            }
        ));
    }

    [Fact]
    public async Task TestModifySpecialPolicyAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.ClearPolicy();

        await e.AddPolicyAsync("alice", "data1");
        await e.AddPolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("alice", "data1", "read", "dump1");

        e.TestGetPolicy(Policy.ValuesListFrom(new[]
            {
                Policy.CreateValues("alice", "data1", ""), Policy.CreateValues("alice", "data1", "read")
            }
        ));
    }
}
