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
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Model")]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net70)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
    public class ModelBenchmark
    {
        private readonly Enforcer _enforcer;

        public ModelBenchmark()
        {
            _enforcer = new Enforcer(TestHelper.GetTestFilePath("rbac_model.conf"));
        }

        private string NowTestUserName { get; set; }
        private string NowTestDataName { get; set; }

        [Params(10, 100, 1000, 10000)] public int NowPolicyCount { get; set; }

        [GlobalSetup(Targets = new[] { nameof(AddPolicy), nameof(HasPolicy), nameof(RemovePolicy) })]
        public void GlobalSetup()
        {
            for (int i = 0; i < NowPolicyCount; i++)
            {
                _enforcer.AddPolicy($"group{i}", $"obj{i / 10}", "read");
            }

            Console.WriteLine($"// Already set {NowPolicyCount} policies.");

            NowTestUserName = $"name{NowPolicyCount / 2 + 1}";
            NowTestDataName = $"data{NowPolicyCount / 2 + 1}";
            Console.WriteLine($"// Already set user name to {NowTestUserName}.");
            Console.WriteLine($"// Already set data name to {NowTestDataName}.");
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public void HasPolicy()
        {
            _enforcer.HasPolicy(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public void AddPolicy()
        {
            _enforcer.AddPolicy(NowTestUserName, NowTestDataName, "read");
        }

        [Benchmark]
        [BenchmarkCategory("ModelManagement")]
        public void RemovePolicy()
        {
            _enforcer.RemovePolicy(NowTestUserName, NowTestDataName, "read");
        }
    }
}
