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
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Casbin.UnitTests.ParallelTest
{
    using RbacRequest = RequestValues<string, string, string, string>;

    [Collection("Model collection")]
    public class SyncParallelTest
    {
        private readonly ITestOutputHelper _output;
        private readonly TestModelFixture TestModelFixture;
        private RbacParallelTestHelper<RbacRequest> _rbacParallelTestHelper;

        public SyncParallelTest(ITestOutputHelper output, TestModelFixture testModelFixture)
        {
            _output = output;
            TestModelFixture = testModelFixture;
        }

        private void InitRbacParallelTestHelper()
        {
            Enforcer e1 = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
            e1.BuildRoleLinks();
            Enforcer e2 = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
            e2.BuildRoleLinks();
            Enforcer e3 = new(TestModelFixture.GetNewRbacWithDomainsTestModel());
            e3.BuildRoleLinks();
            var consumer = new DefaultRbacConsumer<RbacRequest>(e1);
            var transactionFactory = new DefaultTransactionFactory();
            RandomRequestGenerator<RbacRequest> rdg = new RandomRequestGenerator<RbacRequest>(
                DefaultExistedEntropyPool.Create4(e2),
                DefaultRandomEntropyPool.Create4(
                    new KeyValuePair<string, int>("sub", 10),
                    new KeyValuePair<string, int>("dom", 10),
                    new KeyValuePair<string, int>("obj", 10),
                    new KeyValuePair<string, int>("act", 10)
                ),
                new PureExistedEntropyHandler<RbacRequest>(50),
                new PureRandomEntropyHandler<RbacRequest>(30)
            );
            _rbacParallelTestHelper = new RbacParallelTestHelper<RbacRequest>(
                consumer, e3, transactionFactory, rdg
            );
        }

        [Fact]
        public async Task RbacWithDomainsSyncParallelTestAsync()
        {
            InitRbacParallelTestHelper();
            await _rbacParallelTestHelper.TestCorrectness(2000, 1000, 0, 1000);
        }
    }
}
