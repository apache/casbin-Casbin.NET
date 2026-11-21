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
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin;

public static partial class EnforcerExtension
{
    #region Generic List Enforce

    public static bool Enforce<T>(this IEnforcer enforcer, params T[] value)
    {
        var request = Request.CreateValues(value);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T>(this IEnforcer enforcer, params T[] value)
    {
        var request = Request.CreateValues(value);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T>(this IEnforcer enforcer, EnforceContext context, params T[] value)
    {
        var request = Request.CreateValues(value);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T>(this IEnforcer enforcer, EnforceContext context, params T[] value)
    {
        var request = Request.CreateValues(value);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T>(this IEnforcer enforcer, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T>(this IEnforcer enforcer, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T>(this IEnforcer enforcer, string matcher, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T>(this IEnforcer enforcer, string matcher, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T>(this IEnforcer enforcer, string matcher, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T>(this IEnforcer enforcer, string matcher, params T[] value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 1

    public static bool Enforce<T>(this IEnforcer enforcer, T value)
    {
        var request = Request.CreateValues(value);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T>(this IEnforcer enforcer, T value)
    {
        var request = Request.CreateValues(value);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T>(this IEnforcer enforcer, EnforceContext context, T value)
    {
        var request = Request.CreateValues(value);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T>(this IEnforcer enforcer, EnforceContext context, T value)
    {
        var request = Request.CreateValues(value);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T>(this IEnforcer enforcer, T value1)
    {
        var request = Request.CreateValues(value1);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T>(this IEnforcer enforcer, T value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T>(this IEnforcer enforcer, string matcher, T value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T>(this IEnforcer enforcer, string matcher, T value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T>(this IEnforcer enforcer, string matcher, T value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T>(this IEnforcer enforcer, string matcher, T value)
    {
        var request = Request.CreateValues(value);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 2

    public static bool Enforce<T1, T2>(this IEnforcer enforcer, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2>(this IEnforcer enforcer, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2>(this IEnforcer enforcer, EnforceContext context, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2>(this IEnforcer enforcer, EnforceContext context, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2>(this IEnforcer enforcer, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2>(this IEnforcer enforcer, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2>(this IEnforcer enforcer, string matcher, T1 value1,
        T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2)
    {
        var request = Request.CreateValues(value1, value2);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 3

    public static bool Enforce<T1, T2, T3>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3>(this IEnforcer enforcer, EnforceContext context, T1 value1, T2 value2,
        T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3>(this IEnforcer enforcer, EnforceContext context, T1 value1,
        T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
        T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3>(this IEnforcer enforcer, string matcher, T1 value1,
        T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3)
    {
        var request = Request.CreateValues(value1, value2, value3);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 4

    public static bool Enforce<T1, T2, T3, T4>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
        T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4>(this IEnforcer enforcer, EnforceContext context, T1 value1, T2 value2,
        T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4>(this IEnforcer enforcer, EnforceContext context, T1 value1,
        T2 value2, T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
        T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4>(this IEnforcer enforcer, string matcher, T1 value1,
        T2 value2, T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3,
            T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4)
    {
        var request = Request.CreateValues(value1, value2, value3, value4);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 5

    public static bool Enforce<T1, T2, T3, T4, T5>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
        T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5>(this IEnforcer enforcer, EnforceContext context, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5>(this IEnforcer enforcer, string matcher, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, string matcher,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 6

    public static bool Enforce<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, EnforceContext context, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, string matcher, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, string matcher,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 7

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, EnforceContext context, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, string matcher,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, string matcher, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7>(this IEnforcer enforcer, string matcher, T1 value1,
            T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 8

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
        T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3, T4 value4,
            T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, string matcher,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, string matcher, T1 value1,
            T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnforcer enforcer, string matcher, T1 value1,
            T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 9

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, string matcher,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, string matcher, T1 value1,
            T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 10

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, T1 value1, T2 value2,
        T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, EnforceContext context,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, T1 value1, T2 value2, T3 value3,
            T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 11

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6,
        T7 value7, T8 value8, T9 value9, T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10, T11 value11)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 12

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6,
        T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEnforcer enforcer,
            string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
            T9 value9, T10 value10, T11 value11, T12 value12)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 13

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6,
        T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEnforcer enforcer,
            string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
            T9 value9, T10 value10, T11 value11, T12 value12, T13 value13)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion

    #region Generic Enforce Count 14

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer, T1 value1,
        T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        return enforcer.Enforce(enforcer.CreateContext(), request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer,
        T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10,
        T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        return enforcer.EnforceAsync(enforcer.CreateContext(), request);
    }

    public static bool Enforce<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer,
        EnforceContext context, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceEx<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer, T1 value1, T2 value2,
            T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9, T10 value10, T11 value11,
            T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContext(true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    public static bool EnforceWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer,
        string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
        T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.Enforce(context, request);
    }

    public static Task<bool> EnforceWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        this IEnforcer enforcer, string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6,
        T7 value7, T8 value8, T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.EnforceAsync(context, request);
    }

    public static (bool Result, IEnumerable<IEnumerable<string>> Explains)
        EnforceExWithMatcher<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer, string matcher,
            T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8, T9 value9,
            T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = enforcer.Enforce(context, request);
        return (result, context.Explanations);
    }

    public static async Task<(bool Result, IEnumerable<IEnumerable<string>> Explains)>
        EnforceExWithMatcherAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEnforcer enforcer,
            string matcher, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8,
            T9 value9, T10 value10, T11 value11, T12 value12, T13 value13, T14 value14)
    {
        var request = Request.CreateValues(value1, value2, value3, value4, value5, value6, value7, value8, value9,
            value10,
            value11, value12, value13, value14);
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher, true);
        bool result = await enforcer.EnforceAsync(context, request);
        return (result, context.Explanations);
    }

    #endregion
}
