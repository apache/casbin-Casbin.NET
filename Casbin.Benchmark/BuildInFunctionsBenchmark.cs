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
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Casbin.Functions;

namespace Casbin.Benchmark
{
    [MemoryDiagnoser]
    [BenchmarkCategory("Functions")]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net48)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60, baseline: true)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net70)]
    [SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net80)]
    public class BuildInFunctionsBenchmark
    {
        public IEnumerable<object[]> KeyMatch4TestData() => new[]
        {
            new object[] { "/parent/123/child/123", "/parent/{id}/child/{id}" },
            new object[] { "/parent/123/child/123", "/parent/{id}/child/{another_id}" }
        };

        [Benchmark]
        [BenchmarkCategory(nameof(KeyMatch4))]
        [ArgumentsSource(nameof(KeyMatch4TestData))]
        public void KeyMatch4(string arg1, string arg2)
        {
            _ = BuiltInFunctions.KeyMatch4(arg1, arg2);
        }

        public IEnumerable<object[]> IPMatchTestData() => new[]
        {
            new object[] { "192.168.2.123", "192.168.2.123" }, new object[] { "192.168.2.123", "192.168.2.0/24" }
        };

        [Benchmark]
        [BenchmarkCategory(nameof(IPMatch))]
        [ArgumentsSource(nameof(IPMatchTestData))]
        public void IPMatch(string arg1, string arg2)
        {
            _ = BuiltInFunctions.IPMatch(arg1, arg2);
        }
    }
}
