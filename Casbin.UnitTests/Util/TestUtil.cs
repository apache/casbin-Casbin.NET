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
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Rbac;
using Casbin.Util;
using Xunit;

// ReSharper disable PossibleMultipleEnumeration

namespace Casbin.UnitTests.Util;

internal static class TestUtil
{
    internal static void TestBatchEnforce<T>(this IEnforcer e, IEnumerable<(T, bool)> values)
        where T : IRequestValues =>
        Assert.True(values.Select(x => x.Item2).SequenceEqual(e.BatchEnforce(values.Select(x => x.Item1))));

    internal static void TestParallelBatchEnforce<T>(Enforcer e, IEnumerable<(T, bool)> values)
        where T : IRequestValues =>
        Assert.True(values.Select(x => x.Item2)
            .SequenceEqual(e.ParallelBatchEnforce(values.Select(x => x.Item1).ToList())));

    internal static async void TestBatchEnforceAsync<T>(IEnforcer e, IEnumerable<(T, bool)> values)
        where T : IRequestValues
    {
#if !NET452
        var res = e.BatchEnforceAsync(values.Select(x => x.Item1));
#else
        var res = await e.BatchEnforceAsync(values.Select(x => x.Item1));
#endif
        var expectedResults = values.Select(x => x.Item2);
        using var expectedResultEnumerator = expectedResults.GetEnumerator();
#if !NET452
        await foreach (bool item in res)
#else
        foreach(bool item in res)
#endif
        {
            expectedResultEnumerator.MoveNext();
            Assert.Equal(expectedResultEnumerator.Current, item);
        }
    }

    internal static void TestBatchEnforceWithMatcher<T>(this IEnforcer e, string matcher, IEnumerable<(T, bool)> values)
        where T : IRequestValues =>
        Assert.True(values.Select(x => x.Item2)
            .SequenceEqual(e.BatchEnforceWithMatcher(matcher, values.Select(x => x.Item1))));

    internal static void TestBatchEnforceWithMatcherParallel<T>(this Enforcer e, string matcher,
        IEnumerable<(T, bool)> values)
        where T : IRequestValues =>
        Assert.True(values.Select(x => x.Item2)
            .SequenceEqual(e.BatchEnforceWithMatcherParallel<T>(matcher, values.Select(x => x.Item1).ToList())));

    internal static async void TestBatchEnforceWithMatcherAsync<T>(IEnforcer e, string matcher,
        IEnumerable<(T, bool)> values)
        where T : IRequestValues
    {
#if !NET452
        var res = e.BatchEnforceWithMatcherAsync(matcher, values.Select(x => x.Item1));
#else
        var res = await e.BatchEnforceWithMatcherAsync(matcher, values.Select(x => x.Item1));
#endif
        var expectedResults = values.Select(x => x.Item2);
        using var expectedResultEnumerator = expectedResults.GetEnumerator();
#if !NET452
        await foreach (bool item in res)
#else
        foreach(bool item in res)
#endif
        {
            expectedResultEnumerator.MoveNext();
            Assert.Equal(expectedResultEnumerator.Current, item);
        }
    }

    internal static void TestEnforceEx<T1, T2, T3>(this IEnforcer e, T1 sub, T2 obj, T3 act, List<string> except)
    {
        List<IEnumerable<string>> explains = e.EnforceEx(sub, obj, act).Item2.ToList();
        Assert.True(except.SetEquals(explains.FirstOrDefault() ?? []));
    }

    internal static async Task TestEnforceExAsync<T1, T2, T3>(this IEnforcer e, T1 sub, T2 obj, T3 act,
        List<string> except)
    {
        List<IEnumerable<string>> explains = (await e.EnforceExAsync(sub, obj, act)).Item2.ToList();
        Assert.True(except.SetEquals(explains.FirstOrDefault() ?? []));
    }

