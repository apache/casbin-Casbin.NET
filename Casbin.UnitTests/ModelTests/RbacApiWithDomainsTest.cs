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

using System.Threading.Tasks;
using Casbin.UnitTests.Fixtures;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.ModelTests;

[Collection("Model collection")]
public class RbacApiWithDomainsTest
{
    private readonly TestModelFixture _testModelFixture;
    public RbacApiWithDomainsTest(TestModelFixture testModelFixture) => _testModelFixture = testModelFixture;

    [Fact]
    public void TestGetDomainsForUser()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithDomainsModelText,
            TestModelFixture.RbacWithDomainsPolicy2Text));

        e.BuildRoleLinks();

        e.TestGetDomainsForUser("alice", new[] { "domain1", "domain2" });
        e.TestGetDomainsForUser("bob", new[] { "domain2", "domain3" });
        e.TestGetDomainsForUser("user", new[] { "domain3" });
    }

    [Fact]
    public void TestGetRolesFromUserWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewTestModel(
            TestModelFixture.RbacWithDomainsModelText,
            TestModelFixture.RbacWithHierarchyWithDomainsPolicyText));

        e.BuildRoleLinks();

        // This is only able to retrieve the first level of roles.
        TestGetRolesInDomain(e, "alice", "domain1", ["role:global_admin"]);

        // Retrieve all inherit roles. It supports domains as well.
        TestGetImplicitRolesInDomain(e, "alice", "domain1", ["role:global_admin", "role:reader", "role:writer"]);
    }

    [Fact]
    public void TestRoleApiWithDomains()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        TestGetRolesInDomain(e, "alice", "domain1", ["admin"]);
        TestGetRolesInDomain(e, "bob", "domain1", []);
        TestGetRolesInDomain(e, "admin", "domain1", []);
        TestGetRolesInDomain(e, "non_exist", "domain1", []);

        TestGetRolesInDomain(e, "alice", "domain2", []);
        TestGetRolesInDomain(e, "bob", "domain2", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain2", []);
        TestGetRolesInDomain(e, "non_exist", "domain2", []);

        e.DeleteRoleForUserInDomain("alice", "admin", "domain1");
        e.AddRoleForUserInDomain("bob", "admin", "domain1");

        TestGetRolesInDomain(e, "alice", "domain1", []);
        TestGetRolesInDomain(e, "bob", "domain1", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain1", []);
        TestGetRolesInDomain(e, "non_exist", "domain1", []);

        TestGetRolesInDomain(e, "alice", "domain2", []);
        TestGetRolesInDomain(e, "bob", "domain2", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain2", []);
        TestGetRolesInDomain(e, "non_exist", "domain2", []);
    }

    [Fact]
    public async Task TestRoleApiWithDomainsAsync()
    {
        Enforcer e = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
        e.BuildRoleLinks();

        TestGetRolesInDomain(e, "alice", "domain1", ["admin"]);
        TestGetRolesInDomain(e, "bob", "domain1", []);
        TestGetRolesInDomain(e, "admin", "domain1", []);
        TestGetRolesInDomain(e, "non_exist", "domain1", []);

        TestGetRolesInDomain(e, "alice", "domain2", []);
        TestGetRolesInDomain(e, "bob", "domain2", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain2", []);
        TestGetRolesInDomain(e, "non_exist", "domain2", []);

        await e.DeleteRoleForUserInDomainAsync("alice", "admin", "domain1");
        await e.AddRoleForUserInDomainAsync("bob", "admin", "domain1");

        TestGetRolesInDomain(e, "alice", "domain1", []);
        TestGetRolesInDomain(e, "bob", "domain1", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain1", []);
        TestGetRolesInDomain(e, "non_exist", "domain1", []);

        TestGetRolesInDomain(e, "alice", "domain2", []);
        TestGetRolesInDomain(e, "bob", "domain2", ["admin"]);
        TestGetRolesInDomain(e, "admin", "domain2", []);
        TestGetRolesInDomain(e, "non_exist", "domain2", []);
    }
}
