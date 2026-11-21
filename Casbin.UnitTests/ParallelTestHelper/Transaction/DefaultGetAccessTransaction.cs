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
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultGetAccessTransaction<TRequest> : ITransaction<TRequest> where TRequest : IRequestValues
    {
        private List<TRequest> _requests = [];

        public DefaultGetAccessTransaction(TRequest request)
        {
            _requests.Add(request);
        }

        public bool ExpectedResult { get; private set; } = false;
        public bool ActualResult { get; private set; } = true;
        public bool HasCompleted { get; private set; } = false;

        public async Task<bool> ExecuteAsync(IConsumer<TRequest> consumer)
        {
            ActualResult = await consumer.GetAccessAsync(Request.First());
            HasCompleted = true;
            return true;
        }

        public void SetTruth(IEnforcer enforcer)
        {
            ExpectedResult = enforcer.Enforce(enforcer.CreateContext(), Request.First());
        }

        public IEnumerable<TRequest> Request
        {
            get { return _requests; }
        }

        public TransactionType TransactionType { get; } = TransactionType.GetAccess;
    }
}
