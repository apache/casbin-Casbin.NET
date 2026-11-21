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
using System.Threading.Tasks;
using Casbin.Caching;
using Casbin.Effect;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Rbac;
#if !NET452
using Microsoft.Extensions.Logging;
#endif

namespace Casbin
{
    public static partial class EnforcerExtension
    {
        #region Model management

        /// <summary>
        ///     LoadModel reloads the model from the model CONF file. Because the policy is
        ///     Attached to a model, so the policy is invalidated and needs to be reloaded by
        ///     calling LoadPolicy().
        /// </summary>
        public static void LoadModel(this IEnforcer enforcer)
        {
            if (enforcer.Model.Path is null)
            {
                return;
            }

            enforcer.Model = DefaultModel.CreateFromFile(enforcer.Model.Path);
            enforcer.Model.RegisterDomainMatchingFunc();
        }

        #endregion

        #region Set options

        /// <summary>
        /// Changes the enforcing state of Casbin, when Casbin is disabled,
        /// all access will be allowed by the enforce() function.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="enable"></param>
        public static IEnforcer EnableEnforce(this IEnforcer enforcer, bool enable)
        {
            enforcer.Enabled = enable;
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically to the
        /// adapter when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoSave"></param>
        public static IEnforcer EnableAutoSave(this IEnforcer enforcer, bool autoSave)
        {
            enforcer.Model.SetAutoSave(autoSave);
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// to the adapter when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoBuildRoleLinks">Whether to automatically build the role links.</param>
        public static IEnforcer EnableAutoBuildRoleLinks(this IEnforcer enforcer, bool autoBuildRoleLinks)
        {
            enforcer.AutoBuildRoleLinks = autoBuildRoleLinks;
            return enforcer;
        }

        /// <summary>
        /// Controls whether to save a policy rule automatically
        /// notify the Watcher when it is added or removed.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="autoNotifyWatcher">Whether to automatically notify watcher.</param>
        public static IEnforcer EnableAutoNotifyWatcher(this IEnforcer enforcer, bool autoNotifyWatcher)
        {
            enforcer.AutoNotifyWatcher = autoNotifyWatcher;
            return enforcer;
        }

        public static IEnforcer EnableCache(this IEnforcer enforcer, bool enableCache)
        {
            enforcer.EnabledCache = enableCache;
            return enforcer;
        }

        public static IEnforcer EnableAutoCleanEnforceCache(this IEnforcer enforcer, bool autoCleanEnforceCache)
        {
            enforcer.AutoCleanEnforceCache = autoCleanEnforceCache;
            return enforcer;
        }

        #endregion

        #region Set extensions

        /// <summary>
        /// Sets the current effector.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="effector"></param>
        public static IEnforcer SetEffector(this IEnforcer enforcer, IEffector effector)
        {
            enforcer.Effector = effector;
            return enforcer;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="modelPath"></param>
        public static IEnforcer SetModel(this IEnforcer enforcer, string modelPath)
        {
            IModel model = DefaultModel.CreateFromFile(modelPath);
            enforcer.SetModel(model);
            return enforcer;
        }

        /// <summary>
        /// Sets the current model.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="model"></param>
        public static IEnforcer SetModel(this IEnforcer enforcer, IModel model)
        {
            enforcer.Model = model;
            model?.RegisterDomainMatchingFunc();
            enforcer.EnforceCache?.Clear();
#if !NET452
            enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache");
#endif
            return enforcer;
        }

        /// <summary>
        /// Sets an adapter.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="adapter"></param>
        public static IEnforcer SetAdapter(this IEnforcer enforcer, IReadOnlyAdapter adapter)
        {
            enforcer.Adapter = adapter;
            return enforcer;
        }

        /// <summary>
        /// Sets an watcher.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="watcher"></param>
        public static IEnforcer SetWatcher(this IEnforcer enforcer, IWatcher watcher)
        {
            enforcer.Watcher = watcher;
            return enforcer;
        }

        /// <summary>
        /// Sets an enforce cache.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="enforceCache"></param>
        public static IEnforcer SetEnforceCache(this IEnforcer enforcer, IEnforceCache enforceCache)
        {
            enforcer.EnforceCache = enforceCache;
            return enforcer;
        }

        #endregion

        #region Poilcy management

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public static bool LoadPolicy(this IEnforcer enforcer)
        {
            bool result = enforcer.Model.LoadPolicy();
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            enforcer.Model.SortPolicy();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Reloads the policy from file/database.
        /// </summary>
        public static async Task<bool> LoadPolicyAsync(this IEnforcer enforcer)
        {
            bool result = await enforcer.Model.LoadPolicyAsync();
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            enforcer.Model.SortPolicy();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static bool LoadFilteredPolicy(this IEnforcer enforcer, IPolicyFilter filter)
        {
            bool result = enforcer.Model.LoadFilteredPolicy(filter);
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static async Task<bool> LoadFilteredPolicyAsync(this IEnforcer enforcer, IPolicyFilter filter)
        {
            bool result = await enforcer.Model.LoadFilteredPolicyAsync(filter);
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Reloads a filtered policy from file/database.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        [Obsolete("Please use PolicyFilter instead")]
        public static Task<bool> LoadFilteredPolicyAsync(this IEnforcer enforcer, Filter filter) =>
            LoadFilteredPolicyAsync(enforcer, filter as IPolicyFilter);

        /// <summary>
        /// Appends a filtered policy from file/database without clearing the existing policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static bool LoadIncrementalFilteredPolicy(this IEnforcer enforcer, IPolicyFilter filter)
        {
            bool result = enforcer.Model.LoadIncrementalFilteredPolicy(filter);
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Appends a filtered policy from file/database without clearing the existing policies.
        /// </summary>
        /// <param name="enforcer"></param>
        /// <param name="filter">The filter used to specify which type of policy should be loaded.</param>
        /// <returns></returns>
        public static async Task<bool> LoadIncrementalFilteredPolicyAsync(this IEnforcer enforcer, IPolicyFilter filter)
        {
            bool result = await enforcer.Model.LoadIncrementalFilteredPolicyAsync(filter);
            if (result is false)
            {
                return false;
            }

            enforcer.ClearCache();
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }

            return true;
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public static bool SavePolicy(this IEnforcer enforcer)
        {
            bool result = enforcer.Model.SavePolicy();
            if (result is false)
            {
                return false;
            }

            PolicyChangedMessage message = PolicyChangedMessage.CreateSavePolicy();
            enforcer.TryNotifyPolicyChanged(message);
            return true;
        }

        /// <summary>
        /// Saves the current policy (usually after changed with Casbin API)
        /// back to file/database.
        /// </summary>
        public static async Task<bool> SavePolicyAsync(this IEnforcer enforcer)
        {
            if (enforcer.Adapter is null)
            {
                return false;
            }

            bool result = await enforcer.Model.SavePolicyAsync();
            if (result is false)
            {
                return false;
            }

            PolicyChangedMessage message = PolicyChangedMessage.CreateSavePolicy();
            await enforcer.TryNotifyPolicyChangedAsync(message);
            return true;
        }

        /// <summary>
        /// Clears all policy.
        /// </summary>
        public static void ClearPolicy(this IEnforcer enforcer)
        {
            enforcer.Model.PolicyStoreHolder.PolicyStore.ClearPolicy();
            enforcer.ClearCache();
        }

        public static void ClearCache(this IEnforcer enforcer)
        {
            enforcer.EnforceCache?.Clear();
#if !NET452
            enforcer.Logger?.LogInformation("Enforcer Cache, Cleared all enforce cache");
#endif
        }

        #endregion

        #region Role management

        /// <summary>
        /// Manually rebuilds the role inheritance relations.
        /// </summary>
        public static void BuildRoleLinks(this IEnforcer enforcer)
        {
            enforcer.Model.BuildRoleLinks();
        }

        public static IRoleManager GetRoleManager(this IEnforcer enforcer, string roleType) =>
            enforcer.Model.GetRoleManager(roleType);

        public static void SetRoleManager(this IEnforcer enforcer, IRoleManager roleManager) =>
            enforcer.SetRoleManager(PermConstants.DefaultRoleType, roleManager);

        public static void SetRoleManager(this IEnforcer enforcer, string roleType, IRoleManager roleManager)
        {
            enforcer.Model.SetRoleManager(roleType, roleManager);
            if (enforcer.AutoBuildRoleLinks)
            {
                enforcer.BuildRoleLinks();
            }
        }

        public static void AddMatchingFunc(this IEnforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedMatchingFunc(PermConstants.DefaultRoleType, func);
        }

        public static void AddDomainMatchingFunc(this IEnforcer enforcer, Func<string, string, bool> func)
        {
            enforcer.AddNamedDomainMatchingFunc(PermConstants.DefaultRoleType, func);
        }

        public static void AddNamedMatchingFunc(this IEnforcer enforcer, string roleType,
            Func<string, string, bool> func)
        {
            enforcer.Model.GetRoleManager(roleType).AddMatchingFunc(func);
        }

        public static void AddNamedDomainMatchingFunc(this IEnforcer enforcer, string roleType,
            Func<string, string, bool> func)
        {
            enforcer.Model.GetRoleManager(roleType).AddDomainMatchingFunc(func);
        }

        #endregion

        #region Enforce Cotext

        public static EnforceContext CreateContext(this IEnforcer enforcer, bool explain)
        {
            return EnforceContext.Create(enforcer, explain);
        }

        public static EnforceContext CreateContext(this IEnforcer enforcer,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            string matcherType = PermConstants.DefaultMatcherType,
            bool explain = false)
        {
            return EnforceContext.Create(enforcer, requestType, policyType, effectType, matcherType, explain);
        }

        public static EnforceContext CreateContextWithMatcher(this IEnforcer enforcer, string matcher, bool explain)
        {
            return EnforceContext.CreateWithMatcher(enforcer, matcher, explain);
        }

        public static EnforceContext CreateContextWithMatcher(this IEnforcer enforcer,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            bool explain = false)
        {
            return EnforceContext.CreateWithMatcher(enforcer, matcher, requestType, policyType, effectType, explain);
        }

        #endregion

#if !NET452
        internal static void LogEnforceCachedResult<TRequest>(this IEnforcer enforcer, TRequest requestValues,
            bool finalResult)
            where TRequest : IRequestValues
        {
            enforcer.Logger?.LogEnforceCachedResult(requestValues, finalResult);
        }

        internal static void LogEnforceResult<TRequest>(this IEnforcer enforcer, in EnforceContext context,
            TRequest requestValues, bool finalResult)
            where TRequest : IRequestValues
        {
            if (context.Explain)
            {
                enforcer.Logger?.LogEnforceResult(requestValues, finalResult, context.Explanations);
                return;
            }
            enforcer.Logger?.LogEnforceResult(requestValues, finalResult);
        }
#endif
    }
}
