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
using Casbin.Functions;
using Xunit;

namespace Casbin.UnitTests.UtilTests;

public class BuiltInFunctionTest
{
    public static IEnumerable<object[]> ipMatchTestData =
    [
        ["192.168.2.123", "192.168.2.0/24", true],
        ["192.168.2.123", "192.168.3.0/24", false],
        ["192.168.2.123", "192.168.2.0/16", true],
        ["192.168.2.123", "192.168.2.123", true],
        ["192.168.2.123", "192.168.2.123/32", true],
        ["10.0.0.11", "10.0.0.0/8", true], ["11.0.0.123", "10.0.0.0/8", false],
        ["2001:db8::1", "2001:db8::1", true], ["2001:db8::1", "2001:db9::1", false],
        ["2001:db8::1", "2001:db8::1/128", true],
        ["2001:db8::1", "2001:db9::/64", false]
    ];

    public static IEnumerable<object[]> regexMatchTestData =
    [
        ["/topic/create", "/topic/create", true],
        ["/topic/create/123", "/topic/create", true],
        ["/topic/delete", "/topic/create", false],
        ["/topic/edit", "/topic/edit/[0-9]+", false],
        ["/topic/edit/123", "/topic/edit/[0-9]+", true],
        ["/topic/edit/abc", "/topic/edit/[0-9]+", false],
        ["/foo/delete/123", "/topic/delete/[0-9]+", false],
        ["/topic/delete/0", "/topic/delete/[0-9]+", true],
        ["/topic/edit/123s", "/topic/delete/[0-9]+", false]
    ];

    public static IEnumerable<object[]> KeyGetTestData =
    [
        ["/foo", "/foo", ""], ["/foo", "/foo*", ""],
        ["/foo", "/foo/*", ""], ["/foo/bar", "/foo", ""],
        ["/foo/bar", "/foo*", "/bar"], ["/foo/bar", "/foo/*", "bar"],
        ["/foobar", "/foo", ""], ["/foobar", "/foo*", "bar"],
        ["/foobar", "/foo/*", ""]
    ];

    public static IEnumerable<object[]> KeyGet2TestData =
    [
        ["/foo", "/foo", "id", ""], ["/foo", "/foo*", "id", ""],
        ["/foo", "/foo/*", "id", ""], ["/foo/bar", "/foo", "id", ""],
        ["/foo/bar", "/foo*", "id", ""], // different with KeyMatch.
        ["/foo/bar", "/foo/*", "id", ""], ["/foobar", "/foo", "id", ""],
        ["/foobar", "/foo*", "id", ""], // different with KeyMatch.
        ["/foobar", "/foo/*", "id", ""], ["/", "/:resource", "resource", ""],
        ["/resource1", "/:resource", "resource", "resource1"],
        ["/myid", "/:id/using/:resId", "id", ""],
        ["/myid/using/myresid", "/:id/using/:resId", "id", "myid"],
        ["/myid/using/myresid", "/:id/using/:resId", "resId", "myresid"],
        ["/proxy/myid", "/proxy/:id/*", "id", ""],
        ["/proxy/myid/", "/proxy/:id/*", "id", "myid"],
        ["/proxy/myid/res", "/proxy/:id/*", "id", "myid"],
        ["/proxy/myid/res/res2", "/proxy/:id/*", "id", "myid"],
        ["/proxy/myid/res/res2/res3", "/proxy/:id/*", "id", "myid"],
        ["/proxy/myid/res/res2/res3", "/proxy/:id/res/*", "id", "myid"],
        ["/proxy/", "/proxy/:id/*", "id", ""], ["/alice", "/:id", "id", "alice"],
        ["/alice/all", "/:id/all", "id", "alice"], ["/alice", "/:id/all", "id", ""],
        ["/alice/all", "/:id", "id", ""], ["/alice/all", "/:/all", "", ""]
    ];

    public static IEnumerable<object[]> KeyGet3TestData =
    [
        ["/", "/{resource}", "resource", ""],
        ["/resource1", "/{resource}", "resource", "resource1"],
        ["/myid", "/{id}/using/{resId}", "id", ""],
        ["/myid/using/myresid", "/{id}/using/{resId}", "id", "myid"],
        ["/myid/using/myresid", "/{id}/using/{resId}", "resId", "myresid"],
        ["/proxy/myid", "/proxy/{id}/*", "id", ""],
        ["/proxy/myid/", "/proxy/{id}/*", "id", "myid"],
        ["/proxy/myid/res", "/proxy/{id}/*", "id", "myid"],
        ["/proxy/myid/res/res2", "/proxy/{id}/*", "id", "myid"],
        ["/proxy/myid/res/res2/res3", "/proxy/{id}/*", "id", "myid"],
        ["/proxy/", "/proxy/{id}/*", "id", ""],
        ["/api/group1_group_name/project1_admin/info", "/api/{proj}_admin/info", "proj", ""],
        ["/{id/using/myresid", "/{id/using/{resId}", "resId", "myresid"],
        ["/{id/using/myresid/status}", "/{id/using/{resId}/status}", "resId", "myresid"],
        ["/proxy/myid/res/res2/res3", "/proxy/{id}/*/{res}", "res", "res3"],
        ["/api/project1_admin/info", "/api/{proj}_admin/info", "proj", "project1"],
        [
            "/api/group1_group_name/project1_admin/info", "/api/{g}_{gn}/{proj}_admin/info", "g", "group1"
        ],
        [
            "/api/group1_group_name/project1_admin/info", "/api/{g}_{gn}/{proj}_admin/info", "gn", "group_name"
        ],
        [
            "/api/group1_group_name/project1_admin/info", "/api/{g}_{gn}/{proj}_admin/info", "proj", "project1"
        ]
    ];

