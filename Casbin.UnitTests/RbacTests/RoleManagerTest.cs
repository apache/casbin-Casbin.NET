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
using Casbin.Functions;
using Casbin.Rbac;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.RbacTests;

public class RoleManagerTest
{
    [Fact]
    public void TestRoleApi()
    {
        DefaultRoleManager roleManager = new(3);
        roleManager.AddLink("u1", "g1");
        roleManager.AddLink("u2", "g1");
        roleManager.AddLink("u3", "g2");
        roleManager.AddLink("u4", "g2");
        roleManager.AddLink("u4", "g3");
        roleManager.AddLink("g1", "g3");

        // Current role inheritance tree:
        //             g3    g2
        //            /  \  /  \
        //          g1    u4    u3
        //         /  \
        //       u1    u2

        Assert.True(roleManager.HasLink("u1", "g1"));
        Assert.False(roleManager.HasLink("u1", "g2"));
        Assert.True(roleManager.HasLink("u1", "g3"));
        Assert.True(roleManager.HasLink("u2", "g1"));
        Assert.False(roleManager.HasLink("u2", "g2"));
        Assert.True(roleManager.HasLink("u2", "g3"));
        Assert.False(roleManager.HasLink("u3", "g1"));
        Assert.True(roleManager.HasLink("u3", "g2"));
        Assert.False(roleManager.HasLink("u3", "g3"));
        Assert.False(roleManager.HasLink("u4", "g1"));
        Assert.True(roleManager.HasLink("u4", "g2"));
        Assert.True(roleManager.HasLink("u4", "g3"));

        TestGetRoles(roleManager, "u1", ["g1"]);
        TestGetRoles(roleManager, "u2", ["g1"]);
        TestGetRoles(roleManager, "u3", ["g2"]);
        TestGetRoles(roleManager, "u4", ["g2", "g3"]);
        TestGetRoles(roleManager, "g1", ["g3"]);
        TestGetRoles(roleManager, "g2", []);
        TestGetRoles(roleManager, "g3", []);

        roleManager.DeleteLink("g1", "g3");
        roleManager.DeleteLink("u4", "g2");

        // Current role inheritance tree after deleting the links:
        //             g3    g2
        //               \     \
        //          g1    u4    u3
        //         /  \
        //       u1    u2

        Assert.True(roleManager.HasLink("u1", "g1"));
        Assert.False(roleManager.HasLink("u1", "g2"));
        Assert.False(roleManager.HasLink("u1", "g3"));
        Assert.True(roleManager.HasLink("u2", "g1"));
        Assert.False(roleManager.HasLink("u2", "g2"));
        Assert.False(roleManager.HasLink("u2", "g3"));
        Assert.False(roleManager.HasLink("u3", "g1"));
        Assert.True(roleManager.HasLink("u3", "g2"));
        Assert.False(roleManager.HasLink("u3", "g3"));
        Assert.False(roleManager.HasLink("u4", "g1"));
        Assert.False(roleManager.HasLink("u4", "g2"));
        Assert.True(roleManager.HasLink("u4", "g3"));

        TestGetRoles(roleManager, "u1", ["g1"]);
        TestGetRoles(roleManager, "u2", ["g1"]);
        TestGetRoles(roleManager, "u3", ["g2"]);
        TestGetRoles(roleManager, "u4", ["g3"]);
        TestGetRoles(roleManager, "g1", []);
        TestGetRoles(roleManager, "g2", []);
        TestGetRoles(roleManager, "g3", []);
    }

