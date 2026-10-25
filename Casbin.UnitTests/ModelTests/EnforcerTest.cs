using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Casbin.Persist.Adapter.Stream;
using Casbin.Rbac;
using Casbin.UnitTests.Fixtures;
using Casbin.UnitTests.Mock;
using Xunit;
using Xunit.Abstractions;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class EnforcerTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public EnforcerTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void TestEnforceWithMultipleRoleManager()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacMultipleModelText,
            TestModelFixture.RbacMultiplePolicyText));

        DefaultRoleManager roleManager = new(5);
        roleManager.AddMatchingFunc((arg1, arg2) => arg1.Equals(arg2));
        e.SetRoleManager(roleManager);
        bool result = e.Enforce("@adm-user", "org::customer1", "cust1", "manage");
        Assert.True(result);

        roleManager.AddMatchingFunc((arg1, arg2) => !arg1.Equals(arg2));
        e.SetRoleManager(roleManager);
        result = e.Enforce("@adm-user", "org::customer1", "cust1", "manage");
        Assert.False(result);
    }

    [Fact]
    public void TestEnforceWithMultipleEval()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacMultipleEvalModelText,
            TestModelFixture.RbacMultipleEvalPolicyText));

        bool result = e.Enforce(
            "domain1",
            new { Role = "admin" },
            new { Name = "admin_panel" },
            "view");

        Assert.True(result);
    }

    [Fact]
    public void TestEnforceWithoutAutoLoadPolicy()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");
        IEnforcer e = new Enforcer(m, a, new EnforcerOptions { AutoLoadPolicy = false });
        Assert.Empty(e.GetPolicy());

        e = new Enforcer(m, a);
        Assert.NotEmpty(e.GetPolicy());
    }

    [Fact]
    public void TestEnforceSubjectPriority()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.SubjectPriorityModelText,
            TestModelFixture.SubjectPriorityPolicyText));

        Assert.True(e.Enforce("jane", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "read"));
    }

    [Fact]
    public void TestEnforceSubjectPriorityWithDomain()
    {
        Enforcer e = new(
            Path.Combine("Examples", "subject_priority_model_with_domain.conf"),
            Path.Combine("Examples", "subject_priority_policy_with_domain.csv"));

        Assert.True(e.Enforce("alice", "data1", "domain1", "write"));
        Assert.True(e.Enforce("bob", "data2", "domain2", "write"));
    }

    #region In memory model

    [Fact]
    public void TestKeyMatchModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");
        Enforcer e = new(m, a);

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

        Assert.True(e.Enforce("cathy", "/cathy_data", "GET"));
        Assert.True(e.Enforce("cathy", "/cathy_data", "POST"));
        Assert.False(e.Enforce("cathy", "/cathy_data", "DELETE"));

        e = new Enforcer(m);
        a.LoadPolicy(e.Model);

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
    public async Task TestKeyMatchModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");
        Enforcer e = new(m, a);

        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "GET"));
        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "POST"));
        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/alice_data/resource2", "POST"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource1", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource1", "POST"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource2", "POST"));

        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource1", "GET"));
        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource1", "POST"));
        Assert.True(await e.EnforceAsync("bob", "/alice_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource2", "POST"));
        Assert.False(await e.EnforceAsync("bob", "/bob_data/resource1", "GET"));
        Assert.True(await e.EnforceAsync("bob", "/bob_data/resource1", "POST"));

        Assert.True(await e.EnforceAsync("cathy", "/cathy_data", "GET"));
        Assert.True(await e.EnforceAsync("cathy", "/cathy_data", "POST"));
        Assert.False(await e.EnforceAsync("cathy", "/cathy_data", "DELETE"));

        e = new Enforcer(m);
        await a.LoadPolicyAsync(e.Model);

        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "GET"));
        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "POST"));
        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/alice_data/resource2", "POST"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource1", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource1", "POST"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("alice", "/bob_data/resource2", "POST"));

        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource1", "GET"));
        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource1", "POST"));
        Assert.True(await e.EnforceAsync("bob", "/alice_data/resource2", "GET"));
        Assert.False(await e.EnforceAsync("bob", "/alice_data/resource2", "POST"));
        Assert.False(await e.EnforceAsync("bob", "/bob_data/resource1", "GET"));
        Assert.True(await e.EnforceAsync("bob", "/bob_data/resource1", "POST"));
        Assert.False(await e.EnforceAsync("bob", "/bob_data/resource2", "GET"));
        Assert.True(await e.EnforceAsync("bob", "/bob_data/resource2", "POST"));

        Assert.True(await e.EnforceAsync("cathy", "/cathy_data", "GET"));
        Assert.True(await e.EnforceAsync("cathy", "/cathy_data", "POST"));
        Assert.False(await e.EnforceAsync("cathy", "/cathy_data", "DELETE"));
    }

    [Fact]
    public void TestKeyMatchModelInMemoryDeny()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "!some(where (p.eft == deny))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");
        Enforcer e = new(m, a);

        Assert.True(e.Enforce("alice", "/alice_data/resource2", "POST"));
    }

    [Fact]
    public void TestInOperator()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacInOperatorModelText,
            TestModelFixture.RbacInOperatorPolicyText));

        Assert.True(e.Enforce(
            new { Name = "Alice", Amount = 5100, Roles = new[] { "Manager", "DepartmentDirector" } },
            "authorization", "grant"));
        Assert.False(e.Enforce(
            new { Name = "Alice", Amount = 5100, Roles = new[] { "DepartmentDirector" } },
            "authorization", "grant"));
    }

    [Fact]
    public void TestRbacModelInMemoryIndeterminate()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);
        e.AddPermissionForUser("alice", "data1", "invalid");
        Assert.False(e.Enforce("alice", "data1", "read"));
    }

    [Fact]
    public async Task TestRbacModelInMemoryIndeterminateAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);
        await e.AddPermissionForUserAsync("alice", "data1", "invalid");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));
    }

    [Fact]
    public void TestRbacModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

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
    public async Task TestRbacModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "read");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "write");
        await e.AddRoleForUserAsync("alice", "data2_admin");

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
    public void TestRbacBatchEnforceInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), true),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), true),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), false),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), true)
        ];

        e.TestBatchEnforce(testCases);
    }

    [Fact]
    public void TestRbacParallelBatchEnforceInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), true),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), true),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), false),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), true)
        ];

        TestParallelBatchEnforce(e, testCases);
    }

    [Fact]
    public void TestRbacBatchEnforceInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUserAsync("alice", "data1", "read");
        e.AddPermissionForUserAsync("bob", "data2", "write");
        e.AddPermissionForUserAsync("data2_admin", "data2", "read");
        e.AddPermissionForUserAsync("data2_admin", "data2", "write");
        e.AddRoleForUserAsync("alice", "data2_admin");

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), true),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), true),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), false),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), true)
        ];

        TestBatchEnforceAsync(e, testCases);
    }

    [Fact]
    public void TestRbacModelInMemory2()
    {
        string text =
            "[request_definition]\n"
            + "r = sub, obj, act\n"
            + "\n"
            + "[policy_definition]\n"
            + "p = sub, obj, act\n"
            + "\n"
            + "[role_definition]\n"
            + "g = _, _\n"
            + "\n"
            + "[policy_effect]\n"
            + "e = some(where (p.eft == allow))\n"
            + "\n"
            + "[matchers]\n"
            + "m = g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act\n";

        IModel m = DefaultModel.CreateFromText(text);
        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");
        e.AddPermissionForUser("data2_admin", "data2", "read");
        e.AddPermissionForUser("data2_admin", "data2", "write");
        e.AddRoleForUser("alice", "data2_admin");

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
    public async Task TestRbacModelInMemory2Async()
    {
        string text =
            "[request_definition]\n"
            + "r = sub, obj, act\n"
            + "\n"
            + "[policy_definition]\n"
            + "p = sub, obj, act\n"
            + "\n"
            + "[role_definition]\n"
            + "g = _, _\n"
            + "\n"
            + "[policy_effect]\n"
            + "e = some(where (p.eft == allow))\n"
            + "\n"
            + "[matchers]\n"
            + "m = g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act\n";

        IModel m = DefaultModel.CreateFromText(text);

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "read");
        await e.AddPermissionForUserAsync("data2_admin", "data2", "write");
        await e.AddRoleForUserAsync("alice", "data2_admin");

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
    public void TestNotUsedRbacModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        e.AddPermissionForUser("alice", "data1", "read");
        e.AddPermissionForUser("bob", "data2", "write");

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
    public async Task TestNotUsedRbacModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && r.obj == p.obj && r.act == p.act");

        Enforcer e = new(m);

        await e.AddPermissionForUserAsync("alice", "data1", "read");
        await e.AddPermissionForUserAsync("bob", "data2", "write");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestMultipleGroupTypeModelInMemory()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("g", "g2", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && g2(r.obj, p.obj) && r.act == p.act");

        Enforcer e = new(m);
        e.AddPolicy("alice", "data1", "read");
        e.AddPolicy("bob", "data2", "write");
        e.AddPolicy("data_group_admin", "data_group", "write");
        e.AddNamedGroupingPolicy("g", "alice", "data_group_admin");
        e.AddNamedGroupingPolicy("g2", "data1", "data_group");
        e.AddNamedGroupingPolicy("g2", "data2", "data_group");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
    }

    [Fact]
    public async Task TestNonGNamedMultipleGroupTypeModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "firstGroup", "_, _");
        m.AddDef("g", "secondGroup", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "firstGroup(r.sub, p.sub) && secondGroup(r.obj, p.obj) && r.act == p.act");

        Enforcer e = new(m);
        await e.AddPolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("bob", "data2", "write");
        await e.AddPolicyAsync("data_group_admin", "data_group", "write");
        await e.AddNamedGroupingPolicyAsync("firstGroup", "alice", "data_group_admin");
        await e.AddNamedGroupingPolicyAsync("secondGroup", "data1", "data_group");
        await e.AddNamedGroupingPolicyAsync("secondGroup", "data2", "data_group");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
    }

    [Fact]
    public async Task TestMultipleGroupTypeModelInMemoryAsync()
    {
        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("g", "g", "_, _");
        m.AddDef("g", "g2", "_, _");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "g(r.sub, p.sub) && g2(r.obj, p.obj) && r.act == p.act");

        Enforcer e = new(m);
        await e.AddPolicyAsync("alice", "data1", "read");
        await e.AddPolicyAsync("bob", "data2", "write");
        await e.AddPolicyAsync("data_group_admin", "data_group", "write");
        await e.AddNamedGroupingPolicyAsync("g", "alice", "data_group_admin");
        await e.AddNamedGroupingPolicyAsync("g2", "data1", "data_group");
        await e.AddNamedGroupingPolicyAsync("g2", "data2", "data_group");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.True(await e.EnforceAsync("alice", "data2", "write"));
    }

    #endregion

    #region Init enmpty

    [Fact]
    public void TestInitEmpty()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");

        e.SetModel(m);
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "/alice_data/resource1", "GET"));
    }

    [Fact]
    public async Task TestInitEmptyAsync()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        FileAdapter a = new("Examples/keymatch_policy.csv");

        e.SetModel(m);
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "GET"));
    }

    [Fact]
    public void TestInitEmptyByInputStream()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        using (FileStream fs = new("Examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read,
                   FileShare.ReadWrite))
        {
            StreamAdapter a = new(fs);
            e.SetModel(m);
            e.SetAdapter(a);
            e.LoadPolicy();

            Assert.True(e.Enforce("alice", "/alice_data/resource1", "GET"));
        }
    }

    [Fact]
    public async Task TestInitEmptyByInputStreamAsync()
    {
        Enforcer e = new();

        IModel m = DefaultModel.Create();
        m.AddDef("r", "r", "sub, obj, act");
        m.AddDef("p", "p", "sub, obj, act");
        m.AddDef("e", "e", "some(where (p.eft == allow))");
        m.AddDef("m", "m", "r.sub == p.sub && keyMatch(r.obj, p.obj) && regexMatch(r.act, p.act)");

        using (FileStream fs = new("Examples/keymatch_policy.csv", FileMode.Open, FileAccess.Read,
                   FileShare.ReadWrite))
        {
            StreamAdapter a = new(fs);
            e.SetModel(m);
            e.SetAdapter(a);
            await e.LoadPolicyAsync();

            Assert.True(await e.EnforceAsync("alice", "/alice_data/resource1", "GET"));
        }
    }

    #endregion

    #region Store management

    [Fact]
    public void TestReloadPolicy()
    {
        Enforcer e = new("Examples/rbac_model.conf", "Examples/rbac_policy.csv");
        e.LoadPolicy();
        e.TestGetPolicy(
            [
                ["alice", "data1", "read"],
                ["bob", "data2", "write"],
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"]
            ]
        );
    }

    [Fact]
    public async Task TestReloadPolicyAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf", "Examples/rbac_policy.csv");
        await e.LoadPolicyAsync();
        e.TestGetPolicy(
            [
                ["alice", "data1", "read"],
                ["bob", "data2", "write"],
                ["data2_admin", "data2", "read"],
                ["data2_admin", "data2", "write"]
            ]
        );
    }

    [Fact]
    public void TestSavePolicy()
    {
        Enforcer e = new("Examples/rbac_model.conf", "Examples/rbac_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        Enforcer e = new("Examples/rbac_model.conf", "Examples/rbac_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestSavePolicyWithoutBasicModel()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");

        e.SavePolicy();
    }

    [Fact]
    public async Task TestSavePolicyWithoutBasicModelAsync()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");

        await e.SavePolicyAsync();
    }

    [Fact]
    public void TestClearPolicy()
    {
        Enforcer e = new("Examples/rbac_model.conf", "Examples/rbac_policy.csv");

        e.ClearPolicy();
    }

    #endregion

    #region Extension features

    [Fact]
    public void TestEnableEnforce()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");

        e.EnableEnforce(false);
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "write"));
        Assert.True(e.Enforce("alice", "data2", "read"));
        Assert.True(e.Enforce("alice", "data2", "write"));
        Assert.True(e.Enforce("bob", "data1", "read"));
        Assert.True(e.Enforce("bob", "data1", "write"));
        Assert.True(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        e.EnableEnforce(true);
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

#if !NET452
    [Fact]
    public void TestEnableLog()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv")
        {
            Logger = new MockLogger<Enforcer>(_testOutputHelper)
        };

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        e.Logger = null;
        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }
