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

using Casbin.Rbac;
using Xunit;
using static Casbin.UnitTests.Util.TestUtil;

namespace Casbin.UnitTests.RbacTests;

public class GroupRoleManagerTest
{
    [Fact]
    public void TestGroupRoleManager()
    {
        Enforcer e = new("Examples/group_with_domain_model.conf", "Examples/group_with_domain_policy.csv");
        GroupRoleManager roleManager = new(10);
        e.SetRoleManager("g", roleManager);
        e.SetRoleManager("g2", roleManager);
        Assert.True(e.Enforce("alice", "domain1", "data1", "read"));
    }
}
