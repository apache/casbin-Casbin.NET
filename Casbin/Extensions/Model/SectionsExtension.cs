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

#nullable enable
using System.Collections.Generic;

namespace Casbin.Model;

public static class SectionsExtension
{
    public static string GetValue(this ISections sections, string section, string type) =>
        sections.GetAssertion<Assertion>(section, type).Value;

    public static RequestAssertion? GetRequestAssertion(this ISections sections, string type) =>
        sections.GetRequestAssertion(PermConstants.Section.RequestSection, type);

    public static RequestAssertion? GetRequestAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<RequestAssertion>(section, type);

    public static PolicyAssertion GetPolicyAssertion(this ISections sections, string type) =>
        sections.GetPolicyAssertion(PermConstants.Section.PolicySection, type);

    public static PolicyAssertion GetPolicyAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<PolicyAssertion>(section, type);

    public static IDictionary<string, PolicyAssertion> GetPolicyAssertions(this ISections sections) =>
        sections.GetPolicyAssertions(PermConstants.Section.PolicySection);

    public static IDictionary<string, PolicyAssertion> GetPolicyAssertions(this ISections sections, string section) =>
        sections.GetAssertions<PolicyAssertion>(section);

    public static RoleAssertion GetRoleAssertion(this ISections sections, string type) =>
        sections.GetRoleAssertion(PermConstants.Section.RoleSection, type);

    public static RoleAssertion GetRoleAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<RoleAssertion>(section, type);

    public static IDictionary<string, RoleAssertion> GetRoleAssertions(this ISections sections) =>
        sections.GetRoleAssertions(PermConstants.Section.RoleSection);

    public static IDictionary<string, RoleAssertion> GetRoleAssertions(this ISections sections, string section) =>
        sections.GetAssertions<RoleAssertion>(section);

    public static PolicyEffectAssertion GetPolicyEffectAssertion(this ISections sections, string type) =>
        sections.GetPolicyEffectAssertion(PermConstants.Section.PolicyEffectSection, type);

    public static PolicyEffectAssertion
        GetPolicyEffectAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<PolicyEffectAssertion>(section, type);

    public static MatcherAssertion GetMatcherAssertion(this ISections sections, string type) =>
        sections.GetMatcherAssertion(PermConstants.Section.MatcherSection, type);

    public static MatcherAssertion GetMatcherAssertion(this ISections sections, string section, string type) =>
        sections.GetAssertion<MatcherAssertion>(section, type);
}