#endif

    [Fact]
    public void TestEnableAutoSave()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");

        e.EnableAutoSave(false);
        // Because AutoSave is disabled, the policy change only affects the policy in Casbin enforcer,
        // it doesn't affect the policy in the storage.
        e.RemovePolicy("alice", "data1", "read");
        // Reload the policy from the storage to see the effect.
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));

        e.EnableAutoSave(true);
        // Because AutoSave is enabled, the policy change not only affects the policy in Casbin enforcer,
        // but also affects the policy in the storage.
        e.RemovePolicy("alice", "data1", "read");

        // However, the file adapter doesn't implement the AutoSave feature, so enabling it has no effect at all here.

        // Reload the policy from the storage to see the effect.
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read")); // Will not be false here.
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestEnableAutoSaveAsync()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy_for_async_adapter_test.csv");

        e.EnableAutoSave(false);
        // Because AutoSave is disabled, the policy change only affects the policy in Casbin enforcer,
        // it doesn't affect the policy in the storage.
        await e.RemovePolicyAsync("alice", "data1", "read");
        // Reload the policy from the storage to see the effect.
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));

        e.EnableAutoSave(true);
        // Because AutoSave is enabled, the policy change not only affects the policy in Casbin enforcer,
        // but also affects the policy in the storage.
        await e.RemovePolicyAsync("alice", "data1", "read");

        // However, the file adapter doesn't implement the AutoSave feature, so enabling it has no effect at all here.

        // Reload the policy from the storage to see the effect.
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read")); // Will not be false here.
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestAutoSaveGroupingPolicy()
    {
        // This test verifies that AddGroupingPolicy() respects the AutoSave flag.
        // When AutoSave is disabled, grouping policy changes should not be saved to the adapter.

        MockSingleAdapter adapter = new("Examples/rbac_policy.csv");
        Enforcer e = new("Examples/rbac_model.conf", adapter);

        // Verify initial state: alice has data2_admin role
        Assert.True(e.HasGroupingPolicy("alice", "data2_admin"));
        Assert.False(e.HasGroupingPolicy("bob", "data2_admin"));

        adapter.ClearSavedPolicies();
        e.EnableAutoSave(false);

        // Because AutoSave is disabled, the grouping policy change should only affect
        // the policy in Casbin enforcer, it should NOT call the adapter.
        e.AddGroupingPolicy("bob", "data2_admin");

        // Verify the change is in memory
        Assert.True(e.HasGroupingPolicy("bob", "data2_admin"));

        // Verify the adapter was NOT called because AutoSave is disabled
        Assert.Empty(adapter.SavedPolicies);
    }

    [Fact]
    public async Task TestAutoSaveGroupingPolicyAsync()
    {
        // This test verifies that AddGroupingPolicyAsync() respects the AutoSave flag.
        // When AutoSave is disabled, grouping policy changes should not be saved to the adapter.

        MockSingleAdapter adapter = new("Examples/rbac_policy.csv");
        Enforcer e = new("Examples/rbac_model.conf", adapter);

        // Verify initial state
        Assert.True(e.HasGroupingPolicy("alice", "data2_admin"));
        Assert.False(e.HasGroupingPolicy("bob", "data2_admin"));

        adapter.ClearSavedPolicies();
        e.EnableAutoSave(false);

        // Add grouping policy with AutoSave disabled
        await e.AddGroupingPolicyAsync("bob", "data2_admin");

        // Verify the change is in memory
        Assert.True(e.HasGroupingPolicy("bob", "data2_admin"));

        // Verify the adapter was NOT called because AutoSave is disabled
        Assert.Empty(adapter.SavedPolicies);
    }

    [Fact]
    public void TestInitWithAdapter()
    {
        FileAdapter adapter = new("Examples/basic_policy.csv");
        Enforcer e = new("Examples/basic_model.conf", adapter);

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
    public void TestRoleLinks()
    {
        Enforcer e = new("Examples/rbac_model.conf");
        e.EnableAutoBuildRoleLinks(false);
        e.BuildRoleLinks();
        e.Enforce("user501", "data9", "read");
    }

    [Fact]
    public void TestGetAndSetModel()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");
        Enforcer e2 = new("Examples/basic_with_root_model.conf", "Examples/basic_policy.csv");

        Assert.False(e.Enforce("root", "data1", "read"));

        e.SetModel(e2.Model);
        Assert.True(e.Enforce("root", "data1", "read"));
    }

    [Fact]
    public void TestGetAndSetAdapterInMem()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy.csv");
        Enforcer e2 = new("Examples/basic_model.conf", "Examples/basic_inverse_policy.csv");

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));

        IReadOnlyAdapter a2 = e2.Adapter;
        e.SetAdapter(a2);
        e.LoadPolicy();

        Assert.False(e.Enforce("alice", "data1", "read"));
        Assert.True(e.Enforce("alice", "data1", "write"));
    }

    [Fact]
    public async Task TestGetAndSetAdapterInMemAsync()
    {
        Enforcer e = new("Examples/basic_model.conf", "Examples/basic_policy_for_async_adapter_test.csv");
        Enforcer e2 = new("Examples/basic_model.conf", "Examples/basic_inverse_policy.csv");

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));

        IReadOnlyAdapter a2 = e2.Adapter;
        e.SetAdapter(a2);
        await e.LoadPolicyAsync();

        Assert.False(await e.EnforceAsync("alice", "data1", "read"));
        Assert.True(await e.EnforceAsync("alice", "data1", "write"));
    }

    [Fact]
    public void TestEnforceExApi()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        e.TestEnforceEx("alice", "data1", "read", ["alice", "data1", "read"]);
        e.TestEnforceEx("alice", "data1", "write", []);
        e.TestEnforceEx("alice", "data2", "read", []);
        e.TestEnforceEx("alice", "data2", "write", []);
        e.TestEnforceEx("bob", "data1", "read", []);
        e.TestEnforceEx("bob", "data1", "write", []);
        e.TestEnforceEx("bob", "data2", "read", []);
        e.TestEnforceEx("bob", "data2", "write", ["bob", "data2", "write"]);

        e = new Enforcer(TestModelFixture.GetNewRbacTestModel());

        e.TestEnforceEx("alice", "data1", "read", ["alice", "data1", "read"]);
        e.TestEnforceEx("alice", "data1", "write", []);
        // e.TestEnforceEx("alice", "data2", "read", ["data2_admin", "data2", "read"]); TODO: should be fixed
        // e.TestEnforceEx("alice", "data2", "write", ["data2_admin", "data2", "write"]); TODO: should be fixed
        e.TestEnforceEx("bob", "data1", "read", []);
        e.TestEnforceEx("bob", "data1", "write", []);
        e.TestEnforceEx("bob", "data2", "read", []);
        e.TestEnforceEx("bob", "data2", "write", ["bob", "data2", "write"]);

        e = new Enforcer(TestModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        e.TestEnforceEx("alice", "data1", "read", ["alice", "data1", "read", "allow"]);
        e.TestEnforceEx("alice", "data1", "write", ["data1_deny_group", "data1", "write", "deny"]);
        e.TestEnforceEx("alice", "data2", "read", []);
        e.TestEnforceEx("alice", "data2", "write", []);
        e.TestEnforceEx("bob", "data1", "write", []);
        e.TestEnforceEx("bob", "data2", "read", ["data2_allow_group", "data2", "read", "allow"]);
        e.TestEnforceEx("bob", "data2", "write", ["bob", "data2", "write", "deny"]);
    }

    [Fact]
    public async Task TestEnforceExApiAsync()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());

        await e.TestEnforceExAsync("alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await e.TestEnforceExAsync("alice", "data1", "write", new List<string>());
        await e.TestEnforceExAsync("alice", "data2", "read", new List<string>());
        await e.TestEnforceExAsync("alice", "data2", "write", new List<string>());
        await e.TestEnforceExAsync("bob", "data1", "read", new List<string>());
        await e.TestEnforceExAsync("bob", "data1", "write", new List<string>());
        await e.TestEnforceExAsync("bob", "data2", "read", new List<string>());
        await e.TestEnforceExAsync("bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(TestModelFixture.GetNewRbacTestModel());

        await e.TestEnforceExAsync("alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        await e.TestEnforceExAsync("alice", "data1", "write", new List<string>());
        // await e.TestEnforceExAsync("alice", "data2", "read", new List<string> { "data2_admin", "data2", "read" }); TODO: should be fixed
        // await e.TestEnforceExAsync("alice", "data2", "write", new List<string> { "data2_admin", "data2", "write" }); TODO: should be fixed
        await e.TestEnforceExAsync("bob", "data1", "read", new List<string>());
        await e.TestEnforceExAsync("bob", "data1", "write", new List<string>());
        await e.TestEnforceExAsync("bob", "data2", "read", new List<string>());
        await e.TestEnforceExAsync("bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e = new Enforcer(TestModelFixture.GetNewPriorityTestModel());
        e.BuildRoleLinks();

        await e.TestEnforceExAsync("alice", "data1", "read", new List<string> { "alice", "data1", "read", "allow" });
        await e.TestEnforceExAsync("alice", "data1", "write",
            new List<string> { "data1_deny_group", "data1", "write", "deny" });
        await e.TestEnforceExAsync("alice", "data2", "read", new List<string>());
        await e.TestEnforceExAsync("alice", "data2", "write", new List<string>());
        await e.TestEnforceExAsync("bob", "data1", "write", new List<string>());
        await e.TestEnforceExAsync("bob", "data2", "read",
            new List<string> { "data2_allow_group", "data2", "read", "allow" });
        await e.TestEnforceExAsync("bob", "data2", "write", new List<string> { "bob", "data2", "write", "deny" });
    }

#if !NET452
    [Fact]
    public void TestEnforceExApiLog()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel()) { Logger = new MockLogger<Enforcer>(_testOutputHelper) };

        e.TestEnforceEx("alice", "data1", "read", new List<string> { "alice", "data1", "read" });
        e.TestEnforceEx("alice", "data1", "write", new List<string>());
        e.TestEnforceEx("alice", "data2", "read", new List<string>());
        e.TestEnforceEx("alice", "data2", "write", new List<string>());
        e.TestEnforceEx("bob", "data1", "read", new List<string>());
        e.TestEnforceEx("bob", "data1", "write", new List<string>());
        e.TestEnforceEx("bob", "data2", "read", new List<string>());
        e.TestEnforceEx("bob", "data2", "write", new List<string> { "bob", "data2", "write" });

        e.Logger = null;
    }
#endif

    #endregion

    #region EnforceWithMatcher and EnforceExWithMatcher API

    [Fact]
    public void TestEnforceWithMatcherApi()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        Assert.False(e.EnforceWithMatcher(matcher, "alice", "data1", "read"));
        Assert.False(e.EnforceWithMatcher(matcher, "alice", "data1", "write"));
        Assert.False(e.EnforceWithMatcher(matcher, "alice", "data2", "read"));
        Assert.True(e.EnforceWithMatcher(matcher, "alice", "data2", "write"));
        Assert.True(e.EnforceWithMatcher(matcher, "bob", "data1", "read"));
        Assert.False(e.EnforceWithMatcher(matcher, "bob", "data1", "write"));
        Assert.False(e.EnforceWithMatcher(matcher, "bob", "data2", "read"));
        Assert.False(e.EnforceWithMatcher(matcher, "bob", "data2", "write"));
    }

    [Fact]
    public async Task TestEnforceWithMatcherAsync()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        Assert.False(await e.EnforceWithMatcherAsync(matcher, "alice", "data1", "read"));
        Assert.False(await e.EnforceWithMatcherAsync(matcher, "alice", "data1", "write"));
        Assert.False(await e.EnforceWithMatcherAsync(matcher, "alice", "data2", "read"));
        Assert.True(await e.EnforceWithMatcherAsync(matcher, "alice", "data2", "write"));
        Assert.True(await e.EnforceWithMatcherAsync(matcher, "bob", "data1", "read"));
        Assert.False(await e.EnforceWithMatcherAsync(matcher, "bob", "data1", "write"));
        Assert.False(await e.EnforceWithMatcherAsync(matcher, "bob", "data2", "read"));
        Assert.False(await e.EnforceWithMatcherAsync(matcher, "bob", "data2", "write"));
    }

    [Fact]
    public void TestBatchEnforceWithMatcherApi()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), false),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), false),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), true),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), false)
        ];

        e.TestBatchEnforceWithMatcher(matcher, testCases);
    }

    [Fact]
    public void TestBatchEnforceWithMatcherParallel()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), false),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), false),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), true),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), false)
        ];

        e.TestBatchEnforceWithMatcherParallel(matcher, testCases);
    }

    [Fact]
    public void TestBatchEnforceWithMatcherApiAsync()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        IEnumerable<(RequestValues<string, string, string>, bool)> testCases =
        [
            (Request.CreateValues("alice", "data1", "read"), false),
            (Request.CreateValues("alice", "data1", "write"), false),
            (Request.CreateValues("alice", "data2", "read"), false),
            (Request.CreateValues("alice", "data2", "write"), true),
            (Request.CreateValues("bob", "data1", "read"), true),
            (Request.CreateValues("bob", "data1", "write"), false),
            (Request.CreateValues("bob", "data2", "read"), false),
            (Request.CreateValues("bob", "data2", "write"), false)
        ];

        TestBatchEnforceWithMatcherAsync(e, matcher, testCases);
    }

    [Fact]
    public void TestEnforceExWithMatcherApi()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        e.TestEnforceExWithMatcher(matcher, "alice", "data1", "read", []);
        e.TestEnforceExWithMatcher(matcher, "alice", "data1", "write", []);
        e.TestEnforceExWithMatcher(matcher, "alice", "data2", "read", []);
        e.TestEnforceExWithMatcher(matcher, "alice", "data2", "write", ["bob", "data2", "write"]);
        e.TestEnforceExWithMatcher(matcher, "bob", "data1", "read", ["alice", "data1", "read"]);
        e.TestEnforceExWithMatcher(matcher, "bob", "data1", "write", []);
        e.TestEnforceExWithMatcher(matcher, "bob", "data2", "read", []);
        e.TestEnforceExWithMatcher(matcher, "bob", "data2", "write", []);
    }

    [Fact]
    public async Task TestEnforceExWithMatcherAsync()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        string matcher = "r.sub != p.sub && r.obj == p.obj && r.act == p.act";

        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data1", "read", []);
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data1", "write", []);
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data2", "read", []);
        await e.TestEnforceExWithMatcherAsync(matcher, "alice", "data2", "write", ["bob", "data2", "write"]);
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data1", "read", ["alice", "data1", "read"]);
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data1", "write", []);
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data2", "read", []);
        await e.TestEnforceExWithMatcherAsync(matcher, "bob", "data2", "write", []);
    }

    #endregion

    [Fact]
    public void TestInOperatorWithLiteralList()
    {
        string text =
            "[request_definition]\n"
            + "r = sub, obj, act\n"
            + "\n"
            + "[policy_definition]\n"
            + "p = sub, obj, act\n"
            + "\n"
            + "[policy_effect]\n"
            + "e = some(where (p.eft == allow))\n"
            + "\n"
            + "[matchers]\n"
            + "m = r.sub == p.sub && r.obj in (\"a, \\\"b,c\\\"\", \"f(x)\") && r.act in ['read', 'write']\n";

        Enforcer e = new(DefaultModel.CreateFromText(text));
        e.AddPolicy("alice", "any", "any");

        Assert.True(e.Enforce("alice", "a, \"b,c\"", "read"));
        Assert.True(e.Enforce("alice", "f(x)", "write"));
        Assert.False(e.Enforce("alice", "a", "read"));
        Assert.False(e.Enforce("alice", "f(x)", "delete"));
        Assert.False(e.Enforce("bob", "f(x)", "read"));
    }

