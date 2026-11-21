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

namespace Casbin.Model
{
    public interface IPolicyManager : IReadOnlyPolicyManager
    {
        public bool AutoSave { get; set; }

        public bool AddPolicy(IPolicyValues rule);

        public Task<bool> AddPolicyAsync(IPolicyValues rule);

        public bool AddPolicies(IReadOnlyList<IPolicyValues> rules);

        public Task<bool> AddPoliciesAsync(IReadOnlyList<IPolicyValues> rules);

        public bool UpdatePolicy(IPolicyValues oldRule,
            IPolicyValues newRule);

        public Task<bool> UpdatePolicyAsync(IPolicyValues oldRule,
            IPolicyValues newRule);

        public bool UpdatePolicies(IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules);

        public Task<bool> UpdatePoliciesAsync(IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        public bool RemovePolicy(IPolicyValues rule);

        public Task<bool> RemovePolicyAsync(IPolicyValues rule);

        public bool RemovePolicies(IReadOnlyList<IPolicyValues> rules);

        public Task<bool> RemovePoliciesAsync(IReadOnlyList<IPolicyValues> rules);

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(int fieldIndex, IPolicyValues fieldValues);

        public Task<IEnumerable<IPolicyValues>> RemoveFilteredPolicyAsync(int fieldIndex, IPolicyValues fieldValues);
    }
}
