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

namespace Casbin.Model;

public interface IReadOnlyPolicyStore
{
    public bool ContainsNodes(string section);

    public bool ContainsNode(string section, string policyType);

    public int GetRequiredValuesCount(string section, string policyType);

    public bool ValidatePolicy(string section, string policyType, IPolicyValues values);

    public bool ValidatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList);

    public PolicyScanner Scan(string section, string policyType);

    public IEnumerable<IPolicyValues> GetPolicy(string section, string policyType);

    public IEnumerable<string> GetPolicyTypes(string section);

    public IDictionary<string, IEnumerable<string>> GetPolicyTypesAllSections();

    public IDictionary<string, IEnumerable<IPolicyValues>> GetPolicyAllType(string section);

    public IEnumerable<IPolicyValues> GetFilteredPolicy(string section, string policyType, int fieldIndex,
        IPolicyValues fieldValues);

    public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex);

    public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex);

    public bool HasPolicy(string section, string policyType, IPolicyValues values);

    public bool HasPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList);

    public bool HasAllPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList);
}
