using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public partial class ModelTest
{
    private readonly TestModelFixture _testModelFixture;
    public ModelTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    [Fact]
    public void TestBasicModel()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestBasicModelNoPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.BasicModelText));

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.False(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestBasicModelWithRoot()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.BasicWithRootModelText,
            TestModelFixture.BasicPolicyText));

         Assert.True(e.Enforce("alice", "data1", "read"));
         Assert.False(e.Enforce("alice", "data1", "write"));
         Assert.False(e.Enforce("alice", "data2", "read"));
         Assert.False(e.Enforce("alice", "data2", "write"));
         Assert.False(e.Enforce("bob", "data1", "read"));
         Assert.False(e.Enforce("bob", "data1", "write"));
         Assert.False(e.Enforce("bob", "data2", "read"));
         Assert.True(e.Enforce("bob", "data2", "write"));
         Assert.True(e.Enforce("root", "data1", "read"));
         Assert.True(e.Enforce("root", "data1", "write"));
         Assert.True(e.Enforce("root", "data2", "read"));
         Assert.True(e.Enforce("root", "data2", "write"));
    }

    [Fact]
    public void TestBasicModelWithRootNoPolicy()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.BasicWithRootModelText));

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.False(e.Enforce("bob", "data2", "write"));
        Assert.True(e.Enforce("root", "data1", "read"));
        Assert.True(e.Enforce("root", "data1", "write"));
        Assert.True(e.Enforce("root", "data2", "read"));
        Assert.True(e.Enforce("root", "data2", "write"));
    }

    [Fact]
    public void TestBasicModelWithoutUsers()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutUserTestModel());

        Assert.True(e.Enforce("data1", "read"));
        Assert.False(e.Enforce("data1", "write"));
        Assert.False(e.Enforce("data2", "read"));
        Assert.True(e.Enforce("data2", "write"));
    }

    [Fact]
    public void TestBasicModelWithoutResources()
    {
        Enforcer e = new(TestModelFixture.GetBasicWithoutResourceTestModel());

        Assert.True(e.Enforce("alice", "read"));
        Assert.False(e.Enforce("alice", "write"));
        Assert.False(e.Enforce("bob", "read"));
        Assert.True(e.Enforce("bob", "write"));
    }

    [Fact]
    public void TestRbacModel()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithResourceRoles()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithResourceRoleTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "domain1", "data1", "read"));
        Assert.True(e.Enforce("alice", "domain1", "data1", "write"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "write"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "read"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "write"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "read"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithDomainsAtRuntime()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.RbacWithDomainsModelText));
        e.BuildRoleLinks();

        e.AddPolicy("admin", "domain1", "data1", "read");
        e.AddPolicy("admin", "domain1", "data1", "write");
        e.AddPolicy("admin", "domain2", "data2", "read");
        e.AddPolicy("admin", "domain2", "data2", "write");

        e.AddGroupingPolicy("alice", "admin", "domain1");
        e.AddGroupingPolicy("bob", "admin", "domain2");

        Assert.True(e.Enforce("alice", "domain1", "data1", "read"));
        Assert.True(e.Enforce("alice", "domain1", "data1", "write"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "write"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "read"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "write"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "read"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "write"));

        // Remove all policy rules related to domain1 and data1.
        e.RemoveFilteredPolicy(1, "domain1", "data1");

        Assert.False(e.Enforce("alice", "domain1", "data1", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data1", "write"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "write"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "read"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "write"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "read"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "write"));

        // Remove the specified policy rule.
        e.RemovePolicy("admin", "domain2", "data2", "read");

        Assert.False(e.Enforce("alice", "domain1", "data1", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data1", "write"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce("alice", "domain1", "data2", "write"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "read"));
        Assert.False(e.Enforce("bob", "domain2", "data1", "write"));
        Assert.False(e.Enforce("bob", "domain2", "data2", "read"));
        Assert.True(e.Enforce("bob", "domain2", "data2", "write"));
    }

    [Fact]
    public async Task TestRbacModelWithDomainsAtRuntimeAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(TestModelFixture.RbacWithDomainsModelText));
        e.BuildRoleLinks();

        await e.AddPolicyAsync("admin", "domain1", "data1", "read");
        await e.AddPolicyAsync("admin", "domain1", "data1", "write");
        await e.AddPolicyAsync("admin", "domain2", "data2", "read");
        await e.AddPolicyAsync("admin", "domain2", "data2", "write");

        await e.AddGroupingPolicyAsync("alice", "admin", "domain1");
        await e.AddGroupingPolicyAsync("bob", "admin", "domain2");

        Assert.True(await e.EnforceAsync("alice", "domain1", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "domain1", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "write"));
        Assert.True(await e.EnforceAsync("bob", "domain2", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "domain2", "data2", "write"));

        // Remove all policy rules related to domain1 and data1.
        await e.RemoveFilteredPolicyAsync(1, "domain1", "data1");

        Assert.False(await e.EnforceAsync("alice", "domain1", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "write"));
        Assert.True(await e.EnforceAsync("bob", "domain2", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "domain2", "data2", "write"));

        // Remove the specified policy rule.
        await e.RemovePolicyAsync("admin", "domain2", "data2", "read");

        Assert.False(await e.EnforceAsync("alice", "domain1", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "domain1", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "domain2", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "domain2", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithDeny()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDenyTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithOnlyDeny()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithNotDenyModelText,
            TestModelFixture.RbacWithDenyPolicyText));
        e.BuildRoleLinks();

        Assert.False(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithCustomData()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
        // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
        // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
        e.AddGroupingPolicy("bob", "data2_admin", "custom_data");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.True(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        // You should also take the custom data as a parameter when deleting a grouping policy.
        // e.removeGroupingPolicy("bob", "data2_admin") won't work.
        // Or you can remove it by using removeFilteredGroupingPolicy().
        e.RemoveGroupingPolicy("bob", "data2_admin", "custom_data");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestRbacModelWithCustomDataAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.BuildRoleLinks();

        // You can add custom data to a grouping policy, Casbin will ignore it. It is only meaningful to the caller.
        // This feature can be used to store information like whether "bob" is an end user (so no subject will inherit "bob")
        // For Casbin, it is equivalent to: e.addGroupingPolicy("bob", "data2_admin")
        await e.AddGroupingPolicyAsync("bob", "data2_admin", "custom_data");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.True(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));

        // You should also take the custom data as a parameter when deleting a grouping policy.
        // e.removeGroupingPolicy("bob", "data2_admin") won't work.
        // Or you can remove it by using removeFilteredGroupingPolicy().
        await e.RemoveGroupingPolicyAsync("bob", "data2_admin", "custom_data");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.True(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestRbacModelWithCustomRoleManager()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacTestModel());
        e.SetRoleManager(new MockCustomRoleManager());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestAbacModel()
    {
        Enforcer e = new(TestModelFixture.GetNewAbacModel());

        TestResource data1 = new("data1", "alice");
        TestResource data2 = new("data2", "bob");

        Assert.True(e.Enforce("alice", data1, "read"));
        Assert.True(e.Enforce("alice", data1, "write"));
        Assert.False(e.Enforce("alice", data2, "read"));
        Assert.False(e.Enforce("alice", data2, "write"));
        Assert.False(e.Enforce("bob", data1, "read"));
        Assert.False(e.Enforce("bob", data1, "write"));
        Assert.True(e.Enforce("bob", data2, "read"));
        Assert.True(e.Enforce("bob", data2, "write"));
    }

    [Fact]
    public void TestAbacWithEvalModel()
    {
        Enforcer e = new(TestModelFixture.GetNewAbacWithEvalModel());
        TestSubject subject1 = new("alice", 16);
        TestSubject subject2 = new("alice", 20);
        TestSubject subject3 = new("alice", 65);

        Assert.False(e.Enforce(subject1, "/data1", "read"));
        Assert.False(e.Enforce(subject1, "/data2", "read"));
        Assert.False(e.Enforce(subject1, "/data1", "write"));
        Assert.True(e.Enforce(subject1, "/data2", "write"));

        Assert.True(e.Enforce(subject2, "/data1", "read"));
        Assert.False(e.Enforce(subject2, "/data2", "read"));
        Assert.False(e.Enforce(subject2, "/data1", "write"));
        Assert.True(e.Enforce(subject2, "/data2", "write"));

        Assert.True(e.Enforce(subject3, "/data1", "read"));
        Assert.False(e.Enforce(subject3, "/data2", "read"));
        Assert.False(e.Enforce(subject3, "/data1", "write"));
        Assert.False(e.Enforce(subject3, "/data2", "write"));
    }

    [Fact]
    public void TestKeyMatchModel()
    {
        Enforcer e = new(TestModelFixture.GetNewKeyMatchTestModel());

        Assert.True(e.Enforce("alice", "/alice_data/resource1", "GET"));
        Assert.True(e.Enforce("alice", "/alice_data/resource1", "POST"));
        Assert.True(e.Enforce("alice", "/alice_data/resource2", "GET"));
        Assert.False(e.Enforce("alice", "/alice_data/resource2", "POST"));
        Assert.False(e.Enforce("alice", "/bob_data/resource1", "GET"));
        Assert.False(e.Enforce("alice", "/bob_data/resource1", "POST"));
        Assert.False(e.Enforce("alice", "/bob_data/resource2", "GET"));
        Assert.False(e.Enforce("alice", "/bob_data/resource2", "POST"));

        Assert.False(e.Enforce("bob", "/alice_data/resource1", "GET"));
        Assert.False(e.Enforce("bob", "/alice_data/resource1", "POST"));
        Assert.True(e.Enforce("bob", "/alice_data/resource2", "GET"));
        Assert.False(e.Enforce("bob", "/alice_data/resource2", "POST"));
        Assert.False(e.Enforce("bob", "/bob_data/resource1", "GET"));
        Assert.True(e.Enforce("bob", "/bob_data/resource1", "POST"));
        Assert.False(e.Enforce("bob", "/bob_data/resource2", "GET"));
        Assert.True(e.Enforce("bob", "/bob_data/resource2", "POST"));

        Assert.True(e.Enforce("cathy", "/cathy_data", "GET"));
        Assert.True(e.Enforce("cathy", "/cathy_data", "POST"));
        Assert.False(e.Enforce("cathy", "/cathy_data", "DELETE"));
    }

    [Fact]
    public void TestPriorityModelIndeterminate()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.PriorityModelText,
            TestModelFixture.PriorityIndeterminatePolicyText));
        e.BuildRoleLinks();

        Assert.False(e.Enforce("alice", "data1", "read"));
    }

    [Fact]
    public void TestPriorityModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.True(e.Enforce("bob", "data2", "read"));
        Assert.False(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestPriorityExplicitModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityExplicitTestModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
        Assert.False(e.Enforce("data1_deny_group", "data1", "read"));
        Assert.False(e.Enforce("data1_deny_group", "data1", "write"));
        Assert.True(e.Enforce("data2_allow_group", "data2", "read"));
        Assert.True(e.Enforce("data2_allow_group", "data2", "write"));

        // add a higher priority policy
        e.AddPolicy("1", "bob", "data2", "write", "deny");

        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.False(e.Enforce("bob", "data2", "write"));
        Assert.False(e.Enforce("data1_deny_group", "data1", "read"));
        Assert.False(e.Enforce("data1_deny_group", "data1", "write"));
        Assert.True(e.Enforce("data2_allow_group", "data2", "read"));
        Assert.True(e.Enforce("data2_allow_group", "data2", "write"));
    }

    [Fact]
    public void TestPriorityExplicitDenyOverrideModel()
    {
        Enforcer e = new(TestModelFixture.GetNewPriorityExplicitDenyOverrideModel());
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.True(e.Enforce("bob", "data2", "read"));

        // adding a new group, simulating behaviour when two different groups are added to the same person.
        e.AddPolicy("10", "data2_deny_group_new", "data2", "write", "deny");
        e.AddGroupingPolicy("alice", "data2_deny_group_new");

        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.True(e.Enforce("bob", "data2", "read"));

        // expected enforcement result should be true,
        // as there is a policy with a lower rank 10, that produces allow result.
        e.AddPolicy("5", "alice", "data2", "write", "allow");
        Assert.True(e.Enforce("alice", "data2", "write"));

        // adding deny policy for alice for the same obj,
        // to ensure that if there is at least one deny, final result will be deny.
        e.AddPolicy("5", "alice", "data2", "write", "deny");
        Assert.False(e.Enforce("alice", "data2", "write"));

        // adding higher fake higher priority policy for alice,
        // expected enforcement result should be true (ignore this policy).
        e.AddPolicy("2", "alice", "data2", "write", "allow");
        Assert.True(e.Enforce("alice", "data2", "write"));

        e.AddPolicy("1", "fake-subject", "fake-object", "very-fake-action", "allow");
        Assert.True(e.Enforce("alice", "data2", "write"));

        // adding higher (less of 0) priority policy for alice,
        // to override group policies again.
        e.AddPolicy("-1", "alice", "data2", "write", "deny");
        Assert.False(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public void TestKeyMatch2Model()
    {
        Enforcer e = new(TestModelFixture.GetNewKeyMatch2TestModel());

        Assert.False(e.Enforce("alice", "/alice_data", "GET"));
        Assert.True(e.Enforce("alice", "/alice_data/resource1", "GET"));
        Assert.False(e.Enforce("alice", "/alice_data2/myid", "GET"));
        Assert.True(e.Enforce("alice", "/alice_data2/myid/using/res_id", "GET"));
    }

    [Fact]
    public void TestKeyMatchCustomModel()
    {
        static bool CustomFunction(string key1, string key2)
        {
            return (key1 is "/alice_data2/myid/using/res_id" && key2 is "/alice_data/:resource")
                   || (key1 is "/alice_data2/myid/using/res_id" && key2 is "/alice_data2/:id/using/:resId");
        }

        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.KeyMatchCustomModelText,
            TestModelFixture.KeyMatch2PolicyText));

        e.AddFunction("keyMatchCustom", CustomFunction);

        Assert.False(e.Enforce("alice", "/alice_data2/myid", "GET"));
        Assert.True(e.Enforce("alice", "/alice_data2/myid/using/res_id", "GET"));
    }

    [Fact]
    public void TestMultipleTypeModel()
    {
        Enforcer e = new(TestModelFixture.GetNewMultipleTypeTestModel());
        e.BuildRoleLinks();

        // Use default types
        EnforceContext context = e.CreateContext();

        Assert.True(e.Enforce(context, "alice", "data1", "read"));
        Assert.False(e.Enforce(context, "alice", "data1", "write"));

        Assert.True(e.Enforce(context, "bob", "data2", "read"));
        Assert.False(e.Enforce(context, "bob", "data2", "write"));

        // Use r2 p2 and m2 type
        context = e.CreateContext
        (
            PermConstants.RequestType2,
            PermConstants.PolicyType2,
            PermConstants.DefaultPolicyEffectType,
            PermConstants.MatcherType2
        );

        Assert.True(e.Enforce(context, "alice", "domain1", "data2", "read"));
        Assert.False(e.Enforce(context, "alice", "domain1", "data2", "write"));

        Assert.True(e.Enforce(context, "bob", "domain1", "data1", "read"));
        Assert.False(e.Enforce(context, "bob", "domain1", "data1", "write"));

        // Use r3 p3 and m3 type
        context = e.CreateContext
        (
            PermConstants.RequestType3,
            PermConstants.PolicyType3,
            PermConstants.DefaultPolicyEffectType,
            PermConstants.MatcherType3
        );

        Assert.True(e.Enforce(context, new { Age = 30 }, "data2", "read"));
        Assert.False(e.Enforce(context, new { Age = 70 }, "data2", "read"));
    }

    [Fact]
    public void TestOnlyNonDefaultTypeModel()
    {
        IModel model = DefaultModel.Create();
        model.AddDef("r", "r2", "sub, obj, act");
        model.AddDef("p", "p2", "sub, obj, act");
        model.AddDef("g", "g", "_, _");
        model.AddDef("e", "e2", "some(where (p.eft == allow))");
        model.AddDef("m", "m2", "g(r2.sub, p2.sub) && r2.obj == p2.obj && r2.act == p2.act");

        Enforcer e = new(model);
        e.AddNamedPolicy(PermConstants.PolicyType2, "alice", "data1", "read");
        e.AddNamedPolicy(PermConstants.PolicyType2, "bob", "data2", "write");
        e.BuildRoleLinks();

        EnforceContext context = e.CreateContext
        (
            PermConstants.RequestType2,
            PermConstants.PolicyType2,
            PermConstants.PolicyEffectType2,
            PermConstants.MatcherType2
        );

        Assert.True(e.Enforce(context, "alice", "data1", "read"));
        Assert.False(e.Enforce(context, "alice", "data1", "write"));
        Assert.True(e.Enforce(context, "bob", "data2", "write"));
        Assert.False(e.Enforce(context, "bob", "data2", "read"));
    }

    [Fact]
    public void TestAbacComment()
    {
        var model = TestModelFixture.GetNewTestModel(TestModelFixture.AbacCommentText);
        Assert.Equal(3, model.Sections.GetRequestAssertion("r")?.Tokens.Count);
        Assert.Equal(2, model.Sections.GetRequestAssertion("r")?.Tokens["act"]);
        Assert.Equal(3, model.Sections.GetPolicyAssertion("p").Tokens.Count);
        Assert.Equal("some(where (p.eft == allow))", model.Sections.GetPolicyEffectAssertion("e").Value);
        Assert.Equal("r.sub == p.sub && r.obj == p.obj && r.act == p.act",
            model.Sections.GetMatcherAssertion("m").Value);
    }

    [Fact]
    public void TestRbacComment()
    {
        var model = TestModelFixture.GetNewTestModel(TestModelFixture.RbacCommentText);
        Assert.Equal(3, model.Sections.GetRequestAssertion("r")?.Tokens.Count);
        Assert.Equal(2, model.Sections.GetRequestAssertion("r")?.Tokens["act"]);
        Assert.Equal(3, model.Sections.GetPolicyAssertion("p").Tokens.Count);
        Assert.Equal("_, _", model.Sections.GetRoleAssertion("g").Value);
        Assert.Equal("some(where (p.eft == allow))", model.Sections.GetPolicyEffectAssertion("e").Value);
        Assert.Equal("g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act",
            model.Sections.GetMatcherAssertion("m").Value);
    }

    [Fact]
    public void TestModelWithCommaAndQuotations()
    {
        Enforcer e = new Enforcer(TestModelFixture.GetNewCommaAndQuotationsModel());

        Assert.True(e.Enforce("alice", "Comma,Test", "Get"));
        Assert.False(e.Enforce("alice", "Comma,Test", "Post"));
        Assert.False(e.Enforce("alice", "\"Comma,Test\"", "Get"));
        Assert.True(e.Enforce("bob", "\"Comma\",\"Quotations\",Test", "Get"));
        Assert.False(e.Enforce("bob", "\"Comma\",\"Quotations\",Test", "Post"));
        Assert.False(e.Enforce("bob", "\"\"Comma\"\",\"\"Quotations\"\",Test", "Get"));
        Assert.False(e.Enforce("bob", "\"\"\"Comma\"\",\"\"Quotations\"\",Test\"", "Get"));
        Assert.True(e.Enforce("cindy", "\"Muti Quotations Test", "Get"));
        Assert.False(e.Enforce("cindy", "\"Muti Quotations Test", "Post"));
        Assert.False(e.Enforce("cindy", "\"\"Muti Quotations Test", "Get"));
        Assert.False(e.Enforce("cindy", "\"\"Muti Quotations Test\"", "Get"));
    }

    [Fact]
    public void TestModelWithTabs()
    {
        Enforcer e = new Enforcer(TestModelFixture.GetNewTabsModel());
        e.AddRoleForUserInDomain("/user/john", "admin", "/tenant/1");
        Assert.True(e.Enforce("/user/john", "/tenant/1", "/tenant/1/resource", "Write"));
    }

    [Fact]
    public void TestRbacTokensWithSubstringRelation()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacTokensWithSubstringRelationModelText,
            TestModelFixture.RbacTokensWithSubstringRelationPolicyText));
        e.BuildRoleLinks();

        Assert.True(e.Enforce("alice", "tenant1", "data1", "read"));
        Assert.True(e.Enforce("alice", "tenant1", "freeread", "read"));
        Assert.False(e.Enforce("alice", "tenant2", "data2", "read"));
        Assert.False(e.Enforce("alice", "tenant1", "data1", "write"));
        Assert.False(e.Enforce("bob", "tenant1", "data1", "read"));
        Assert.False(e.Enforce("alice", "tenant3", "freeread", "read"));
        Assert.False(e.Enforce("alice", "tenant1", "freeread", "write"));
    }

    [Fact]
    public void TestAbacTokensWithSubstringRelation()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacTokensWithSubstringRelationModelText,
            TestModelFixture.AbacTokensWithSubstringRelationPolicyText));

        TestResource data1 = new("data1", "alice");
        TestResource data2 = new("data2", "bob");
        TestSubject subjecta = new("alice", 16);
        TestSubject subjectb = new("bob", 65);
        TestSubject subjectc = new("candy", 30);
        TestSubject subjectd = new("donale", -1);
        TestSubject subjecte = new("eleena", 1000000009);

        Assert.True(e.Enforce(subjecta, data1, "read"));
        Assert.True(e.Enforce(subjectb, data2, "write"));
        Assert.True(e.Enforce(subjectc, data1, "read"));
        Assert.True(e.Enforce(subjectc, data2, "write"));
        Assert.True(e.Enforce(subjecta, data2, "write"));
        Assert.True(e.Enforce(subjectb, data1, "read"));

        Assert.True(e.Enforce(subjecta, data1, "write"));
        Assert.True(e.Enforce(subjectb, data2, "read"));

        Assert.False(e.Enforce(subjectc, data1, "write"));
        Assert.False(e.Enforce(subjectc, data2, "read"));
        Assert.False(e.Enforce(subjectd, data1, "read"));
        Assert.False(e.Enforce(subjecte, data2, "write"));
    }

    [Fact]
    public void TestBackslashLineFeed()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.BackslashLineFeedModelText,
            TestModelFixture.BackslashLineFeedPolicyText));

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public void TestAccidentalCacheRead()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("aliced", "ata1", "read"));
        Assert.False(e.Enforce("alice", "data", "1read"));
    }

    [Fact]
    public void TestRbacWithIndexMatcher()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithIndexMatcherModelText,
            TestModelFixture.RbacWithIndexMatcherPolicyText));
        e.BuildRoleLinks();
        var rule = new Dictionary<string, Dictionary<string, string>>
        {
            ["CompanyData"] = new() { ["CompanyIsActive"] = "True", ["BusinessRole"] = "Role1" }
        };
        Assert.True(e.Enforce(rule, "WebApp", "/api/transactions/getTransactions", "POST"));
        Assert.False(e.Enforce(rule, "Admin", "/api/transactions/getTransactions", "POST"));

        rule = new Dictionary<string, Dictionary<string, string>>
        {
            ["CompanyData"] = new() { ["CompanyIsActive"] = "False", ["BusinessRole"] = "Role1" }
        };
        Assert.False(e.Enforce(rule, "WebApp", "/api/transactions/getTransactions", "POST"));
        Assert.True(e.Enforce(rule, "Admin", "/api/transactions/getTransactions", "POST"));
    }

    [Fact]
    public void TestAbacWithDynamicValueType()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacWithDynamicValueTypeModelText,
            TestModelFixture.AbacWithDynamicValueTypePolicyText));
        var sub = new { Name = "bob" };
        var obj1 = new { Object = "/data1", Property1 = "prop-1" };
        var obj2 = new { Object = "/data2", Property2 = "prop-2" };
        var obj3 = new { Object = "/data2", Property3 = "prop-3" };
        Assert.True(e.Enforce(sub, obj1, "read"));
        Assert.True(e.Enforce(sub, obj2, "read"));
        Assert.False(e.Enforce(sub, obj3, "read"));
        // Request again to test the cache hit logic.
        Assert.True(e.Enforce(sub, obj1, "read"));
        Assert.True(e.Enforce(sub, obj2, "read"));
        Assert.False(e.Enforce(sub, obj3, "read"));
    }

