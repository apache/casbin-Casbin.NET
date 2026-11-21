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

namespace Casbin.Model
{
    public interface IPolicyStore : IReadOnlyPolicyStore
    {
        public bool AddNode(string section, string type, PolicyAssertion policyAssertion);

        public bool AddPolicy(string section, string policyType, IPolicyValues values);

        public bool AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public bool UpdatePolicy(string section, string policyType, IPolicyValues oldRule, IPolicyValues newRule);

        public bool UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldRules,
            IReadOnlyList<IPolicyValues> newRules);

        public bool RemovePolicy(string section, string policyType, IPolicyValues rule);

        public bool RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> rules);

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues);

        public bool SortPolicyByPriority(string section, string policyType);

        public bool SortPolicyBySubjectHierarchy(string section, string policyType,
            IDictionary<string, int> subjectHierarchyMap);

        public void ClearPolicy();
    }
}
