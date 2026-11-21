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

namespace Casbin.Persist;

public class PolicyChangedMessage : IPolicyChangeMessage
{
    public PolicyOperation Operation { get; private init; }
    public string Section { get; private init; }
    public string PolicyType { get; private init; }
    public int FieldIndex { get; private init; }
    public IPolicyValues Values { get; private init; }
    public IPolicyValues NewValues { get; private init; }
    public IReadOnlyList<IPolicyValues> ValuesList { get; private init; }
    public IReadOnlyList<IPolicyValues> NewValuesList { get; private init; }

    public static PolicyChangedMessage CreateAddPolicy(string section, string policyType, IPolicyValues rule) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.AddPolicy, Section = section, PolicyType = policyType, Values = rule
        };

    public static PolicyChangedMessage CreateUpdatePolicy(string section, string policyType,
        IPolicyValues values, IPolicyValues newValues) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.UpdatePolicy,
            Section = section,
            PolicyType = policyType,
            Values = values,
            NewValues = newValues
        };

    public static PolicyChangedMessage CreateRemovePolicy(string section, string policyType, IPolicyValues values) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.RemovePolicy, Section = section, PolicyType = policyType, Values = values
        };

    public static PolicyChangedMessage CreateRemoveFilteredPolicy(string section, string policyType,
        int fieldIndex, IReadOnlyList<IPolicyValues> fieldValues) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.RemoveFilteredPolicy,
            Section = section,
            PolicyType = policyType,
            FieldIndex = fieldIndex,
            ValuesList = fieldValues
        };

    public static PolicyChangedMessage CreateAddPolicies(string section, string policyType,
        IReadOnlyList<IPolicyValues> rules) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.AddPolicies, Section = section, PolicyType = policyType, ValuesList = rules
        };

    public static PolicyChangedMessage CreateUpdatePolicies(string section, string policyType,
        IReadOnlyList<IPolicyValues> valuesList, IReadOnlyList<IPolicyValues> newValueList) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.UpdatePolicies,
            Section = section,
            PolicyType = policyType,
            ValuesList = valuesList,
            NewValuesList = newValueList
        };

    public static PolicyChangedMessage CreateRemovePolicies(string section, string policyType,
        IReadOnlyList<IPolicyValues> valuesList) =>
        new PolicyChangedMessage
        {
            Operation = PolicyOperation.RemovePolicies,
            Section = section,
            PolicyType = policyType,
            ValuesList = valuesList
        };

    public static PolicyChangedMessage CreateSavePolicy() =>
        new PolicyChangedMessage { Operation = PolicyOperation.SavePolicy };
}