#if !NET452 && !NET461 && !NET462 && !NETSTANDARD
    [Fact]
    public void TestAbacJsonRequest()
    {
        Enforcer e = new(TestModelFixture.GetNewAbacModel());

        JsonValue data1Json = new JsonValue("""{ "Name": "data1", "Owner": "alice"}""");
        JsonValue data2Json = new JsonValue("""{ "Name": "data2", "Owner": "bob"}""");

        Assert.True(e.Enforce("alice", data1Json, "read"));
        Assert.True(e.Enforce("alice", data1Json, "write"));
        Assert.False(e.Enforce("alice", data2Json, "read"));
        Assert.False(e.Enforce("alice", data2Json, "write"));
        Assert.False(e.Enforce("bob", data1Json, "read"));
        Assert.False(e.Enforce("bob", data1Json, "write"));
        Assert.True(e.Enforce("bob", data2Json, "read"));
        Assert.True(e.Enforce("bob", data2Json, "write"));

        e = new Enforcer(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacNotUsingPolicyModelText,
            TestModelFixture.AbacRuleEffectPolicyText));

        Assert.True(e.Enforce("alice", data1Json, "read"));
        Assert.True(e.Enforce("alice", data1Json, "write"));
        Assert.False(e.Enforce("alice", data2Json, "read"));
        Assert.False(e.Enforce("alice", data2Json, "write"));

        e = new Enforcer(TestModelFixture.GetNewTestModel(
            TestModelFixture.AbacRuleModelText,
            TestModelFixture.AbacRulePolicyText));
        JsonValue sub1Json = new JsonValue("""{ "Name": "alice", "Age": 16}""");
        JsonValue sub2Json = new JsonValue("""{ "Name": "alice", "Age": 20}""");
        JsonValue sub3Json = new JsonValue("""{ "Name": "alice", "Age": 65}""");

        Assert.False(e.Enforce(sub1Json, "/data1", "read"));
        Assert.False(e.Enforce(sub1Json, "/data2", "read"));
        Assert.False(e.Enforce(sub1Json, "/data1", "write"));
        Assert.True(e.Enforce(sub1Json, "/data2", "write"));
        Assert.True(e.Enforce(sub2Json, "/data1", "read"));
        Assert.False(e.Enforce(sub2Json, "/data2", "read"));
        Assert.False(e.Enforce(sub2Json, "/data1", "write"));
        Assert.True(e.Enforce(sub2Json, "/data2", "write"));
        Assert.True(e.Enforce(sub3Json, "/data1", "read"));
        Assert.False(e.Enforce(sub3Json, "/data2", "read"));
        Assert.False(e.Enforce(sub3Json, "/data1", "write"));
        Assert.False(e.Enforce(sub3Json, "/data2", "write"));
    }
#endif
}
