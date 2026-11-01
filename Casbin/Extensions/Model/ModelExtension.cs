using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Functions;
using Casbin.Persist;
using Casbin.Rbac;

namespace Casbin.Model
{
    public static class ModelExtension
    {
        public static bool SortPolicy(this IModel model)
        {
            if (model.Sections.TryGetAssertion(PermConstants.Section.PolicyEffectSection,
                    PermConstants.DefaultPolicyEffectType, out PolicyEffectAssertion assertion) is false)
            {
                return false;
            }

            switch (assertion.Value)
            {
                case PermConstants.PolicyEffect.Priority:
                case PermConstants.PolicyEffect.PriorityDenyOverride:
                    return model.PolicyStoreHolder.PolicyStore.SortPolicyByPriority(PermConstants.Section.PolicySection,
                        PermConstants.DefaultPolicyType);
                case PermConstants.PolicyEffect.SubjectPriority:
                    Dictionary<string, int> map = model.Sections.GetRoleAssertion(PermConstants.DefaultRoleType)
                        .GetSubjectHierarchyMap();
                    return model.PolicyStoreHolder.PolicyStore.SortPolicyBySubjectHierarchy(
                        PermConstants.Section.PolicySection,
                        PermConstants.DefaultPolicyType, map);
                default:
                    return false;
            }
        }

        public static bool LoadPolicy(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                return false;
            }

