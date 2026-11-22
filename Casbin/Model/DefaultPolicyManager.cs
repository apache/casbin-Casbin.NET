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
using Casbin.Model.Holder;
using JetBrains.Annotations;

namespace Casbin.Model
{
    [PublicAPI]
    public class DefaultPolicyManager : IPolicyManager
    {
        private readonly AdapterHolder _adapterHolder;
        private readonly PolicyStoreHolder _policyStoreHolder;
        private readonly string _policyType;
        private readonly string _section;

        internal DefaultPolicyManager(string section, string policyType,
            PolicyStoreHolder policyStoreHolder, AdapterHolder adapterHolder)
        {
            _section = section;
            _policyType = policyType;
            _policyStoreHolder = policyStoreHolder;
            _adapterHolder = adapterHolder;
        }

        private bool HasAdapter => _adapterHolder.Adapter is not null;

        public bool AutoSave { get; set; } = true;

        public PolicyScanner Scan() =>
            _policyStoreHolder.PolicyStore.Scan(_section, _policyType);

        public IEnumerable<IPolicyValues> GetPolicy() =>
            _policyStoreHolder.PolicyStore.GetPolicy(_section, _policyType);

        public IEnumerable<IPolicyValues> GetFilteredPolicy(int fieldIndex, IPolicyValues fieldValues) =>
            _policyStoreHolder.PolicyStore.GetFilteredPolicy(_section, _policyType, fieldIndex, fieldValues);

        public IEnumerable<string> GetValuesForFieldInPolicy(int fieldIndex) =>
            _policyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(_section, _policyType, fieldIndex);

        public bool HasPolicy(IPolicyValues values) =>
            _policyStoreHolder.PolicyStore.HasPolicy(_section, _policyType, values);

        public bool HasPolicies(IReadOnlyList<IPolicyValues> valueList) =>
            _policyStoreHolder.PolicyStore.HasPolicies(_section, _policyType, valueList);

        public bool HasAllPolicies(IReadOnlyList<IPolicyValues> rules) =>
            _policyStoreHolder.PolicyStore.HasAllPolicies(_section, _policyType, rules);

        public virtual bool AddPolicy(IPolicyValues values)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.AddPolicy(_section, _policyType, values);
            }

