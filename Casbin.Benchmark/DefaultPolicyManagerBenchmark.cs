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
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Casbin.Model;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Model")]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net48)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net70)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
    public class DefaultPolicyManagerBenchmark
    {
        private readonly Enforcer _enforcer;
        private readonly DefaultPolicyManager _policyManager;

        public DefaultPolicyManagerBenchmark()
        {
            _enforcer = new Enforcer(TestHelper.GetTestFilePath("rbac_model.conf"));
            _policyManager = (DefaultPolicyManager)_enforcer.Model.Sections
                .GetPolicyAssertion(PermConstants.DefaultPolicyType).PolicyManager;
        }

        private string NowTestUserName { get; set; }
        private string NowTestDataName { get; set; }
        private IPolicyValues NowTestPolicy { get; set; }
        private List<IPolicyValues> NowTestExistedPolicyList { get; set; } = new List<IPolicyValues>();
        private List<IPolicyValues> NowTestNullPolicyList { get; set; } = new List<IPolicyValues>();

        [Params(10, 100, 1000, 10000)] public int NowPolicyCount { get; set; }

        [GlobalSetup(Targets = new[]
        {
            nameof(AddPolicyAsync), nameof(RemovePolicyAsync), nameof(UpdatePolicyAsync),
            nameof(RemovePoliciesAsync), nameof(AddPoliciesAsync), nameof(UpdatePoliciesAsync)
        })]
        public void GlobalSetup()
        {
            var rd = new Random();
            for (int i = 0; i < NowPolicyCount; i++)
            {
                _enforcer.AddPolicy($"group{i}", $"obj{i / 10}", "read");
                int num = rd.Next(1000);
                if (num == 0)
                {
                    NowTestExistedPolicyList.Add(
                        new PolicyValues<string, string, string>($"group{i}", $"obj{i / 10}", "read"));
                    NowTestNullPolicyList.Add(
                        new PolicyValues<string, string, string>($"name{i}", $"data{i / 10}", "read"));
                }
            }

            Console.WriteLine($"// Already set {NowPolicyCount} policies.");

            NowTestUserName = $"name{NowPolicyCount / 2 + 1}";
            NowTestDataName = $"data{NowPolicyCount / 2 + 1}";
            NowTestPolicy = new PolicyValues<string, string, string>(NowTestUserName, NowTestDataName, "read");
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set data name to {NowTestDataName}.");
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task AddPolicyAsync()
        {
            await _policyManager.AddPolicyAsync(NowTestPolicy);
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task RemovePolicyAsync()
        {
            await _policyManager.RemovePolicyAsync(NowTestPolicy);
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task UpdatePolicyAsync()
        {
            await _policyManager.UpdatePolicyAsync(NowTestPolicy,
                new PolicyValues<string, string, string>(NowTestUserName + "up", NowTestDataName + "up", "read"));
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task RemovePoliciesAsync()
        {
            await _policyManager.RemovePoliciesAsync(NowTestExistedPolicyList);
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task AddPoliciesAsync()
        {
            await _policyManager.AddPoliciesAsync(NowTestNullPolicyList);
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public async Task UpdatePoliciesAsync()
        {
            await _policyManager.UpdatePoliciesAsync(NowTestExistedPolicyList, NowTestNullPolicyList);
        }
    }
}
