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
using Casbin.Functions;
using Casbin.Model;
using Casbin.Rbac;
using static Casbin.Benchmark.TestHelper;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Enforcer")]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net70)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
    public class DefaultRoleManagerBenchmark
    {
        private Enforcer NowEnforcer { get; set; }
        private DefaultRoleManager NowRoleManager { get; set; }
        private string NowTestUserName { get; set; }
        private string NowTestRoleName { get; set; }
        private string NowTestRole2Name { get; set; }
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
        [BenchmarkCategory("RbacModel")]
        public void HasLinkWithPatternLarge()
        {
            NowRoleManager.HasLink("staffUser1001", "staff001", "/orgs/1/sites/site001");
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void HasLinkWithDomainPatternLarge()
        {
            NowRoleManager.HasLink("staffUser1001", "staff001", "/orgs/1/sites/site001");
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void HasLinkWithPatternAndDomainPatternLarge()
        {
            NowRoleManager.HasLink("staffUser1001", "staff001", "/orgs/1/sites/site001");
        }

        [Benchmark]
        [BenchmarkCategory("RbacModel")]
        public void BuildRoleLinksWithPatternLarge()
        {
            NowEnforcer.BuildRoleLinks();
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void BuildRoleLinksWithDomainPatternLarge()
        {
            NowEnforcer.BuildRoleLinks();
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithDomain")]
        public void BuildRoleLinksWithPatternAndDomainPatternLarge()
        {
            NowEnforcer.BuildRoleLinks();
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithSmallScale")]
        public void RbacModelWithSmallScale()
        {
            for (int i = 0; i < RbacScale[0][0]; i++)
            {
                NowRoleManager.HasLink(NowTestRoleName, $"group{i}");
            }
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithMiddleScale")]
        public void RbacModelWithMediumScale()
        {
            for (int i = 0; i < RbacScale[1][0]; i++)
            {
                NowRoleManager.HasLink(NowTestRoleName, $"group{i}");
            }
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithLargeScale")]
        public void RbacModelWithLargeScale()
        {
            for (int i = 0; i < RbacScale[2][0]; i++)
            {
                NowRoleManager.HasLink(NowTestRoleName, $"group{i}");
            }
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithSmallScale")]
        public void RbacModelWithSmallScaleRandom()
        {
            NowRoleManager.HasLink(NowTestRoleName, NowTestRole2Name);
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithMiddleScale")]
        public void RbacModelWithMediumScaleRandom()
        {
            for (int i = 0; i < RbacScale[1][0]; i++)
            {
                NowRoleManager.HasLink(NowTestRoleName, NowTestRole2Name);
            }
        }

        [Benchmark]
        [BenchmarkCategory("RbacModelWithLargeScale")]
        public void RbacModelWithLargeScaleRandom()
        {
            for (int i = 0; i < RbacScale[2][0]; i++)
            {
                NowRoleManager.HasLink(NowTestRoleName, NowTestRole2Name);
            }
        }

        #region GlobalSetup

        [GlobalSetup(Targets = new[] { nameof(BuildRoleLinksWithPatternLarge), nameof(HasLinkWithPatternLarge) })]
        public void GlobalSetupForRbacModelWithMatchingFunc()
        {
            GlobalSetupFromFile("rbac_with_pattern_large_scale_model.conf", "rbac_with_pattern_large_scale_policy.csv");
            NowEnforcer.AddNamedMatchingFunc("g", BuiltInFunctions.KeyMatch4);
        }

        [GlobalSetup(Targets =
            new[] { nameof(BuildRoleLinksWithDomainPatternLarge), nameof(HasLinkWithDomainPatternLarge) })]
        public void GlobalSetupForRbacModelWithDomainMatchingFunc()
        {
            GlobalSetupFromFile("rbac_with_pattern_large_scale_model.conf", "rbac_with_pattern_large_scale_policy.csv");
            NowEnforcer.AddNamedDomainMatchingFunc("g", BuiltInFunctions.KeyMatch4);
        }

        [GlobalSetup(Targets = new[]
        {
            nameof(BuildRoleLinksWithPatternAndDomainPatternLarge), nameof(HasLinkWithPatternAndDomainPatternLarge)
        })]
        public void GlobalSetupForRbacModelWithPatternAndDomainMatchingFunc()
        {
            GlobalSetupFromFile("rbac_with_pattern_large_scale_model.conf", "rbac_with_pattern_large_scale_policy.csv");
            NowEnforcer.AddNamedMatchingFunc("g", BuiltInFunctions.KeyMatch4);
            NowEnforcer.AddNamedDomainMatchingFunc("g", BuiltInFunctions.KeyMatch4);
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

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithSmallScaleRandom) })]
        public void GlobalSetupForRbacModelWithSmallScaleRandom()
        {
            int groupCount = RbacScale[0][0];
            int userCount = RbacScale[0][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount, true);
            Console.WriteLine(
                $"// Set the Rbac Model with small scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithMediumScaleRandom) })]
        public void GlobalSetupForRbacModelWithMediumScaleRandom()
        {
            int groupCount = RbacScale[1][0];
            int userCount = RbacScale[1][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount, true);
            Console.WriteLine(
                $"// Set the Rbac Model with medium scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        [GlobalSetup(Targets = new[] { nameof(RbacModelWithLargeScaleRandom) })]
        public void GlobalSetupForRbacModelWithLargeScaleRandom()
        {
            int groupCount = RbacScale[2][0];
            int userCount = RbacScale[2][1];
            GlobalSetupForRbacModelWithScale(groupCount, userCount, true);
            Console.WriteLine(
                $"// Set the RBAC with large scale ({groupCount} groups and {userCount} users) enforcer.");
        }

        #endregion

        #region private help method

        /// <summary>
        /// Build the dependencies of groups and users without cycle.
        /// </summary>
        /// <param name="groupCount"></param>
        /// <returns></returns>
        private List<List<string>> BuildGroupDependencies(int groupCount)
        {
            List<List<string>> res = new List<List<string>>();
            int[] disjointSet = new int[groupCount];
            // Init the Disjoint-set
            for (int i = 0; i < groupCount; i++)
            {
                disjointSet[i] = i;
            }

            // Build dependencies between groups
            var rd = new Random();
            for (int i = 0; i < groupCount; i++)
            {
                int father = rd.Next(groupCount);
                if (findRootInDisjointSet(disjointSet, i) != findRootInDisjointSet(disjointSet, father))
                {
                    disjointSet[findRootInDisjointSet(disjointSet, i)] = findRootInDisjointSet(disjointSet, father);
                    res.Add(new[] { $"group{i}", $"group{father}" }.ToList());
                }
            }

            return res;
        }

        /// <summary>
        /// Find the root in Disjoint-set
        /// </summary>
        /// <param name="disjointSet"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private int findRootInDisjointSet(int[] disjointSet, int x)
        {
            if (disjointSet[x] != x)
            {
                disjointSet[x] = findRootInDisjointSet(disjointSet, disjointSet[x]);
            }

            return disjointSet[x];
        }

        private void GlobalSetupForRbacModelWithScale(int groupCount, int userCount, bool withRandomGroups = false)
        {
            GlobalSetupFromFile("rbac_model.conf", "rbac_policy.csv");
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
            if (withRandomGroups)
            {
                NowEnforcer.AddGroupingPolicies(BuildGroupDependencies(groupCount));
            }

            NowEnforcer.BuildRoleLinks();

            NowTestUserName = $"user{userCount / 2 + 1}"; // if 1000 => 501...
            NowTestRoleName = $"group{groupCount / 2 + 1}"; // if 1000 => 501...
            NowTestRole2Name = $"group{groupCount / 10 + 1}";
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set role1 name to {NowTestRoleName}.");
            NowRoleManager = (DefaultRoleManager)NowEnforcer.Model.Sections
                .GetRoleAssertion(PermConstants.Section.RoleSection, PermConstants.DefaultRoleType).RoleManager;
        }

        private void GlobalSetupFromFile(string modelFileName, string policyFileName = null)
        {
            if (policyFileName is null)
            {
                NowEnforcer = new Enforcer(
                    GetTestFilePath(modelFileName));
                NowEnforcer.EnableCache(false);
                return;
            }

            NowEnforcer = new Enforcer(
                GetTestFilePath(modelFileName),
                GetTestFilePath(policyFileName));
            NowEnforcer.EnableCache(false);
            NowRoleManager = (DefaultRoleManager)NowEnforcer.Model.Sections
                .GetRoleAssertion(PermConstants.Section.RoleSection, PermConstants.DefaultRoleType).RoleManager;
        }

        #endregion
    }
}
