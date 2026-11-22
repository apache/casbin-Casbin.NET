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
using Casbin.Evaluation;

namespace Casbin.Effect
{
    public ref struct EffectChain
    {
        public EffectChain(string effectExpression)
        {
            EffectExpression = effectExpression ?? throw new ArgumentNullException(nameof(effectExpression));
            EffectExpressionType = DefaultEffector.ParseEffectExpressionType(EffectExpression);

            CanChain = true;
            Result = false;

            HitPolicy = false;
            HitPolicyCount = 0;
        }

        public EffectChain(string effectExpression, EffectExpressionType effectExpressionType)
        {
            EffectExpression = effectExpression;
            EffectExpressionType = effectExpressionType;

            CanChain = true;
            Result = false;

            HitPolicy = false;
            HitPolicyCount = 0;
        }

        public bool Result { get; private set; }

        public bool HitPolicy { get; private set; }

        public int HitPolicyCount { get; private set; }

        public bool CanChain { get; private set; }

        public string EffectExpression { get; }

        public EffectExpressionType EffectExpressionType { get; }

        public bool Chain(PolicyEffect effect)
        {
            if (CanChain is false)
            {
                throw new InvalidOperationException();
            }

            bool result = Result;

            if (EffectEvaluator.TryEvaluate(effect, EffectExpressionType,
                    ref result, out bool hitPolicy))
            {
                CanChain = false;
                Result = result;
                HitPolicy = hitPolicy;
                return true;
            }

            Result = result;
            HitPolicy = hitPolicy;
            return true;
        }

        public bool TryChain(PolicyEffect effect)
        {
            if (CanChain is false)
            {
                return false;
            }

            bool result = Result;
            if (EffectEvaluator.TryEvaluate(effect, EffectExpressionType,
                    ref result, out bool hitPolicy))
            {
                CanChain = false;
                Result = result;
                HitPolicy = hitPolicy;
                if (hitPolicy)
                {
                    HitPolicyCount++;
                }

                return true;
            }

            Result = result;
            HitPolicy = hitPolicy;
            if (hitPolicy)
            {
                HitPolicyCount++;
            }

            return true;
        }

        public bool TryChain(PolicyEffect effect, out bool? result)
        {
            if (TryChain(effect))
            {
                result = Result;
                return true;
            }

            result = null;
            return false;
        }
    }
}
