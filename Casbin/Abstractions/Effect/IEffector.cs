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

namespace Casbin.Effect
{
    /// <summary>
    /// Effector is the interface for Casbin effectors.
    /// </summary>
    public interface IEffector
    {
        /// <summary>
        /// Merges all matching results collected by the enforcer into a single decision.
        /// </summary>
        /// <param name="effectExpression">The expression of [policy_effect].</param>
        /// <param name="effects">The effects of all matched rules.</param>
        /// <param name="results">The matcher results of all matched rules.</param>
        /// <param name="policyCount"></param>
        /// <param name="hitPolicyIndex"></param>
        /// <param name="policyIndex"></param>
        /// <returns>The final effect.</returns>
        PolicyEffect MergeEffects(string effectExpression, IReadOnlyList<PolicyEffect>  effects, IReadOnlyList<float> results, int policyIndex, int policyCount, out int hitPolicyIndex);
    }
}
