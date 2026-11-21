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
using Casbin.Evaluation;

namespace Casbin.Effect
{
    /// <summary>
    /// DefaultEffector is default effector for Casbin.
    /// </summary>
    public class DefaultEffector : IEffector, IChainEffector
    {
        public EffectChain CreateChain(string effectExpression) => new(effectExpression);

        public EffectChain CreateChain(string effectExpression, EffectExpressionType effectExpressionType) =>
            new(effectExpression, effectExpressionType);

        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression"></param>
        /// <param name="effects"></param>
        /// <param name="matches"></param>
        /// <param name="policyIndex"></param>
        /// <param name="policyCount"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <returns></returns>
        public PolicyEffect MergeEffects(string effectExpression, IReadOnlyList<PolicyEffect> effects,
            IReadOnlyList<float> matches, int policyIndex, int policyCount, out int hitPolicyIndex)
        {
            EffectExpressionType effectExpressionType = ParseEffectExpressionType(effectExpression);
            bool? result = MergeEffects(effectExpressionType, effects, matches, policyIndex, policyCount,
                out hitPolicyIndex);
            return result.ToPolicyEffect();
        }

        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpressionType"></param>
        /// <param name="effects"></param>
        /// <param name="matches"></param>
        /// <param name="policyCount"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <param name="policyIndex"></param>
        /// <returns></returns>
        private static bool? MergeEffects(EffectExpressionType effectExpressionType,
            IReadOnlyList<PolicyEffect> effects,
            IReadOnlyList<float> matches, int policyIndex, int policyCount, out int hitPolicyIndex)
        {
            hitPolicyIndex = -1;
            int effectCount = policyIndex + 1;
            bool finalResult = false;

            for (int index = 0; index < effectCount; index++)
            {
                if (EffectEvaluator.TryEvaluate(effects[index], effectExpressionType,
                        ref finalResult, out bool hitPolicy))
                {
                    if (hitPolicy)
                    {
                        hitPolicyIndex = index;
                    }

                    return finalResult;
                }
            }

            if (effectCount == policyCount)
            {
                return finalResult;
            }

            return null;
        }

        public static EffectExpressionType ParseEffectExpressionType(string effectExpression) => effectExpression switch
        {
            PermConstants.PolicyEffect.AllowOverride => EffectExpressionType.AllowOverride,
            PermConstants.PolicyEffect.DenyOverride => EffectExpressionType.DenyOverride,
            PermConstants.PolicyEffect.AllowAndDeny => EffectExpressionType.AllowAndDeny,
            PermConstants.PolicyEffect.Priority => EffectExpressionType.Priority,
            PermConstants.PolicyEffect.SubjectPriority => EffectExpressionType.PriorityAllOverride,
            PermConstants.PolicyEffect.PriorityDenyOverride => EffectExpressionType.PriorityDenyOverride,
            _ => throw new NotSupportedException("Not supported policy effect.")
        };
    }
}
