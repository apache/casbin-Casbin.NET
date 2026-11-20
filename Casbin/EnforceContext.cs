using System.Collections.Generic;
using Casbin.Model;

namespace Casbin
{
    public struct EnforceContext
    {
        private EnforceContext(EnforceView view, bool cacheable, bool explain = false)
        {
            View = view;
            Cacheable = cacheable;
            HandleOptionAndCached = false;

            Explain = explain;
            Explanations = explain ? new List<IEnumerable<string>>() : null;
        }

        public EnforceView View { get; }
        internal bool Cacheable { get; }
        public bool HandleOptionAndCached { get; internal set; }

        public bool Explain { get; }
        public List<IEnumerable<string>> Explanations { get; }

        public static EnforceContext Create(IEnforcer enforcer, bool explain)
        {
            return Create(
                enforcer,
                PermConstants.DefaultRequestType,
                PermConstants.DefaultPolicyType,
                PermConstants.DefaultPolicyEffectType,
                PermConstants.DefaultMatcherType,
                explain
            );
        }

        public static EnforceContext Create(
            IEnforcer enforcer,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            string matcherType = PermConstants.DefaultMatcherType,
            bool explain = false)
        {
            IModel model = enforcer.Model;
            string name = string.Concat(requestType, policyType, effectType, matcherType);
            bool cacheable = requestType == PermConstants.DefaultRequestType
                && policyType == PermConstants.DefaultPolicyType
                && effectType == PermConstants.DefaultPolicyEffectType
                && matcherType == PermConstants.DefaultMatcherType;
            if (model.EnforceViewCache.TryGet(name, out EnforceView view))
            {
                return new EnforceContext(view, cacheable, explain);
            }
            view = EnforceView.Create(model, requestType, policyType, effectType, matcherType);
            _ = model.EnforceViewCache.TryAdd(name, view);
            return new EnforceContext(view, cacheable, explain);
        }

        public static EnforceContext CreateWithMatcher(IEnforcer enforcer, string matcher, bool explain)
        {
            return CreateWithMatcher(
                enforcer,
                matcher,
                PermConstants.DefaultRequestType,
                PermConstants.DefaultPolicyType,
                PermConstants.DefaultPolicyEffectType,
                explain);
        }

        public static EnforceContext CreateWithMatcher(
            IEnforcer enforcer,
            string matcher,
            string requestType = PermConstants.DefaultRequestType,
            string policyType = PermConstants.DefaultPolicyType,
            string effectType = PermConstants.DefaultPolicyEffectType,
            bool explain = false)
        {
            return new EnforceContext(EnforceView.CreateWithMatcher(enforcer.Model,
                matcher, requestType, policyType, effectType), false, explain);
        }
    }
}
