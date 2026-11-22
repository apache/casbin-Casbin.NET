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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin
{
    public static class ManagementEnforcerExtension
    {
        /// <summary>
        ///     Adds a customized function.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="name">The name of the new function.</param>
        /// <param name="function">The function.</param>
        public static void AddFunction<T>(this IEnforcer enforcer, string name, T function) where T : Delegate
        {
            enforcer.Model.ExpressionHandler.SetFunction(name, function);
            if (enforcer.AutoCleanEnforceCache)
            {
                enforcer.ClearCache();
            }
        }

        #region "p" (Store) Management

        #region Get Store Items (sub, obj, act)

        /// <summary>
        /// Gets the list of subjects that show up in the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <returns>
        /// All the subjects in "p" policy rules. It actually collects the
        /// 0-index elements of "p" policy rules. So make sure your subject
        /// is the 0-index element, like (sub, obj, act). Duplicates are removed.
        /// </returns>
        public static IEnumerable<string> GetAllSubjects(this IEnforcer enforcer) =>
            GetAllNamedSubjects(enforcer, PermConstants.Section.PolicySection);

        /// <summary>
        /// GetAllNamedSubjects gets the list of subjects that show up in the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the subjects in policy rules of the policyType type. It actually
        /// collects the 0-index elements of the policy rules.So make sure
        /// your subject is the 0-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllNamedSubjects(this IEnforcer enforcer, string policyType) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(PermConstants.Section.PolicySection,
                policyType, 0);

        /// <summary>
        /// Gets the list of objects that show up in the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <returns>
        /// All the objects in "p" policy rules. It actually collects the
        /// 1-index elements of "p" policy rules.So make sure your object
        /// is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllObjects(this IEnforcer enforcer) =>
            GetAllNamedObjects(enforcer, PermConstants.Section.PolicySection);

        /// <summary>
        /// Gets the list of objects that show up in the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the objects in policy rules of the policyType type. It actually
        /// collects the 1-index elements of the policy rules.So make sure
        /// your object is the 1-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllNamedObjects(this IEnforcer enforcer, string policyType) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(PermConstants.DefaultPolicyType,
                policyType, 1);

        /// <summary>
        /// Gets the list of actions that show up in the current policy.
        /// </summary>
        /// <returns>
        /// All the actions in "p" policy rules. It actually collects
        /// the 2-index elements of "p" policy rules.So make sure your action
        /// is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllActions(this IEnforcer enforcer) =>
            GetAllNamedActions(enforcer, PermConstants.Section.PolicySection);

        /// <summary>
        /// Gets the list of actions that show up in the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>
        /// All the actions in policy rules of the policyType type. It actually
        /// collects the 2-index elements of the policy rules.So make sure
        /// your action is the 2-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllNamedActions(this IEnforcer enforcer, string policyType) =>
            enforcer.Model.PolicyStoreHolder.PolicyStore.GetValuesForFieldInPolicy(PermConstants.Section.PolicySection,
                policyType, 2);

        #endregion

        #region Get Store

        /// <summary>
        /// Gets all the authorization rules in the policy.
        /// </summary>
        /// <returns> all the "p" policy rules.</returns>
        public static IEnumerable<IEnumerable<string>> GetPolicy(this IEnforcer enforcer) =>
            enforcer.GetNamedPolicy(PermConstants.Section.PolicySection);

        /// <summary>
        /// Gets all the authorization rules in the named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <returns>The "p" policy rules of the specified policyType.</returns>
        public static IEnumerable<IEnumerable<string>> GetNamedPolicy(this IEnforcer enforcer, string policyType) =>
            enforcer.InternalGetPolicy(PermConstants.Section.PolicySection, policyType);

        /// <summary>
        /// Gets all the authorization rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>The filtered "p" policy rules.</returns>
        public static IEnumerable<IEnumerable<string>> GetFilteredPolicy(this IEnforcer enforcer, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.GetFilteredNamedPolicy(PermConstants.Section.PolicySection, fieldIndex, fieldValues);

        /// <summary>
        /// Gets all the authorization rules in the named policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>The filtered "p" policy rules of the specified policyType.</returns>
        public static IEnumerable<IEnumerable<string>> GetFilteredNamedPolicy(this IEnforcer enforcer,
            string policyType, int fieldIndex, params string[] fieldValues) =>
            enforcer.InternalGetFilteredPolicy(PermConstants.Section.PolicySection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        #endregion End of "p" (Store) Management

        #region Has Store

        /// <summary>
        /// Determines whether an authorization rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasPolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.HasPolicy(values as string[] ?? values.ToArray());

        /// <summary>
        /// Determines whether an authorization rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasPolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.HasNamedPolicy(PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasNamedPolicy(this IEnforcer enforcer, string policyType, IEnumerable<string> values) =>
            enforcer.HasNamedPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Determines whether a named authorization rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasNamedPolicy(this IEnforcer enforcer, string policyType, params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return policyStore.HasPolicy(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        #endregion

        #region Add Store

        /// <summary>
        ///     Adds an authorization rule to the current policy. If the rule
        ///     already exists, the function returns false and the rule will not be added.
        ///     Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddPolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.AddPolicy(values as string[] ?? values.ToArray());

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddPolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.AddNamedPolicy(PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddPolicyAsync(this IEnforcer enforcer, IEnumerable<string> values) =>
            AddPolicyAsync(enforcer, values as string[] ?? values.ToArray());

        /// <summary>
        /// Adds an authorization rule to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddPolicyAsync(this IEnforcer enforcer, params string[] values) =>
            enforcer.AddNamedPolicyAsync(PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedPolicy(this IEnforcer enforcer, string policyType, IEnumerable<string> values) =>
            enforcer.AddNamedPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        ///     Adds an authorization rule to the current named policy.If the
        ///     rule already exists, the function returns false and the rule will not be added.
        ///     Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedPolicy(this IEnforcer enforcer, string policyType, params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalAddPolicy(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        /// Adds an authorization rule to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.AddNamedPolicyAsync(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        ///     Adds an authorization rule to the current named policy.If the
        ///     rule already exists, the function returns false and the rule will not be added.
        ///     Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedPolicyAsync(this IEnforcer enforcer, string policyType, params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalAddPolicyAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        /// Adds authorization rules to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddPolicies(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> values) =>
            AddNamedPolicies(enforcer, PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Adds authorization rules to the current policy. If the rule
        /// already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddPoliciesAsync(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> values) =>
            AddNamedPoliciesAsync(enforcer, PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Adds authorization rules to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="valuesList">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalAddPolicies(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        /// <summary>
        /// Adds authorization rules to the current named policy.If the
        /// rule already exists, the function returns false and the rule will not be added.
        /// Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="valuesList">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalAddPoliciesAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        #endregion

        #region Update Store

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rule to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdatePolicy(this IEnforcer enforcer, IEnumerable<string> oldValues,
            IEnumerable<string> newValues) =>
            enforcer.UpdatePolicy(oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rule to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdatePolicy(this IEnforcer enforcer, IEnumerable<string> oldValues,
            params string[] newValues) =>
            enforcer.UpdateNamedPolicy(PermConstants.DefaultPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rule to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdatePolicyAsync(this IEnforcer enforcer, IEnumerable<string> oldValues,
            IEnumerable<string> newValues) =>
            enforcer.UpdatePolicyAsync(oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rule to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdatePolicyAsync(this IEnforcer enforcer, IEnumerable<string> oldValues,
            params string[] newValues) =>
            enforcer.UpdateNamedPolicyAsync(PermConstants.DefaultPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates an authorization rule to the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, IEnumerable<string> newValues) =>
            enforcer.UpdateNamedPolicy(policyType, oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, params string[] newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalUpdatePolicy(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(oldValues, requiredCount), Policy.ValuesFrom(newValues, requiredCount));
        }


        /// <summary>
        ///     Updates an authorization rule to the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, IEnumerable<string> newValues) =>
            enforcer.UpdateNamedPolicyAsync(policyType, oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates an authorization rule to the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old one.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, params string[] newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalUpdatePolicyAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(oldValues, requiredCount), Policy.ValuesFrom(newValues, requiredCount));
        }

        /// <summary>
        ///     Updates authorization rules to the current policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rules to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rules to replace the old ones, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdatePolicies(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> oldValues,
            IEnumerable<IEnumerable<string>> newValues) =>
            enforcer.UpdateNamedPolicies(PermConstants.DefaultPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates authorization rules to the current policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "p" policy rules to be replaced, policyType "p" is implicitly used.</param>
        /// <param name="newValues">The "p" policy rules to replace the old ones, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdatePoliciesAsync(this IEnforcer enforcer,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues) =>
            enforcer.UpdateNamedPoliciesAsync(PermConstants.DefaultPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates authorization rules to the current named policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old ones.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalUpdatePolicies(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(oldValues), Policy.ValuesListFrom(newValues, requiredCount));
        }

        /// <summary>
        ///     Updates authorization rules to the current named policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="oldValues">The "p" policy rule to be replaced.</param>
        /// <param name="newValues">The "p" policy rule to replace the old ones.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalUpdatePoliciesAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(oldValues), Policy.ValuesListFrom(newValues, requiredCount));
        }

        #endregion

        #region Remove Store

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemovePolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.RemovePolicy(values as string[] ?? values.ToArray());

        /// <summary>
        ///     Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemovePolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.RemoveNamedPolicy(PermConstants.Section.PolicySection, values);

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemovePolicyAsync(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.RemovePolicyAsync(values as string[] ?? values.ToArray());

        /// <summary>
        /// Removes an authorization rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemovePolicyAsync(this IEnforcer enforcer, params string[] values) =>
            enforcer.RemoveNamedPolicyAsync(PermConstants.DefaultPolicyType, values);

        /// <summary>
        ///     Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.RemoveNamedPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedPolicy(this IEnforcer enforcer, string policyType, params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalRemovePolicy(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        ///     Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.RemoveNamedPolicyAsync(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Removes an authorization rule from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalRemovePolicyAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        /// Removes authorization rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemovePolicies(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> values) =>
            enforcer.RemoveNamedPolicies(PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Removes authorization rules from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "p" policy rule, policyType "p" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool>
            RemovePoliciesAsync(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> values) =>
            enforcer.RemoveNamedPoliciesAsync(PermConstants.DefaultPolicyType, values);

        /// <summary>
        /// Removes authorization rules from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalRemovePolicies(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(values, requiredCount));
        }

        /// <summary>
        /// Removes authorization rules from the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="values">The "p" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.PolicySection, policyType);
            return enforcer.InternalRemovePoliciesAsync(PermConstants.Section.PolicySection, policyType,
                Policy.ValuesListFrom(values, requiredCount));
        }

        /// <summary>
        /// Removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveFilteredPolicy(this IEnforcer enforcer, int fieldIndex, params string[] fieldValues) =>
            enforcer.RemoveFilteredNamedPolicy(PermConstants.DefaultPolicyType, fieldIndex, fieldValues);

        /// <summary>
        /// Removes an authorization rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveFilteredPolicyAsync(this IEnforcer enforcer, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.RemoveFilteredNamedPolicyAsync(PermConstants.DefaultPolicyType, fieldIndex, fieldValues);

        /// <summary>
        /// Removes an authorization rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveFilteredNamedPolicy(this IEnforcer enforcer, string policyType, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.InternalRemoveFilteredPolicy(PermConstants.Section.PolicySection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        /// <summary>
        /// Removes an authorization rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "p", "p2", "p3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to  match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveFilteredNamedPolicyAsync(this IEnforcer enforcer, string policyType,
            int fieldIndex, params string[] fieldValues) =>
            enforcer.InternalRemoveFilteredPolicyAsync(PermConstants.Section.PolicySection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        #endregion

        #endregion // End of "p" (Store) Management

        #region "g" (Grouping/Role Store) Management

        #region Get Grouping/Role Store Items (role)

        /// <summary>
        /// Gets the list of roles that show up in the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <returns>
        /// All the roles in "g" policy rules. It actually collects
        /// the 1-index elements of "g" policy rules. So make sure your
        /// role is the 1-index element, like (sub, role).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllRoles(this IEnforcer enforcer) =>
            enforcer.GetAllNamedRoles(PermConstants.Section.RoleSection);

        /// <summary>
        /// Gets the list of roles that show up in the current named policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>
        /// All the subjects in policy rules of the policyType type. It actually
        /// collects the 0-index elements of the policy rules.So make
        /// Sure your subject is the 0-index element, like (sub, obj, act).
        /// Duplicates are removed.</returns>
        public static IEnumerable<string> GetAllNamedRoles(this IEnforcer enforcer, string policyType) =>
            enforcer.InternalGetValuesForFieldInPolicy(PermConstants.Section.RoleSection, policyType, 1);

        #endregion

        #region Get Grouping/Role Store

        /// <summary>
        /// Gets all the role inheritance rules in the policy.
        /// </summary>
        /// <returns>all the "g" policy rules.</returns>
        public static IEnumerable<IEnumerable<string>> GetGroupingPolicy(this IEnforcer enforcer) =>
            enforcer.GetNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType);

        /// <summary>
        /// Gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>The filtered "g" policy rules.</returns>
        public static IEnumerable<IEnumerable<string>> GetFilteredGroupingPolicy(this IEnforcer enforcer,
            int fieldIndex, params string[] fieldValues) =>
            enforcer.GetFilteredNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, fieldIndex,
                fieldValues);

        /// <summary>
        /// Gets all the role inheritance rules in the policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <returns>The "g" policy rules of the specified policyType.</returns>
        public static IEnumerable<IEnumerable<string>> GetNamedGroupingPolicy(this IEnforcer enforcer,
            string policyType) =>
            enforcer.InternalGetPolicy(PermConstants.Section.RoleSection, policyType);

        /// <summary>
        /// Gets all the role inheritance rules in the policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>The filtered "g" policy rules of the specified policyType.</returns>
        public static IEnumerable<IEnumerable<string>> GetFilteredNamedGroupingPolicy(this IEnforcer enforcer,
            string policyType, int fieldIndex, params string[] fieldValues) =>
            enforcer.InternalGetFilteredPolicy(PermConstants.Section.RoleSection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        #endregion

        #region Has Grouping/Role Store

        /// <summary>
        ///     Determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasGroupingPolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.HasGroupingPolicy(values as string[] ?? values.ToArray());

        /// <summary>
        ///     Determines whether a role inheritance rule exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasGroupingPolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.HasNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, values);

        /// <summary>
        ///     Determines whether a named role inheritance rule
        ///     exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.HasNamedGroupingPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        ///     Determines whether a named role inheritance rule
        ///     exists.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Whether the rule exists.</returns>
        public static bool HasNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalHasPolicy(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        #endregion

        #region Add Grouping/Role Store

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddGroupingPolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.AddGroupingPolicy(values as string[] ?? values.ToArray());

        /// <summary>
        ///     Adds a role inheritance rule to the current policy. If the
        ///     rule already exists, the function returns false and the rule will not be
        ///     Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddGroupingPolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.AddNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, values);

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddGroupingPolicyAsync(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.AddGroupingPolicyAsync(values as string[] ?? values.ToArray());

        /// <summary>
        /// Adds a role inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddGroupingPolicyAsync(this IEnforcer enforcer, params string[] values) =>
            enforcer.AddNamedGroupingPolicyAsync(PermConstants.DefaultGroupingPolicyType, values);

        /// <summary>
        /// Adds a named role inheritance rule to the current
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.AddNamedGroupingPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Adds a named role inheritance rule to the current
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedGroupingPolicy(this IEnforcer enforcer, string policyType, params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalAddPolicy(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        ///     Adds a named role inheritance rule to the current
        ///     policy. If the rule already exists, the function returns false and the rule
        ///     will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.AddNamedGroupingPolicyAsync(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Adds a named role inheritance rule to the current
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalAddPolicyAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        /// Adds roles inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddGroupingPolicies(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> valuesList) =>
            enforcer.AddNamedGroupingPolicies(PermConstants.DefaultGroupingPolicyType, valuesList);

        /// <summary>
        /// Adds roles inheritance rule to the current policy. If the
        /// rule already exists, the function returns false and the rule will not be
        /// Added.Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddGroupingPoliciesAsync(this IEnforcer enforcer,
            IEnumerable<IEnumerable<string>> valuesList) =>
            enforcer.AddNamedGroupingPoliciesAsync(PermConstants.DefaultGroupingPolicyType, valuesList);

        /// <summary>
        /// Adds named roles inheritance rule to the current
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="valuesList">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool AddNamedGroupingPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalAddPolicies(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        /// <summary>
        /// Adds named roles inheritance rule to the current
        /// policy. If the rule already exists, the function returns false and the rule
        /// will not be added. Otherwise the function returns true by adding the new rule.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="valuesList">The "g" policy rule.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> AddNamedGroupingPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalAddPoliciesAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        #endregion

        #region Update Grouping/Role Store

        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateGroupingPolicy(this IEnforcer enforcer, IEnumerable<string> oldValues,
            IEnumerable<string> newValues) =>
            enforcer.UpdateGroupingPolicy(oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateGroupingPolicy(this IEnforcer enforcer, IEnumerable<string> oldValues,
            params string[] newValues) =>
            enforcer.UpdateNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateGroupingPolicyAsync(this IEnforcer enforcer, IEnumerable<string> oldValues,
            IEnumerable<string> newValues) =>
            enforcer.UpdateGroupingPolicyAsync(oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateGroupingPolicyAsync(this IEnforcer enforcer, IEnumerable<string> oldValues,
            params string[] newValues) =>
            enforcer.UpdateNamedGroupingPolicyAsync(PermConstants.DefaultGroupingPolicyType, oldValues,
                newValues);

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, IEnumerable<string> newValues) =>
            enforcer.UpdateNamedGroupingPolicy(policyType, oldValues, newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, params string[] newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalUpdatePolicy(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(oldValues, requiredCount), Policy.ValuesFrom(newValues, requiredCount));
        }

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, IEnumerable<string> newValues) =>
            enforcer.UpdateNamedGroupingPolicyAsync(policyType, oldValues,
                newValues as string[] ?? newValues.ToArray());

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> oldValues, params string[] newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalUpdatePolicyAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(oldValues, requiredCount), Policy.ValuesFrom(newValues, requiredCount));
        }


        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old ones, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateGroupingPolicies(this IEnforcer enforcer, IEnumerable<IEnumerable<string>> oldValues,
            IEnumerable<IEnumerable<string>> newValues) =>
            enforcer.UpdateNamedGroupingPolicies(PermConstants.DefaultGroupingPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old one, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateGroupingPoliciesAsync(this IEnforcer enforcer,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues) =>
            enforcer.UpdateNamedGroupingPoliciesAsync(PermConstants.DefaultGroupingPolicyType, oldValues, newValues);

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old ones, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool UpdateNamedGroupingPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalUpdatePolicies(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(oldValues), Policy.ValuesListFrom(newValues, requiredCount));
        }

        /// <summary>
        ///     Updates a role inheritance rule from the current policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="oldValues">The "g" policy rule to be replaced, policyType "g" is implicitly used.</param>
        /// <param name="newValues">The "g" policy rule to replace the old ones, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> UpdateNamedGroupingPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> oldValues, IEnumerable<IEnumerable<string>> newValues)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalUpdatePoliciesAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(oldValues), Policy.ValuesListFrom(newValues, requiredCount));
        }

        #endregion

        #region Remove Grouping/Role Store

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveGroupingPolicy(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.RemoveGroupingPolicy(values as string[] ?? values.ToArray());

        /// <summary>
        ///     Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveGroupingPolicy(this IEnforcer enforcer, params string[] values) =>
            enforcer.RemoveNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, values);

        /// <summary>
        /// Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveGroupingPolicyAsync(this IEnforcer enforcer, IEnumerable<string> values) =>
            enforcer.RemoveGroupingPolicyAsync(values as string[] ?? values.ToArray());

        /// <summary>
        ///     Removes a role inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveGroupingPolicyAsync(this IEnforcer enforcer, params string[] values) =>
            enforcer.RemoveNamedGroupingPolicyAsync(PermConstants.DefaultGroupingPolicyType, values);

        /// <summary>
        ///     Removes a role inheritance rule from the current
        ///     policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.RemoveNamedGroupingPolicy(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Removes a role inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedGroupingPolicy(this IEnforcer enforcer, string policyType,
            params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalRemovePolicy(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        ///     Removes a role inheritance rule from the current
        ///     policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<string> values) =>
            enforcer.RemoveNamedGroupingPolicyAsync(policyType, values as string[] ?? values.ToArray());

        /// <summary>
        /// Removes a role inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="values">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedGroupingPolicyAsync(this IEnforcer enforcer, string policyType,
            params string[] values)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalRemovePolicyAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesFrom(values, requiredCount));
        }

        /// <summary>
        /// Removes roles inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveGroupingPolicies(this IEnforcer enforcer,
            IEnumerable<IEnumerable<string>> valuesList) =>
            enforcer.RemoveNamedGroupingPolicies(PermConstants.DefaultGroupingPolicyType, valuesList);

        /// <summary>
        /// Removes roles inheritance rule from the current policy.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveGroupingPoliciesAsync(this IEnforcer enforcer,
            IEnumerable<IEnumerable<string>> valuesList) =>
            enforcer.RemoveNamedGroupingPoliciesAsync(PermConstants.DefaultGroupingPolicyType, valuesList);

        /// <summary>
        /// Removes roles inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveNamedGroupingPolicies(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalRemovePolicies(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        /// <summary>
        /// Removes roles inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="valuesList">The "g" policy rule, policyType "g" is implicitly used.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveNamedGroupingPoliciesAsync(this IEnforcer enforcer, string policyType,
            IEnumerable<IEnumerable<string>> valuesList)
        {
            IPolicyStore policyStore = enforcer.Model.PolicyStoreHolder.PolicyStore;
            int requiredCount = policyStore.GetRequiredValuesCount(PermConstants.Section.RoleSection, policyType);
            return enforcer.InternalRemovePoliciesAsync(PermConstants.Section.RoleSection, policyType,
                Policy.ValuesListFrom(valuesList, requiredCount));
        }

        /// <summary>
        /// Removes a role inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveFilteredGroupingPolicy(this IEnforcer enforcer, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.RemoveFilteredNamedGroupingPolicy(PermConstants.DefaultGroupingPolicyType, fieldIndex,
                fieldValues);

        /// <summary>
        /// Removes a role inheritance rule from the current
        /// policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveFilteredGroupingPolicyAsync(this IEnforcer enforcer, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.RemoveFilteredNamedGroupingPolicyAsync(PermConstants.DefaultGroupingPolicyType, fieldIndex,
                fieldValues);

        /// <summary>
        /// Removes a role inheritance rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static bool RemoveFilteredNamedGroupingPolicy(this IEnforcer enforcer, string policyType, int fieldIndex,
            params string[] fieldValues) =>
            enforcer.InternalRemoveFilteredPolicy(PermConstants.Section.RoleSection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        /// <summary>
        /// Removes a role inheritance rule from the current named policy, field filters can be specified.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="policyType">The policy type, can be "g", "g2", "g3", ..</param>
        /// <param name="fieldIndex">The policy rule's start index to be matched.</param>
        /// <param name="fieldValues">The field values to be matched, value "" means not to match this field.</param>
        /// <returns>Succeeds or not.</returns>
        public static Task<bool> RemoveFilteredNamedGroupingPolicyAsync(this IEnforcer enforcer,
            string policyType, int fieldIndex, params string[] fieldValues) =>
            enforcer.InternalRemoveFilteredPolicyAsync(PermConstants.Section.RoleSection, policyType, fieldIndex,
                Policy.ValuesFrom(fieldValues));

        #endregion

        #endregion // End of "g" (Grouping/Role Store) Management
    }
}
