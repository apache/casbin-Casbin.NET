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
using Casbin.Model;
using Casbin.UnitTests.Extensions;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultRbacConsumer<TRequest> : IConsumer<TRequest> where TRequest : IRequestValues
    {
        private Enforcer _enforcer;

        public DefaultRbacConsumer(Enforcer enforcer)
        {
            _enforcer = enforcer;
        }

        public DefaultRbacConsumer(string modelPath, string policyPath)
        {
            if (!string.IsNullOrEmpty(modelPath) && !string.IsNullOrEmpty(policyPath))
            {
                _enforcer = new Enforcer(modelPath, policyPath);
            }
            else
            {
                _enforcer = new Enforcer();
            }

            _enforcer.BuildRoleLinks();
        }

        public async Task<bool> GetAccessAsync(TRequest request)
        {
            return await _enforcer.EnforceAsync(_enforcer.CreateContext(), request);
        }

        public Task<bool> UpdatePolicyAsync(TRequest oldRequest, TRequest newRequest) =>
            // TODO
            Task.FromResult(true);

        public async Task<bool> RemovePolicyAsync(TRequest request)
        {
            return await _enforcer.RemovePolicyAsync(request.ToEnumerable());
        }

        public async Task<bool> AddPolicyAsync(TRequest request)
        {
            return await _enforcer.AddPolicyAsync(request.ToEnumerable());
        }
    }
}
