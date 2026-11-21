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
using Casbin.Effect;

namespace Casbin.Evaluation
{
    internal static class EffectEvaluator
    {
        internal static bool TryEvaluate(PolicyEffect effect, EffectExpressionType effectExpressionType,
            ref bool result, out bool hitPolicy)
        {
            hitPolicy = false;

            switch (effectExpressionType)
            {
                case EffectExpressionType.AllowOverride:
                {
                    result = false;
                    if (effect is PolicyEffect.Allow)
                    {
                        result = true;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case EffectExpressionType.DenyOverride:
                {
                    result = true;
                    if (effect is PolicyEffect.Deny)
                    {
                        result = false;
                        hitPolicy = true;
                        return true;
                    }
                }
                break;

                case EffectExpressionType.AllowAndDeny:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            hitPolicy = true;
                            return false;
                        case PolicyEffect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case EffectExpressionType.Priority:
                case EffectExpressionType.PriorityAllOverride:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            hitPolicy = true;
                            return true;
                        case PolicyEffect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case EffectExpressionType.PriorityDenyOverride:
                    switch (effect)
                    {
                        case PolicyEffect.Allow:
                            result = true;
                            hitPolicy = true;
                            return false;
                        case PolicyEffect.Deny:
                            result = false;
                            hitPolicy = true;
                            return true;
                    }
                    break;

                case EffectExpressionType.Custom:
                    // TODO: Support custom policy effect.
                    break;

                default:
                    throw new NotSupportedException("Not supported policy effect type.");
            }

            return false;
        }

    }
}
