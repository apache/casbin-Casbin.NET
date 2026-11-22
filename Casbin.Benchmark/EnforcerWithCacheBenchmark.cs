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
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using static Casbin.Benchmark.TestHelper;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("EnforcerWithCache")]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net70)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
    public class EnforcerWithCacheBenchmark
    {
        private Enforcer NowEnforcer { get; set; }
        private string NowTestUserName { get; set; }
        private string NowTestDataName { get; set; }
        private TestResource NowTestResource { get; set; }

        private int[][] RbacScale { get; } =
        {
            new[] { 100, 1000 }, // Small
            new[] { 1000, 10000 }, // Medium
            new[] { 10000, 100000 } // Large
        };

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            NowEnforcer = null;
            Console.WriteLine("// Cleaned the enforcer");
        }

        [Benchmark]
        //[Benchmark(Description = "ACL, 2 rules (2 users)")]
        [BenchmarkCategory("BasicModel")]
        public void BasicModel()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC, 5 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModel()
        {
            _ = NowEnforcer.Enforce("alice", "data2", "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC (small), 1100 rules (1000 users, 100 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithSmallScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC (medium), 11000 rules (10000 users, 1000 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithMediumScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC (large), 110000 rules (100000 users, 10000 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithLargeScale()
        {
            _ = NowEnforcer.Enforce(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC with resource roles, 6 rules (2 users, 2 roles)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithResourceRoles()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RBAC with domains/tenants, 6 rules (2 users, 1 role, 2 domains)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithDomains()
        {
            _ = NowEnforcer.Enforce("alice", "domain1", "data1", "read");
        }

        [Benchmark]
        //[Benchmark(Description = "Deny-override, 6 rules (2 users, 1 role)")]
        [BenchmarkCategory("RbacModel")]
        public void RbacModelWithDeny()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        [Benchmark]
        //[Benchmark(Description = "ABAC, 0 rule (0 user)")]
        [BenchmarkCategory("AbacModel")]
        public void AbacModel()
        {
            var data1 = NowTestResource;
            _ = NowEnforcer.Enforce("alice", data1, "read");
        }

        [Benchmark]
        //[Benchmark(Description = "RESTful, 5 rules (3 users)")]
        [BenchmarkCategory("KeyMatchModel")]
        public void KeyMatchModel()
        {
            _ = NowEnforcer.Enforce("alice", "/alice_data/resource1", "GET");
        }

        [Benchmark]
        //[Benchmark(Description = "Priority, 9 rules (2 users, 2 roles)")]
        [BenchmarkCategory("PriorityModel")]
        public void PriorityModel()
        {
            _ = NowEnforcer.Enforce("alice", "data1", "read");
        }

        #region GlobalSetup

        [GlobalSetup(Targets = new[] { nameof(BasicModel) })]
        public void GlobalSetupForBasicModel()
        {
            GlobalSetupFromFile("basic_model.conf", "basic_policy.csv");
            Console.WriteLine("// Set the Basic enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModel) })]
        public void GlobalSetupForRbacModel()
        {
            GlobalSetupFromFile("rbac_model.conf", "rbac_policy.csv");
            Console.WriteLine("// Set the Rbac Model enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithSmallScale) })]
        public void GlobalSetupForRbacModelWithSmallScale()
        {
            int groupCount = RbacScale[0][0];
            int userCount = RbacScale[0][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine(
                $"// Set the Rbac Model with small scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithMediumScale) })]
        public void GlobalSetupForRbacModelWithMediumScale()
        {
            int groupCount = RbacScale[1][0];
            int userCount = RbacScale[1][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine(
                $"// Set the Rbac Model with medium scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithLargeScale) })]
        public void GlobalSetupForRbacModelWithLargeScale()
        {
            int groupCount = RbacScale[2][0];
            int userCount = RbacScale[2][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount);
            Console.WriteLine(
                $"// Set the RBAC with large scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithResourceRoles) })]
        public void GlobalSetupForRbacModelWithResourceRoles()
        {
            GlobalSetupFromFile("rbac_with_resource_roles_model.conf", "rbac_with_resource_roles_policy.csv");
            Console.WriteLine("// Set the RbacModel With Resource Roles enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithDomains) })]
        public void GlobalSetupForRbacModelWithDomains()
        {
            GlobalSetupFromFile("rbac_with_domains_model.conf", "rbac_with_domains_policy.csv");
            Console.WriteLine("// Set the Rbac Model With Domains enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithDeny) })]
        public void GlobalSetupForRbacModelWithDeny()
        {
            GlobalSetupFromFile("rbac_with_deny_model.conf", "rbac_with_deny_policy.csv");
            Console.WriteLine("// Set the Rbac Model With Deny enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(AbacModel) })]
        public void GlobalSetupForAbacModel()
        {
            GlobalSetupFromFile("abac_model.conf");
            NowTestResource = new TestResource("data1", "alice");
            Console.WriteLine("// Set the Abac Model enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(KeyMatchModel) })]
        public void GlobalSetupForKeyMatchModel()
        {
            GlobalSetupFromFile("keymatch_model.conf", "keymatch_policy.csv");
            Console.WriteLine("// Set the Key Match Model enforcer");
        }

        [GlobalSetup(Targets = new[] { nameof(PriorityModel) })]
        public void GlobalSetupForPriorityModel()
        {
            GlobalSetupFromFile("priority_model.conf", "priority_policy.csv");
            Console.WriteLine("// Set the Priority Model enforcer");
        }

        #endregion

        #region private help method

        private void GlobalSetupForRbacModelWithScale(int groupCount, int userCount)
        {
            GlobalSetupForRbacModel();
            var policyList = new List<List<string>>();
            for (int i = 0; i < groupCount; i++)
            {
                policyList.Add(new[] { $"group{i}", $"data{i / 10}", "read" }.ToList());
            }

            NowEnforcer.AddPolicies(policyList);

            policyList.Clear();
            for (int i = 0; i < userCount; i++)
            {
                policyList.Add(new[] { $"user{i}", $"group{i / 10}" }.ToList());
            }

            NowEnforcer.EnableAutoBuildRoleLinks(false);
            NowEnforcer.AddGroupingPolicies(policyList);
            NowEnforcer.BuildRoleLinks();

            NowTestUserName = $"user{userCount / 2 + 1}"; // if 1000 => 501...
            NowTestDataName = $"data{groupCount / 10 - 1}"; // if 100 => 9...
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set data name to {NowTestDataName}.");
        }

        private void GlobalSetupFromFile(string modelFileName, string policyFileName = null)
        {
            if (policyFileName is null)
            {
                NowEnforcer = new Enforcer(GetTestFilePath(modelFileName));
                NowEnforcer.EnableCache(true);
                return;
            }

            NowEnforcer = new Enforcer(GetTestFilePath(modelFileName), GetTestFilePath(policyFileName));
            NowEnforcer.EnableCache(true);
        }

        #endregion
    }
}