            _adapterHolder.SingleAdapter?.AddPolicy(_section, _policyType, values);
            return _policyStoreHolder.PolicyStore.AddPolicy(_section, _policyType, values);
        }

        public virtual bool AddPolicies(IReadOnlyList<IPolicyValues> rules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.AddPolicies(_section, _policyType, rules);
            }

            _adapterHolder.BatchAdapter?.AddPolicies(_section, _policyType, rules);
            return _policyStoreHolder.PolicyStore.AddPolicies(_section, _policyType, rules);
        }

        public virtual bool UpdatePolicy(IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.UpdatePolicy(_section, _policyType, oldRule, newRule);
            }

            _adapterHolder.SingleAdapter?.UpdatePolicy(_section, _policyType, oldRule, newRule);
            return _policyStoreHolder.PolicyStore.UpdatePolicy(_section, _policyType, oldRule, newRule);
        }

        public virtual bool UpdatePolicies(IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.UpdatePolicies(_section, _policyType, oldRules, newRules);
            }

            _adapterHolder.BatchAdapter?.UpdatePolicies(_section, _policyType, oldRules, newRules);
            return _policyStoreHolder.PolicyStore.UpdatePolicies(_section, _policyType, oldRules, newRules);
        }

        public virtual bool RemovePolicy(IPolicyValues rule)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemovePolicy(_section, _policyType, rule);
            }

            _adapterHolder.SingleAdapter?.RemovePolicy(_section, _policyType, rule);
            return _policyStoreHolder.PolicyStore.RemovePolicy(_section, _policyType, rule);
        }

        public virtual bool RemovePolicies(IReadOnlyList<IPolicyValues> rules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemovePolicies(_section, _policyType, rules);
            }

            _adapterHolder.BatchAdapter?.RemovePolicies(_section, _policyType, rules);
            return _policyStoreHolder.PolicyStore.RemovePolicies(_section, _policyType, rules);
        }

        public virtual IEnumerable<IPolicyValues> RemoveFilteredPolicy(int fieldIndex, IPolicyValues fieldValues)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemoveFilteredPolicy(_section, _policyType, fieldIndex,
                    fieldValues);
            }

            _adapterHolder.BatchAdapter?.RemoveFilteredPolicy(_section, _policyType, fieldIndex, fieldValues);
            return _policyStoreHolder.PolicyStore.RemoveFilteredPolicy(_section, _policyType, fieldIndex, fieldValues);
        }

        public virtual async Task<bool> AddPolicyAsync(IPolicyValues rule)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.AddPolicy(_section, _policyType, rule);
            }

            if (_adapterHolder.SingleAdapter is not null)
            {
                await _adapterHolder.SingleAdapter.AddPolicyAsync(_section, _policyType, rule);
            }

            return _policyStoreHolder.PolicyStore.AddPolicy(_section, _policyType, rule);
        }

        public virtual async Task<bool> AddPoliciesAsync(
            IReadOnlyList<IPolicyValues> rules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.AddPolicies(_section, _policyType, rules);
            }

            if (_adapterHolder.BatchAdapter is not null)
            {
                await _adapterHolder.BatchAdapter.AddPoliciesAsync(_section, _policyType, rules);
            }

            return _policyStoreHolder.PolicyStore.AddPolicies(_section, _policyType, rules);
        }

        public virtual async Task<bool> UpdatePolicyAsync(
            IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.UpdatePolicy(_section, _policyType, oldRule, newRule);
            }

            if (_adapterHolder.SingleAdapter is not null)
            {
                await _adapterHolder.SingleAdapter.UpdatePolicyAsync(_section, _policyType, oldRule, newRule);
            }

            return _policyStoreHolder.PolicyStore.UpdatePolicy(_section, _policyType, oldRule, newRule);
        }

        public virtual async Task<bool> UpdatePoliciesAsync(
            IReadOnlyList<IPolicyValues> oldRules, IReadOnlyList<IPolicyValues> newRules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.UpdatePolicies(_section, _policyType, oldRules, newRules);
            }

            if (_adapterHolder.BatchAdapter is not null)
            {
                await _adapterHolder.BatchAdapter.UpdatePoliciesAsync(_section, _policyType, oldRules, newRules);
            }

            return _policyStoreHolder.PolicyStore.UpdatePolicies(_section, _policyType, oldRules, newRules);
        }

        public virtual async Task<bool> RemovePolicyAsync(IPolicyValues rule)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemovePolicy(_section, _policyType, rule);
            }

            if (_adapterHolder.SingleAdapter is not null)
            {
                await _adapterHolder.SingleAdapter.RemovePolicyAsync(_section, _policyType, rule);
            }

            return _policyStoreHolder.PolicyStore.RemovePolicy(_section, _policyType, rule);
        }

        public virtual async Task<bool> RemovePoliciesAsync(
            IReadOnlyList<IPolicyValues> rules)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemovePolicies(_section, _policyType, rules);
            }

            if (_adapterHolder.BatchAdapter is not null)
            {
                await _adapterHolder.BatchAdapter.RemovePoliciesAsync(_section, _policyType, rules);
            }

            return _policyStoreHolder.PolicyStore.RemovePolicies(_section, _policyType, rules);
        }

        public virtual async Task<IEnumerable<IPolicyValues>> RemoveFilteredPolicyAsync(
            int fieldIndex, IPolicyValues fieldValues)
        {
            if (HasAdapter is false || AutoSave is false)
            {
                return _policyStoreHolder.PolicyStore.RemoveFilteredPolicy(_section, _policyType, fieldIndex,
                    fieldValues);
            }

            if (_adapterHolder.BatchAdapter is not null)
            {
                await _adapterHolder.BatchAdapter.RemoveFilteredPolicyAsync(_section, _policyType, fieldIndex,
                    fieldValues);
            }

            return _policyStoreHolder.PolicyStore.RemoveFilteredPolicy(_section, _policyType, fieldIndex, fieldValues);
        }

        public Task<IEnumerable<IPolicyValues>> GetPolicyAsync() =>
            Task.FromResult(_policyStoreHolder.PolicyStore.GetPolicy(_section, _policyType));

        public void ClearPolicy() => _policyStoreHolder.PolicyStore.ClearPolicy();
    }
}
