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
using Casbin.Model;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.GenericTests;

[Collection("Model collection")]
public class SupportCountTest
{
    [Fact]
    public void TestSupportCount()
    {
        for (int i = 1; i <= 14; i++)
        {
            IEnforcer enforcer = new Enforcer(DefaultModel.CreateFromText(TestModelFixture.SupportCountModelText));
            string policyType = $"p{i}";
            string requestType = $"r{i}";
            string matcherType = $"m{i}";
            if (i is 1)
            {
                policyType = PermConstants.DefaultPolicyType;
                requestType = PermConstants.DefaultRequestType;
                matcherType = PermConstants.DefaultMatcherType;
            }

            enforcer.AddNamedPolicy(policyType, CreateTestPolicy(i));
            TestEnforce(enforcer, enforcer.CreateContext(requestType, policyType,
                PermConstants.DefaultPolicyEffectType, matcherType), i);
        }
    }

    private static void TestEnforce(IEnforcer enforcer, EnforceContext context, int requestCount)
    {
        switch (requestCount)
        {
            case 1:
                Assert.True(enforcer.Enforce(context, "value1"));
                Assert.True(enforcer.Enforce(context, ["value1"]));
                break;
            case 2:
                Assert.True(enforcer.Enforce(context, "value1", "value2"));
                Assert.True(enforcer.Enforce(context, ["value1", "value2"]));
                break;
            case 3:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3"));
                Assert.True(enforcer.Enforce(context, ["value1", "value2", "value3"]));
                break;
            case 4:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4"));
                Assert.True(enforcer.Enforce(context, ["value1", "value2", "value3", "value4"]));
                break;
            case 5:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5"));
                Assert.True(enforcer.Enforce(context, ["value1", "value2", "value3", "value4", "value5"]));
                break;
            case 6:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6"));
                Assert.True(enforcer.Enforce(context,
                    ["value1", "value2", "value3", "value4", "value5", "value6"]));
                break;
            case 7:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7"));
                Assert.True(enforcer.Enforce(context,
                    ["value1", "value2", "value3", "value4", "value5", "value6", "value7"]));
                break;
            case 8:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8"));
                Assert.True(enforcer.Enforce(context,
                    ["value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8"]));
                break;
            case 9:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9"));
                Assert.True(enforcer.Enforce(context,
                [
                    "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8", "value9"
                ]));
                break;
            case 10:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10"));
                Assert.True(enforcer.Enforce(context,
                [
                    "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8", "value9",
                        "value10"
                ]));
                break;
            case 11:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11"));
                Assert.True(enforcer.Enforce(context,
                [
                    "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8", "value9",
                        "value10", "value11"
                ]));
                break;
            case 12:
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11", "value12"));
                Assert.True(enforcer.Enforce(context,
                [
                    "value1", "value2", "value3", "value4", "value5", "value6", "value7", "value8", "value9",
                        "value10", "value11", "value12"
                ]));
                break;
            case 13:
                Assert.True(enforcer.Enforce(context, Request.CreateValues("value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11", "value12", "value13")));
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3", "value4", "value5", "value6",
                    "value7", "value8", "value9", "value10", "value11", "value12", "value13"));
                break;
            case 14:
                Assert.True(enforcer.Enforce(context, Request.CreateValues("value1", "value2", "value3",
                    "value4", "value5", "value6", "value7", "value8", "value9",
                    "value10", "value11", "value12", "value13", "value14")));
                Assert.True(enforcer.Enforce(context, "value1", "value2", "value3", "value4", "value5", "value6",
                    "value7", "value8", "value9", "value10", "value11", "value12", "value13", "value14"));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requestCount));
        }
    }

    private static IPolicyValues CreateTestPolicy(int count)
    {
        List<string> policy = [];
        for (int i = 0; i < count; i++)
        {
            policy.Add($"value{i + 1}");
        }

        return Policy.ValuesFrom(policy);
    }
}
