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

namespace Casbin;

#if !NET452
using BatchEnforceAsyncResults = IAsyncEnumerable<bool>;
#else
using BatchEnforceAsyncResults = System.Threading.Tasks.Task<IEnumerable<bool>>;
#endif

public static partial class EnforcerExtension
{
    #region Batch Enforce Count 1

    public static IEnumerable<bool> BatchEnforce<T>(this IEnforcer enforcer, IEnumerable<T> values) where T : IRequestValues
    {
        return enforcer.BatchEnforce(enforcer.CreateContext(), values);
    }

    public static IEnumerable<bool> ParallelBatchEnforce<T>(this Enforcer enforcer, IReadOnlyList<T> values, int maxDegreeOfParallelism = -1) 
        where T : IRequestValues
    {
        return enforcer.ParallelBatchEnforce<T>(enforcer.CreateContext(), values, maxDegreeOfParallelism);
    }

    public static BatchEnforceAsyncResults BatchEnforceAsync<T>(this IEnforcer enforcer, IEnumerable<T> values) where T : IRequestValues
    {
        return enforcer.BatchEnforceAsync(enforcer.CreateContext(), values);
    }

    public static IEnumerable<bool> BatchEnforceWithMatcher<T>(this IEnforcer enforcer, string matcher, 
        IEnumerable<T> values) where T : IRequestValues
    {
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.BatchEnforce(context, values);
    }

    public static IEnumerable<bool> BatchEnforceWithMatcherParallel<T>(this Enforcer enforcer, string matcher, 
        IReadOnlyList<T> values, int maxDegreeOfParallelism = -1) where T : IRequestValues
    {
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.ParallelBatchEnforce(context, values, maxDegreeOfParallelism);
    }

    public static BatchEnforceAsyncResults BatchEnforceWithMatcherAsync<T>(this IEnforcer enforcer, string matcher, 
        IEnumerable<T> values) where T : IRequestValues
    {
        EnforceContext context = enforcer.CreateContextWithMatcher(matcher);
        return enforcer.BatchEnforceAsync(context, values);
    }

    #endregion
}
