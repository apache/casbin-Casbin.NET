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
