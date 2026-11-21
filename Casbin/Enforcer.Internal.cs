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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Casbin.Effect;
using Casbin.Evaluation;
using Casbin.Model;
using Casbin.Util;

namespace Casbin;

public partial class Enforcer
{
    private Task<bool> InternalEnforceAsync<TRequest>(EnforceContext context, TRequest requestValues)
        where TRequest : IRequestValues
    {
        bool CallInFunc()
        {
            return InternalEnforce(in context, in requestValues);
        }

        return Task.Run(CallInFunc);
    }

    private bool InternalEnforce<TRequest>(in EnforceContext context, in TRequest requestValues)
        where TRequest : IRequestValues
    {
        if (context.View.SupportGeneric is false)
        {
            return InternalEnforce<IRequestValues, IPolicyValues>(in context, requestValues);
        }

        return context.View.PolicyTokens.Count switch
        {
            1 => InternalEnforce<TRequest, PolicyValues<string>>(in context, requestValues),
            2 => InternalEnforce<TRequest, PolicyValues<string, string>>(in context, requestValues),
            3 => InternalEnforce<TRequest, PolicyValues<string, string, string>>(in context, requestValues),
            4 => InternalEnforce<TRequest, PolicyValues<string, string, string, string>>(in context,
                requestValues),
            5 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string>>(in context,
                requestValues),
            6 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string>>(
                in context,
                requestValues),
            7 => InternalEnforce<TRequest,
                PolicyValues<string, string, string, string, string, string, string>>(
                in context, requestValues),
            8 => InternalEnforce<TRequest,
                PolicyValues<string, string, string, string, string, string, string, string>>(in context,
                requestValues),
            9 => InternalEnforce<TRequest,
                PolicyValues<string, string, string, string, string, string, string, string, string>>(
                in context,
                requestValues),
            10 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                string, string, string>>(in context, requestValues),
            11 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                string, string, string, string>>(in context, requestValues),
            12 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                string, string, string, string, string>>(in context, requestValues),
            13 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                string, string, string, string, string, string>>(in context, requestValues),
            14 => InternalEnforce<TRequest, PolicyValues<string, string, string, string, string, string, string,
                string, string, string, string, string, string, string>>(in context, requestValues),
            _ => InternalEnforce<IRequestValues, IPolicyValues>(in context, requestValues)
        };
    }

    private bool InternalEnforce<TRequest, TPolicy>(in EnforceContext context, in TRequest requestValues)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        EnforceSession session = new EnforceSession();
        IExpressionHandler expressionHandler = Model.ExpressionHandler;
        PolicyScanner<TRequest> scanner = context.View.PolicyAssertion.Scan(in requestValues);

        EffectChain effectChain = new();
        if (Effector is IChainEffector effector)
        {
            session.IsChainEffector = true;
            effectChain = effector.CreateChain(context.View.Effect, context.View.EffectExpressionType);
        }

        session.HasNextPolicy = scanner.HasNext();
        HandleInitialRequest(in context, ref session, in requestValues);

        if (session.HasNextPolicy && context.View.HasPolicyParameter)
        {
            try
            {
                int policyIndex = 0;
                while (scanner.GetNext(out IPolicyValues outValues))
                {
                    TPolicy policyValues = (TPolicy)outValues;
                    session.PolicyIndex = policyIndex;

                    HandleBeforeExpression(in context, ref session, in effectChain, in requestValues, policyValues);
                    session.ExpressionResult = expressionHandler.Invoke(in context, session.ExpressionString,
                        in requestValues, in policyValues);

                    if (session.IsChainEffector)
                    {
                        HandleExpressionResult(in context, ref session, ref effectChain, in requestValues, policyValues);
                    }
                    else
                    {
                        HandleExpressionResult(in context, ref session, Effector, in requestValues, policyValues);
                    }

                    if (session.Determined)
                    {
                        scanner.Interrupt();
                        break;
                    }

                    policyIndex++;
                }
            }
            finally
            {
                scanner.Interrupt();
            }
        }
        else
        {
            DummyPolicyValues policyValues = DummyPolicyValues.Empty;
            HandleBeforeExpression(in context, ref session, in effectChain, in requestValues, policyValues);
            session.ExpressionResult = expressionHandler.Invoke(in context, session.ExpressionString,
                in requestValues, in policyValues);

            if (session.IsChainEffector)
            {
                HandleExpressionResult(in context, ref session, ref effectChain, in requestValues, policyValues);
            }
            else
            {
                HandleExpressionResult(in context, ref session, Effector, in requestValues, policyValues);
            }
        }