    internal static void TestEnforceExWithMatcher<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub, T2 obj, T3 act,
        List<string> except)
    {
        List<IEnumerable<string>> explains = e.EnforceExWithMatcher(matcher, sub, obj, act).Item2.ToList();
        Assert.True(except.SetEquals(explains.FirstOrDefault() ?? []));
    }

    internal static async Task TestEnforceExWithMatcherAsync<T1, T2, T3>(this IEnforcer e, string matcher, T1 sub,
        T2 obj, T3 act, List<string> except)
    {
        List<IEnumerable<string>> explains = (await e.EnforceExWithMatcherAsync(matcher, sub, obj, act)).Item2.ToList();
        Assert.True(except.SetEquals(explains.FirstOrDefault() ?? []));
    }

    internal static void TestGetPolicy(this IEnforcer e, List<List<string>> except)
    {
        IEnumerable<IEnumerable<string>> actual = e.GetPolicy();
        Assert.True(except.DeepEquals(actual));
    }

    internal static void TestGetFilteredPolicy(IEnforcer e, int fieldIndex, List<List<string>> except,
        params string[] fieldValues)
    {
        IEnumerable<IEnumerable<string>> actual = e.GetFilteredPolicy(fieldIndex, fieldValues);
        Assert.True(except.DeepEquals(actual));
    }

    internal static void TestGetFilteredGroupingPolicy(IEnforcer e, int fieldIndex, List<List<string>> except,
        params string[] fieldValues)
    {
        IEnumerable<IEnumerable<string>> actual = e.GetFilteredGroupingPolicy(fieldIndex, fieldValues);
        Assert.True(except.DeepEquals(actual));
    }

    internal static void TestGetRoles(IEnforcer e, string name, List<string> except, string domain = null)
    {
        List<string> actual = e.GetRolesForUser(name, domain).ToList();
        Assert.True(except.SetEquals(actual));
    }

    internal static void TestGetUsers(IEnforcer e, string name, List<string> except, string domain = null)
    {
        List<string> actual = e.GetUsersForRole(name, domain).ToList();
        Assert.True(except.SetEquals(actual));
    }

    internal static void TestGetPermissions(IEnforcer e, string name, List<List<string>> except, string domain = null)
    {
        IEnumerable<IEnumerable<string>> actual = e.GetPermissionsForUser(name, domain);
        Assert.True(except.DeepEquals(actual)); // TODO: why use SetEquals will be failed?
    }

    internal static void TestGetImplicitPermissions(IEnforcer e, string name, List<List<string>> except,
        string domain = null)
    {
        IEnumerable<IEnumerable<string>> actual = e.GetImplicitPermissionsForUser(name, domain);
        Assert.True(except.DeepEquals(actual)); // TODO: why use SetEquals will be failed?
    }

    internal static void TestGetRolesInDomain(IEnforcer e, string name, string domain, List<string> except)
    {
        List<string> actual = e.GetRolesForUserInDomain(name, domain).ToList();
        Assert.True(except.SetEquals(actual));
    }

    internal static void TestGetDomainsForUser(this IEnforcer e, string name, IEnumerable<string> except)
    {
        List<string> actual = e.GetDomainsForUser(name).ToList();
        Assert.True(except.SetEquals(actual));
    }

    internal static void TestGetImplicitRolesInDomain(IEnforcer e, string name, string domain, List<string> except)
    {
        List<string> actual = e.GetImplicitRolesForUser(name, domain).ToList();
        Assert.True(except.SetEquals(actual));
    }

    #region RoleManager test

    internal static void TestGetRoles(IRoleManager roleManager, string name, List<string> except)
    {
        List<string> actual = roleManager.GetRoles(name).ToList();
        Assert.True(except.SetEquals(actual));
    }

    internal static void TestGetRolesWithDomain(IRoleManager roleManager, string name, string domain,
        List<string> except)
    {
        List<string> actual = roleManager.GetRoles(name, domain).ToList();
        Assert.True(except.SetEquals(actual));
    }

    #endregion
}