            DefaultPolicyStore policyStore = new();
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
            }

            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                }
            }

            model.AdapterHolder.EpochAdapter.LoadPolicy(policyStore);
            model.PolicyStoreHolder.PolicyStore = policyStore;
            return true;
        }

        public static async Task<bool> LoadPolicyAsync(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                return false;
            }

            DefaultPolicyStore policyStore = new();
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
            }

            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                }
            }

            await model.AdapterHolder.EpochAdapter.LoadPolicyAsync(policyStore);
            model.PolicyStoreHolder.PolicyStore = policyStore;
            return true;
        }

        public static bool LoadFilteredPolicy(this IModel model, IPolicyFilter filter)
        {
            model.PolicyStoreHolder.PolicyStore?.ClearPolicy();
            return LoadFilteredPolicyInternal(model, filter);
        }

        public static async Task<bool> LoadFilteredPolicyAsync(this IModel model, IPolicyFilter filter)
        {
            model.PolicyStoreHolder.PolicyStore?.ClearPolicy();
            return await LoadFilteredPolicyInternalAsync(model, filter);
        }

        public static bool LoadIncrementalFilteredPolicy(this IModel model, IPolicyFilter filter)
        {
            return LoadFilteredPolicyInternal(model, filter);
        }

        public static async Task<bool> LoadIncrementalFilteredPolicyAsync(this IModel model, IPolicyFilter filter)
        {
            return await LoadFilteredPolicyInternalAsync(model, filter);
        }

        private static bool LoadFilteredPolicyInternal(IModel model, IPolicyFilter filter)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.FilteredAdapter is null)
            {
                return false;
            }

            // Initialize policy store if it doesn't exist
            if (model.PolicyStoreHolder.PolicyStore is null)
            {
                DefaultPolicyStore policyStore = new();
                foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
                }

                if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
                {
                    foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                    {
                        policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                    }
                }
                model.PolicyStoreHolder.PolicyStore = policyStore;
            }

            model.AdapterHolder.FilteredAdapter.LoadFilteredPolicy(model.PolicyStoreHolder.PolicyStore, filter);
            return true;
        }

        private static async Task<bool> LoadFilteredPolicyInternalAsync(IModel model, IPolicyFilter filter)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.FilteredAdapter is null)
            {
                return false;
            }

            // Initialize policy store if it doesn't exist
            if (model.PolicyStoreHolder.PolicyStore is null)
            {
                DefaultPolicyStore policyStore = new();
                foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
                {
                    policyStore.AddNode(PermConstants.Section.PolicySection, pair.Key, pair.Value);
                }

                if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
                {
                    foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                    {
                        policyStore.AddNode(PermConstants.Section.RoleSection, pair.Key, pair.Value);
                    }
                }
                model.PolicyStoreHolder.PolicyStore = policyStore;
            }

            await model.AdapterHolder.FilteredAdapter.LoadFilteredPolicyAsync(model.PolicyStoreHolder.PolicyStore, filter);
            return true;
        }

        public static bool SavePolicy(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                throw new InvalidOperationException("Cannot save policy when use a readonly adapter");
            }

            if (model.AdapterHolder.FilteredAdapter is not null && model.AdapterHolder.FilteredAdapter.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save filtered policies");
            }

            model.AdapterHolder.EpochAdapter.SavePolicy(model.PolicyStoreHolder.PolicyStore);
            return true;
        }

        public static async Task<bool> SavePolicyAsync(this IModel model)
        {
            if (model.AdapterHolder.Adapter is null)
            {
                return false;
            }

            if (model.AdapterHolder.EpochAdapter is null)
            {
                throw new InvalidOperationException("Cannot save policy when use a readonly adapter");
            }

            if (model.AdapterHolder.FilteredAdapter is not null && model.AdapterHolder.FilteredAdapter.IsFiltered)
            {
                throw new InvalidOperationException("Cannot save filtered policies");
            }

            await model.AdapterHolder.EpochAdapter.SavePolicyAsync(model.PolicyStoreHolder.PolicyStore);
            return true;
        }

        public static void SetAutoSave(this IModel model, bool autoSave)
        {
            // Set AutoSave for policy assertions (section "p")
            foreach (KeyValuePair<string, PolicyAssertion> pair in model.Sections.GetPolicyAssertions())
            {
                pair.Value.PolicyManager.AutoSave = autoSave;
            }

            // Set AutoSave for role/grouping assertions (section "g") if they exist
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection))
            {
                foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
                {
                    pair.Value.PolicyManager.AutoSave = autoSave;
                }
            }
        }

        public static IPolicyManager GetPolicyManager(this IModel model, string section,
            string policyType = PermConstants.DefaultPolicyType) =>
            model.Sections.GetPolicyAssertion(section, policyType).PolicyManager;

        public static IRoleManager
            GetRoleManager(this IModel model, string roleType = PermConstants.DefaultRoleType) =>
            model.Sections.GetRoleAssertion(roleType).RoleManager;

        public static void SetRoleManager(this IModel model, string roleType, IRoleManager roleManager)
        {
            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.RoleManager = roleManager;
            model.EnforceCache.Clear();
            model.GFunctionCachePool.Clear(roleType);
            model.ExpressionHandler.SetFunction(roleType, BuiltInFunctions.GenerateGFunction(
                assertion.RoleManager, model.GFunctionCachePool.GetCache(roleType)));
        }

        internal static void RegisterDomainMatchingFunc(this IModel model)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false
                || model.Sections.TryGetAssertion(PermConstants.Section.RequestSection,
                    PermConstants.DefaultRequestType, out RequestAssertion requestAssertion) is false
                || model.Sections.TryGetAssertion(PermConstants.Section.PolicySection,
                    PermConstants.DefaultPolicyType, out PolicyAssertion policyAssertion) is false
                || model.Sections.TryGetAssertion(PermConstants.Section.MatcherSection,
                    PermConstants.DefaultMatcherType, out MatcherAssertion matcherAssertion) is false
                || requestAssertion.Tokens.Count < 2 || policyAssertion.Tokens.Count < 2)
            {
                return;
            }

            string requestDomainToken = requestAssertion.Tokens.First(token => token.Value is 1).Key;
            string policyDomainToken = policyAssertion.Tokens.First(token => token.Value is 1).Key;
            string matchFunc =
                $"keyMatch({PermConstants.DefaultRequestType}.{requestDomainToken}, {PermConstants.DefaultPolicyType}.{policyDomainToken})";
            if (matcherAssertion.Value.Contains(matchFunc) is false)
            {
                return;
            }

            foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
            {
                if (pair.Value.GroupPolicyCount > 2)
                {
                    pair.Value.RoleManager.AddDomainMatchingFunc(BuiltInFunctions.KeyMatch);
                }
            }
        }

        /// <summary>
        ///     Initializes the roles in RBAC.
        /// </summary>
        public static void BuildRoleLinks(this IModel model, string roleType = null)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            if (roleType is not null)
            {
                RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
                assertion.RoleManager.Clear();
                assertion.BuildRoleLinks();
                model.GFunctionCachePool.Clear(roleType);
                return;
            }

            foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
            {
                string name = pair.Key;
                RoleAssertion assertion = pair.Value;
                assertion.RoleManager.Clear();
                model.GFunctionCachePool.Clear(name);
            }

            foreach (KeyValuePair<string, RoleAssertion> pair in model.Sections.GetRoleAssertions())
            {
                RoleAssertion assertion = pair.Value;
                assertion.BuildRoleLinks();
            }
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="roleType"></param>
        /// <param name="rule"></param>
        public static void BuildIncrementalRoleLink(this IModel model, PolicyOperation policyOperation,
            string roleType, IPolicyValues rule)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, rule);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relation.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRule"></param>
        /// <param name="newRule"></param>
        public static void BuildIncrementalRoleLink(this IModel model, PolicyOperation policyOperation,
            string roleType, IPolicyValues oldRule, IPolicyValues newRule)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLink(policyOperation, oldRule, newRule);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="roleType"></param>
        /// <param name="rules"></param>
        public static void BuildIncrementalRoleLinks(this IModel model, PolicyOperation policyOperation,
            string roleType, IEnumerable<IPolicyValues> rules)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, rules);
            model.GFunctionCachePool.Clear(roleType);
        }

        /// <summary>
        ///     Provides incremental build the role inheritance relations.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="policyOperation"></param>
        /// <param name="roleType"></param>
        /// <param name="oldRules"></param>
        /// <param name="newRules"></param>
        public static void BuildIncrementalRoleLinks(this IModel model, PolicyOperation policyOperation,
            string roleType, IEnumerable<IPolicyValues> oldRules, IEnumerable<IPolicyValues> newRules)
        {
            if (model.Sections.ContainsSection(PermConstants.Section.RoleSection) is false)
            {
                return;
            }

            RoleAssertion assertion = model.Sections.GetRoleAssertion(roleType);
            assertion.BuildIncrementalRoleLinks(policyOperation, oldRules, newRules);
            model.GFunctionCachePool.Clear(roleType);
        }
    }
}
