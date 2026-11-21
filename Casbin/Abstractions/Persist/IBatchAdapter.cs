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

namespace Casbin.Persist
{
    public interface IBatchAdapter
    {
        void AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        Task AddPoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        void UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        Task UpdatePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        void RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        Task RemovePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        void RemoveFilteredPolicy(string section, string policyType, int fieldIndex, IPolicyValues fieldValues);

        Task RemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex, IPolicyValues fieldValues);
    }
}