    public static IEnumerable<object[]> keyMatchTestData =
    [
        ["/foo", "/foo", true], ["/foo", "/foo*", true],
        ["/foo", "/foo/*", false], ["/foo/bar", "/foo", false],
        ["/foo/bar", "/foo*", true], ["/foo/bar", "/foo/*", true],
        ["/foobar", "/foo", false], ["/foobar", "/foo*", true],
        ["/foobar", "/foo/*", false]
    ];

    public static IEnumerable<object[]> KeyMatch2TestData =
    [
        ["/foo", "/foo", true], ["/foo", "/foo*", true],
        ["/foo", "/foo/*", false], ["/foo/bar", "/foo", false],
        ["/foo/bar", "/foo*", false], // different with KeyMatch.
        ["/foo/bar", "/foo/*", true], ["/foobar", "/foo", false],
        ["/foobar", "/foo*", false], // different with KeyMatch.
        ["/foobar", "/foo/*", false], ["/", "/:resource", false],
        ["/resource1", "/:resource", true], ["/myid", "/:id/using/:resId", false],
        ["/myid/using/myresid", "/:id/using/:resId", true],
        ["/proxy/myid", "/proxy/:id/*", false], ["/proxy/myid/", "/proxy/:id/*", true],
        ["/proxy/myid/res", "/proxy/:id/*", true],
        ["/proxy/myid/res/res2", "/proxy/:id/*", true],
        ["/proxy/myid/res/res2/res3", "/proxy/:id/*", true],
        ["/proxy/", "/proxy/:id/*", false], ["/alice", "/:id", true],
        ["/alice/all", "/:id/all", true], ["/alice", "/:id/all", false],
        ["/alice/all", "/:id", false], ["/alice/all", "/:/all", false]
    ];