        return session.EnforceResult;
    }

    private static void HandleInitialRequest<TRequest>(
        in EnforceContext context, ref EnforceSession session, in TRequest request)
        where TRequest : IRequestValues
    {
        session.ExpressionString = context.View.HasEval ? context.View.Matcher : context.View.TransformedMatcher;

        int requestTokenCount = context.View.RequestAssertion.Tokens.Count;
        if (requestTokenCount > request.Count)
        {
            throw new ArgumentException(
                $"Invalid request size: expected {requestTokenCount} at least, got {request.Count}.");
        }

        if (session.IsChainEffector is false)
        {
            session.PolicyEffects = new List<PolicyEffect>();
        }
    }

    private static void HandleBeforeExpression<TRequest, TPolicy>(
        in EnforceContext context, ref EnforceSession session, in EffectChain effectChain,
        in TRequest request, scoped in TPolicy policy)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        int policyTokenCount = context.View.PolicyAssertion.Tokens.Count;

        if (session.HasNextPolicy is false)
        {
            if (context.View.HasEval)
            {
                throw new ArgumentException("Please make sure rule exists in policy when using eval() in matcher");
            }

            return;
        }

        if (policyTokenCount > policy.Count)
        {
            throw new ArgumentException(
                $"Invalid policy size: expected {policyTokenCount} at least, got {policy.Count}.");
        }

        if (session.IsChainEffector is false &&
            context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
        {
            ThrowHelper.ThrowNotSupportException(
                $"Only IChainEffector<T> support {nameof(EffectExpressionType.PriorityDenyOverride)} policy effect expression.");
        }

        if (context.View.HasPriority &&
            context.View.EffectExpressionType is EffectExpressionType.PriorityDenyOverride)
        {
            if (int.TryParse(policy[context.View.PriorityIndex], out int nowPriority))
            {
                if (session.Priority.HasValue && nowPriority != session.Priority.Value
                                              && effectChain.HitPolicyCount > 0)
                {
                    session.DetermineResult(effectChain.Result);
                }

                session.Priority = nowPriority;
            }
        }

        if (context.View.HasEval is false)
        {
            return;
        }

        session.ExpressionString = context.View.Matcher;
        session.ExpressionString = RewriteEval(in context, session.ExpressionString, policy);
        session.ExpressionString = RewriteInOperator(in context, session.ExpressionString);
        session.ExpressionString = EnforceView.TransformMatcher(context.View, session.ExpressionString);
    }

    private static void HandleExpressionResult<TRequest, TPolicy>(
        in EnforceContext context, ref EnforceSession session, IEffector effector,
        in TRequest request, scoped in TPolicy policy)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        PolicyEffect nowEffect;
        if (session.HasNextPolicy is false)
        {
            nowEffect = GetEffect(session.ExpressionResult);
            nowEffect = effector.MergeEffects(context.View.Effect, new[] { nowEffect }, null, 0,
                0, out _);
            bool finalResult = nowEffect.ToNullableBool() ?? false;
            session.DetermineResult(finalResult);
        }

        nowEffect = GetEffect(session.ExpressionResult);

        if (nowEffect is not PolicyEffect.Indeterminate && context.View.HasEffect)
        {
            string policyEffect = policy[context.View.EffectIndex];
            nowEffect = policyEffect switch
            {
                "allow" => PolicyEffect.Allow,
                "deny" => PolicyEffect.Deny,
                _ => PolicyEffect.Indeterminate
            };
        }

        session.PolicyEffects[session.PolicyIndex] = nowEffect;

        int policyCount = session.HasNextPolicy ? session.PolicyIndex + 1 : session.PolicyIndex;
        nowEffect = effector.MergeEffects(context.View.Effect, session.PolicyEffects, null, session.PolicyIndex,
            policyCount, out int hitPolicyIndex);

        if (context.Explain && hitPolicyIndex is not -1)
        {
            context.Explanations.Add(policy);
        }

        if (nowEffect is not PolicyEffect.Indeterminate)
        {
            session.DetermineResult(nowEffect.ToNullableBool() ?? false);
        }

        session.EnforceResult = false;
    }

    private static void HandleExpressionResult<TRequest, TPolicy>(
        in EnforceContext context, ref EnforceSession session, ref EffectChain effectChain,
        in TRequest request, scoped in TPolicy policy)
        where TRequest : IRequestValues
        where TPolicy : IPolicyValues
    {
        PolicyEffect nowEffect;
        if (session.HasNextPolicy is false)
        {
            nowEffect = GetEffect(session.ExpressionResult);
            if (effectChain.TryChain(nowEffect))
            {
                session.DetermineResult(effectChain.Result);
            }

            session.DetermineResult(false);
        }

        nowEffect = GetEffect(session.ExpressionResult);

        if (nowEffect is not PolicyEffect.Indeterminate
            && context.View.HasEffect
            && context.View.HasPolicyParameter)
        {
            string policyEffect = policy[context.View.EffectIndex];
            nowEffect = policyEffect switch
            {
                "allow" => PolicyEffect.Allow,
                "deny" => PolicyEffect.Deny,
                _ => PolicyEffect.Indeterminate
            };
        }

        bool chainResult = effectChain.TryChain(nowEffect);

        if (context.Explain && effectChain.HitPolicy)
        {
            if (effectChain.CanChain is false && effectChain.Result is false)
            {
                context.Explanations.Clear();
            }

            context.Explanations.Add(policy);
        }

        if (chainResult is false || effectChain.CanChain is false)
        {
            session.DetermineResult(effectChain.Result);
        }

        session.EnforceResult = effectChain.Result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PolicyEffect GetEffect(bool expressionResult) =>
        expressionResult ? PolicyEffect.Allow : PolicyEffect.Indeterminate;

    private static string RewriteEval(in EnforceContext context, string expressionString,
        IPolicyValues policyValues)
    {
        if (context.View.EvalRules is null)
        {
            return expressionString;
        }

        Dictionary<string, string> rules = new();
        foreach (KeyValuePair<string, int> rule in context.View.EvalRules)
        {
            rules[rule.Key] = policyValues[rule.Value].Replace('\'', '"');
        }

        expressionString = StringUtil.ReplaceEval(expressionString, rules);
        return expressionString;
    }

    private static string RewriteInOperator(in EnforceContext context, string expressionString)
    {
        if (context.View.Matcher is null)
        {
            return expressionString;
        }

        expressionString = StringUtil.ReplaceInListOperator(expressionString);
        expressionString = StringUtil.ReplaceInOperator(expressionString);
        return expressionString;
    }
}
