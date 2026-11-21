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
using Casbin.Persist;
#if !NET452
#endif

namespace Casbin
{
    internal static partial class InternalEnforcerExtension
    {
        internal static IEnumerable<IPolicyValues> InternalGetPolicy(this IEnforcer enforcer, string section,
            string policyType) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetPolicy(section, policyType);

        internal static IEnumerable<IPolicyValues> InternalGetFilteredPolicy(this IEnforcer enforcer,
            string section, string policyType, int fieldIndex, IPolicyValues fieldValues) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetFilteredPolicy(section, policyType, fieldIndex,
                fieldValues);

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicy(this IEnforcer enforcer, string section,
            string policyType, int fieldIndex) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(section, policyType, fieldIndex);

        internal static IEnumerable<string> InternalGetValuesForFieldInPolicyAllTypes(this IEnforcer enforcer,
            string section, int fieldIndex) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicyAllTypes(section, fieldIndex);

        internal static bool InternalHasPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues rule) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.HasPolicy(section, policyType, rule);

        internal static bool InternalHasPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.HasPolicies(section, policyType, rules);

        /// <summary>
        /// Adds a values to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues values)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(values))
            {
                return false;
            }

            bool ruleAdded = policyManager.AddPolicy(values);
            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyChangedMessage.CreateAddPolicy(section, policyType, values));
            return true;
        }

        /// <summary>
        /// Adds a values to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues values)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(values))
            {
                return false;
            }

            bool ruleAdded = await policyManager.AddPolicyAsync(values);
            if (ruleAdded is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyChangedMessage.CreateAddPolicy(section, policyType, values));
            return true;
        }

        /// <summary>
        /// Adds valuesList to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        internal static bool InternalAddPolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> valuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicies(valuesList))
            {
                return false;
            }

            bool ruleAdded = policyManager.AddPolicies(valuesList);
            if (ruleAdded is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyChangedMessage.CreateAddPolicies(section, policyType, valuesList));
            return true;
        }


        /// <summary>
        /// Adds valuesList to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="valuesList"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalAddPoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicies(valuesList))
            {
                return false;
            }

            bool ruleAdded = await policyManager.AddPoliciesAsync(valuesList);
            if (ruleAdded is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer,
                PolicyChangedMessage.CreateAddPolicies(section, policyType, valuesList));
            return true;
        }

        /// <summary>
        ///     Updates a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues oldValues, IPolicyValues newValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(oldValues) is false)
            {
                return false;
            }

            bool ruleUpdated = policyManager.UpdatePolicy(oldValues, newValues);
            if (ruleUpdated is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer,
                PolicyChangedMessage.CreateUpdatePolicy(section, policyType, oldValues, newValues));
            return true;
        }

        /// <summary>
        /// Updates a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues oldValues, IPolicyValues newValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(oldValues) is false)
            {
                return false;
            }

            bool ruleUpdated = await policyManager.UpdatePolicyAsync(oldValues, newValues);
            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer,
                PolicyChangedMessage.CreateUpdatePolicy(section, policyType, oldValues, newValues));
            return true;
        }

        /// <summary>
        ///     Updates valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValuesList"></param>
        /// <param name="newValuesList"></param>
        /// <returns></returns>
        internal static bool InternalUpdatePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> oldValuesList, IReadOnlyList<IPolicyValues> newValuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasAllPolicies(oldValuesList) is false)
            {
                return false;
            }

            bool ruleUpdated = policyManager.UpdatePolicies(oldValuesList, newValuesList);
            if (ruleUpdated is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer,
                PolicyChangedMessage.CreateUpdatePolicies(section, policyType, oldValuesList, newValuesList));
            return true;
        }

        /// <summary>
        ///     Updates valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="oldValuesList"></param>
        /// <param name="newValuesList"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalUpdatePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> oldValuesList, IReadOnlyList<IPolicyValues> newValuesList)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasAllPolicies(oldValuesList) is false)
            {
                return false;
            }

            bool ruleUpdated = await policyManager.UpdatePoliciesAsync(oldValuesList, newValuesList);
            if (ruleUpdated is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer,
                PolicyChangedMessage.CreateUpdatePolicies(section, policyType, oldValuesList, newValuesList));
            return true;
        }

        /// <summary>
        /// Removes a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicy(this IEnforcer enforcer, string section, string policyType,
            IPolicyValues values)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(values) is false)
            {
                return false;
            }

            bool ruleRemoved = policyManager.RemovePolicy(values);
            if (ruleRemoved is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyChangedMessage.CreateRemovePolicy(section, policyType, values));
            return true;
        }

        /// <summary>
        /// Removes a values from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePolicyAsync(this IEnforcer enforcer, string section,
            string policyType, IPolicyValues rule)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicy(rule) is false)
            {
                return false;
            }

            bool ruleRemoved = await policyManager.RemovePolicyAsync(rule);
            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer, PolicyChangedMessage.CreateRemovePolicy(section, policyType, rule));
            return true;
        }

        /// <summary>
        /// Removes valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static bool InternalRemovePolicies(this IEnforcer enforcer, string section, string policyType,
            IReadOnlyList<IPolicyValues> rules)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicies(rules) is false)
            {
                return false;
            }

            bool ruleRemoved = policyManager.RemovePolicies(rules);
            if (ruleRemoved is false)
            {
                return false;
            }

            OnPolicyChanged(enforcer, PolicyChangedMessage.CreateRemovePolicies(section, policyType, rules));
            return true;
        }

        /// <summary>
        /// Removes valuesList from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="rules"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemovePoliciesAsync(this IEnforcer enforcer, string section,
            string policyType, IReadOnlyList<IPolicyValues> rules)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            if (policyManager.HasPolicies(rules) is false)
            {
                return false;
            }

            bool ruleRemoved = await policyManager.RemovePoliciesAsync(rules);
            if (ruleRemoved is false)
            {
                return false;
            }

            await OnPolicyAsyncChanged(enforcer,
                PolicyChangedMessage.CreateRemovePolicies(section, policyType, rules));
            return true;
        }

        /// <summary>
        /// Removes valuesList based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static bool InternalRemoveFilteredPolicy(this IEnforcer enforcer, string section, string policyType,
            int fieldIndex, IPolicyValues fieldValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            IEnumerable<IPolicyValues> effectPolicies = policyManager.RemoveFilteredPolicy(fieldIndex, fieldValues);

            if (effectPolicies is null)
            {
                return false;
            }

            IReadOnlyList<IPolicyValues> rules = effectPolicies as IReadOnlyList<IPolicyValues> ??
                                                 effectPolicies.ToList();
            OnPolicyChanged(enforcer,
                PolicyChangedMessage.CreateRemoveFilteredPolicy(section, policyType, fieldIndex, rules));
            return true;
        }

        /// <summary>
        /// Removes valuesList based on field filters from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="section"></param>
        /// <param name="policyType"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        internal static async Task<bool> InternalRemoveFilteredPolicyAsync(this IEnforcer enforcer, string section,
            string policyType, int fieldIndex, IPolicyValues fieldValues)
        {
            IPolicyManager policyManager = enforcer.Model.GetPolicyManager(section, policyType);
            IEnumerable<IPolicyValues> effectPolicies =
                await policyManager.RemoveFilteredPolicyAsync(fieldIndex, fieldValues);

            if (effectPolicies is null)
            {
                return false;
            }

            IReadOnlyList<IPolicyValues> rules = effectPolicies as IReadOnlyList<IPolicyValues> ??
                                                 effectPolicies.ToList();
            await OnPolicyAsyncChanged(enforcer,
                PolicyChangedMessage.CreateRemoveFilteredPolicy(section, policyType, fieldIndex, rules));
            return true;
        }
    }
}