#if !NET452
    #region ExpressionHandler Tests

    [Fact]
    public void TestExpressionHandlerSingleQuoteReplacement()
    {
        Enforcer e = new(TestModelFixture.GetBasicTestModel());
        // Single quotes should be replaced with double quotes to handle DynamicExpresso limitations
        string matcherWithSingleQuotes = "r.sub == 'alice' && r.obj == p.obj && r.act == p.act";

        Assert.True(e.EnforceWithMatcher(matcherWithSingleQuotes, "alice", "data1", "read"));
        Assert.False(e.EnforceWithMatcher(matcherWithSingleQuotes, "alice", "data1", "write"));
        Assert.False(e.EnforceWithMatcher(matcherWithSingleQuotes, "bob", "data1", "read"));
    }

    [Fact]
    public void TestExpressionHandlerLogsWarningOnInvalidExpression()
    {
        var logger = new MockLogger<Enforcer>(_testOutputHelper);
        Enforcer e = new(TestModelFixture.GetBasicTestModel()) { Logger = logger };

        // An invalid expression should return false and log a warning
        Assert.False(e.EnforceWithMatcher("this_is_not_valid!!!", "alice", "data1", "read"));
        Assert.Contains(logger.Logs, log => log.Level == Microsoft.Extensions.Logging.LogLevel.Warning);
    }

    #endregion
#endif
}