    [Fact]
    public void TestDomainRoleApi()
    {
        DefaultRoleManager roleManager = new(3);
        roleManager.AddLink("u1", "g1", "domain1");
        roleManager.AddLink("u2", "g1", "domain1");
        roleManager.AddLink("u3", "admin", "domain2");
        roleManager.AddLink("u4", "admin", "domain2");
        roleManager.AddLink("u4", "admin", "domain1");
        roleManager.AddLink("g1", "admin", "domain1");

        // Current role inheritance tree:
        //       domain1:admin    domain2:admin
        //            /       \  /       \
        //      domain1:g1     u4         u3
        //         /  \
        //       u1    u2

        Assert.True(roleManager.HasLink("u1", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u1", "g1", "domain2"));
        Assert.True(roleManager.HasLink("u1", "admin", "domain1"));
        Assert.False(roleManager.HasLink("u1", "admin", "domain2"));

        Assert.True(roleManager.HasLink("u2", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u2", "g1", "domain2"));
        Assert.True(roleManager.HasLink("u2", "admin", "domain1"));
        Assert.False(roleManager.HasLink("u2", "admin", "domain2"));

        Assert.False(roleManager.HasLink("u3", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u3", "g1", "domain2"));
        Assert.False(roleManager.HasLink("u3", "admin", "domain1"));
        Assert.True(roleManager.HasLink("u3", "admin", "domain2"));

        Assert.False(roleManager.HasLink("u4", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u4", "g1", "domain2"));
        Assert.True(roleManager.HasLink("u4", "admin", "domain1"));
        Assert.True(roleManager.HasLink("u4", "admin", "domain2"));

        roleManager.DeleteLink("g1", "admin", "domain1");
        roleManager.DeleteLink("u4", "admin", "domain2");

        // Current role inheritance tree after deleting the links:
        //       domain1:admin    domain2:admin
        //                    \          \
        //      domain1:g1     u4         u3
        //         /  \
        //       u1    u2

        Assert.True(roleManager.HasLink("u1", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u1", "g1", "domain2"));
        Assert.False(roleManager.HasLink("u1", "admin", "domain1"));
        Assert.False(roleManager.HasLink("u1", "admin", "domain2"));

        Assert.True(roleManager.HasLink("u2", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u2", "g1", "domain2"));
        Assert.False(roleManager.HasLink("u2", "admin", "domain1"));
        Assert.False(roleManager.HasLink("u2", "admin", "domain2"));

        Assert.False(roleManager.HasLink("u3", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u3", "g1", "domain2"));
        Assert.False(roleManager.HasLink("u3", "admin", "domain1"));
        Assert.True(roleManager.HasLink("u3", "admin", "domain2"));

        Assert.False(roleManager.HasLink("u4", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u4", "g1", "domain2"));
        Assert.True(roleManager.HasLink("u4", "admin", "domain1"));
        Assert.False(roleManager.HasLink("u4", "admin", "domain2"));
    }

    [Fact]
    public void TestClearApi()
    {
        DefaultRoleManager roleManager = new(3);
        roleManager.AddLink("u1", "g1");
        roleManager.AddLink("u2", "g1");
        roleManager.AddLink("u3", "g2");
        roleManager.AddLink("u4", "g2");
        roleManager.AddLink("u4", "g3");
        roleManager.AddLink("g1", "g3");

        // Current role inheritance tree:
        //             g3    g2
        //            /  \  /  \
        //          g1    u4    u3
        //         /  \
        //       u1    u2

        roleManager.Clear();

        // All data is cleared.
        // No role inheritance now.

        Assert.False(roleManager.HasLink("u1", "g1"));
        Assert.False(roleManager.HasLink("u1", "g2"));
        Assert.False(roleManager.HasLink("u1", "g3"));
        Assert.False(roleManager.HasLink("u2", "g1"));
        Assert.False(roleManager.HasLink("u2", "g2"));
        Assert.False(roleManager.HasLink("u2", "g3"));
        Assert.False(roleManager.HasLink("u3", "g1"));
        Assert.False(roleManager.HasLink("u3", "g2"));
        Assert.False(roleManager.HasLink("u3", "g3"));
        Assert.False(roleManager.HasLink("u4", "g1"));
        Assert.False(roleManager.HasLink("u4", "g2"));
        Assert.False(roleManager.HasLink("u4", "g3"));
    }

    [Fact]
    public void TestDomainPatternRoleApi()
    {
        DefaultRoleManager roleManager = new(10);

        roleManager.AddDomainMatchingFunc(BuiltInFunctions.KeyMatch2);

        roleManager.AddLink("u1", "g1", "domain1");
        roleManager.AddLink("u2", "g1", "domain2");
        roleManager.AddLink("u3", "g1", "*");
        roleManager.AddLink("u4", "g2", "domain3");

        // Current role inheritance tree after deleting the links:
        //       domain1:g1    domain2:g1			domain3:g2
        //		   /      \    /      \					|
        //	 domain1:u1    *:g1     domain2:u2		domain3:u4
        // 					|
        // 				   *:u3

        Assert.True(roleManager.HasLink("u1", "g1", "domain1"));
        Assert.False(roleManager.HasLink("u2", "g1", "domain1"));
        Assert.True(roleManager.HasLink("u2", "g1", "domain2"));
        Assert.True(roleManager.HasLink("u3", "g1", "domain1"));
        Assert.True(roleManager.HasLink("u3", "g1", "domain2"));
        Assert.False(roleManager.HasLink("u1", "g2", "domain1"));
        Assert.True(roleManager.HasLink("u4", "g2", "domain3"));
        Assert.False(roleManager.HasLink("u3", "g2", "domain3"));

        TestGetRolesWithDomain(roleManager, "u3", "domain1", ["g1"]);
        TestGetRolesWithDomain(roleManager, "u1", "domain1", ["g1"]);
        TestGetRolesWithDomain(roleManager, "u3", "domain2", ["g1"]);
        TestGetRolesWithDomain(roleManager, "u1", "domain2", []);
        TestGetRolesWithDomain(roleManager, "u4", "domain3", ["g2"]);
    }

    [Fact]
    public void TestAllMatchingFuncApi()
    {
        DefaultRoleManager roleManager = new(10);

        roleManager.AddMatchingFunc(BuiltInFunctions.KeyMatch2);
        roleManager.AddDomainMatchingFunc(BuiltInFunctions.KeyMatch2);
        roleManager.AddLink("/book/:id", "book_group", "*");

        // Current role inheritance tree after deleting the links:
        //  		*:book_group
        //				|
        // 			*:/book/:id

        Assert.True(roleManager.HasLink("/book/1", "book_group", "domain1"));
        Assert.True(roleManager.HasLink("/book/2", "book_group", "domain1"));
    }
}