    public static IEnumerable<object[]> KeyMatch3TestData =
    [
        // keyMatch3(}, is similar with KeyMatch2(},, except using "/proxy/{id}" instead of "/proxy/:id".
        ["/foo", "/foo", true], ["/foo", "/foo*", true],
        ["/foo", "/foo/*", false], ["/foo/bar", "/foo", false],
        ["/foo/bar", "/foo*", false], ["/foo/bar", "/foo/*", true],
        ["/foobar", "/foo", false], ["/foobar", "/foo*", false],
        ["/foobar", "/foo/*", false], ["/", "/{resource}", false],
        ["/resource1", "/{resource}", true], ["/myid", "/{id}/using/{resId}", false],
        ["/myid/using/myresid", "/{id}/using/{resId}", true],
        ["/proxy/myid", "/proxy/{id}/*", false], ["/proxy/myid/", "/proxy/{id}/*", true],
        ["/proxy/myid/res", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2/res3", "/proxy/{id}/*", true],
        ["/proxy/", "/proxy/{id}/*", false],
        ["/myid/using/myresid", "/{id/using/{resId}", false]
    ];

    public static IEnumerable<object[]> KeyMatch4TestData =
    [
        ["/parent/123/child/123", "/parent/{id}/child/{id}", true],
        ["/parent/123/child/123", "/parent/{i/d}/child/{i/d}", false],
        ["/parent/123/child/456", "/parent/{id}/child/{id}", false],
        ["/parent/123/child/123", "/parent/{id}/child/{another_id}", true],
        ["/parent/123/child/456", "/parent/{id}/child/{another_id}", true],
        ["/parent/123/child/123/book/123", "/parent/{id}/child/{id}/book/{id}", true],
        ["/parent/123/child/123/book/456", "/parent/{id}/child/{id}/book/{id}", false],
        ["/parent/123/child/456/book/123", "/parent/{id}/child/{id}/book/{id}", false],
        ["/parent/123/child/456/book/", "/parent/{id}/child/{id}/book/{id}", false],
        ["/parent/123/child/456", "/parent/{id}/child/{id}/book/{id}", false]
    ];

    public static IEnumerable<object[]> KeyMatch5TestData =
    [
        ["/parent/child?status=1&type=2", "/parent/child", true],
        ["/parent?status=1&type=2", "/parent/child", false],
        ["/parent/child/?status=1&type=2", "/parent/child/", true],
        ["/parent/child/?status=1&type=2", "/parent/child", false],
        ["/parent/child?status=1&type=2", "/parent/child/", false],
        // wildcard *
        ["/foo", "/foo", true],
        ["/foo", "/foo*", true],
        ["/foo", "/foo/*", false],
        ["/foo/bar", "/foo", false],
        ["/foo/bar", "/foo*", false],
        ["/foo/bar", "/foo/*", true],
        ["/foobar", "/foo", false],
        ["/foobar", "/foo*", false],
        ["/foobar", "/foo/*", false],
        // {param} placeholders
        ["/", "/{resource}", false],
        ["/resource1", "/{resource}", true],
        ["/myid", "/{id}/using/{resId}", false],
        ["/myid/using/myresid", "/{id}/using/{resId}", true],
        // {param} + wildcard
        ["/proxy/myid", "/proxy/{id}/*", false],
        ["/proxy/myid/", "/proxy/{id}/*", true],
        ["/proxy/myid/res", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2/res3", "/proxy/{id}/*", true],
        ["/proxy/", "/proxy/{id}/*", false],
        // query strings with {param} + wildcard
        ["/proxy/myid?status=1&type=2", "/proxy/{id}/*", false],
        ["/proxy/myid/res?status=1&type=2", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2?status=1&type=2", "/proxy/{id}/*", true],
        ["/proxy/myid/res/res2/res3?status=1&type=2", "/proxy/{id}/*", true]
    ];

    public static IEnumerable<object[]> GlobMatchTestData =
    [
        ["/foo", "/foo", true], ["/foo", "/foo*", true],
        ["/foo", "/foo/*", false], ["/foo/bar", "/foo", false],
        ["/foo/bar", "/foo*", false], ["/foo/bar", "/foo/*", true],
        ["/foobar", "/foo", false], ["/foobar", "/foo*", true],
        ["/foobar", "/foo/*", false], ["/foo", "*/foo", true],
        ["/foo", "*/foo*", true], ["/foo", "*/foo/*", false],
        ["/foo/bar", "*/foo", false], ["/foo/bar", "*/foo*", false],
        ["/foo/bar", "*/foo/*", true], ["/foobar", "*/foo", false],
        ["/foobar", "*/foo*", true], ["/foobar", "*/foo/*", false],
        ["/prefix/foo", "*/foo", false], ["/prefix/foo", "*/foo*", false],
        ["/prefix/foo", "*/foo/*", false], ["/prefix/foo/bar", "*/foo", false],
        ["/prefix/foo/bar", "*/foo*", false], ["/prefix/foo/bar", "*/foo/*", false],
        ["/prefix/foobar", "*/foo", false], ["/prefix/foobar", "*/foo*", false],
        ["/prefix/foobar", "*/foo/*", false],
        ["/prefix/subprefix/foo", "*/foo", false],
        ["/prefix/subprefix/foo", "*/foo*", false],
        ["/prefix/subprefix/foo", "*/foo/*", false],
        ["/prefix/subprefix/foo/bar", "*/foo", false],
        ["/prefix/subprefix/foo/bar", "*/foo*", false],
        ["/prefix/subprefix/foo/bar", "*/foo/*", false],
        ["/prefix/subprefix/foobar", "*/foo", false],
        ["/prefix/subprefix/foobar", "*/foo*", false],
        ["/prefix/subprefix/foobar", "*/foo/*", false]
    ];

    [Theory]
    [MemberData(nameof(ipMatchTestData))]
    public void TestIPMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.IPMatch(key1, key2));

    [Theory]
    [MemberData(nameof(regexMatchTestData))]
    public void TestRegexMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.RegexMatch(key1, key2));

    [Theory]
    [MemberData(nameof(KeyGetTestData))]
    public void TestKeyGet(string key1, string key2, string expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyGet(key1, key2));

    [Theory]
    [MemberData(nameof(KeyGet2TestData))]
    public void TestKeyGet2(string key1, string key2, string pathVar, string expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyGet2(key1, key2, pathVar));

    [Theory]
    [MemberData(nameof(KeyGet3TestData))]
    public void TestKeyGet3(string key1, string key2, string pathVar, string expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyGet3(key1, key2, pathVar));

    [Theory]
    [MemberData(nameof(keyMatchTestData))]
    public void TestKeyMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch2TestData))]
    public void TestKeyMatch2(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch2(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch3TestData))]
    public void TestKeyMatch3(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch3(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch4TestData))]
    public void TestKeyMatch4(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch4(key1, key2));

    [Theory]
    [MemberData(nameof(KeyMatch5TestData))]
    public void TestKeyMatch5(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.KeyMatch5(key1, key2));

    [Theory]
    [MemberData(nameof(GlobMatchTestData))]
    public void TestGlobMatch(string key1, string key2, bool expectedResult) =>
        Assert.Equal(expectedResult,
            BuiltInFunctions.GlobMatch(key1, key2));
}
