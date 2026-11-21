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
using System.Linq;
#if !NET452
#endif

namespace Casbin.Model
{
    public partial class DefaultPolicyStore : IPolicyStore
    {
        private readonly IDictionary<string, IDictionary<string, Node>> _nodesMap =
            new Dictionary<string, IDictionary<string, Node>>();

        public bool AddNode(string section, string type, PolicyAssertion policyAssertion)
        {
            if (_nodesMap.TryGetValue(section, out IDictionary<string, Node> nodes) is false)
            {
                nodes = new Dictionary<string, Node>();
                _nodesMap[section] = nodes;
            }

            if (nodes.ContainsKey(type))
            {
                return false;
            }

            Node node = new Node(policyAssertion);
            nodes[type] = node;
            return true;
        }

        public bool ContainsNodes(string section) => _nodesMap.ContainsKey(section);

        public bool ContainsNode(string section, string policyType) =>
            _nodesMap.TryGetValue(section, out IDictionary<string, Node> nodes) && nodes.ContainsKey(policyType);

        public int GetRequiredValuesCount(string section, string policyType) =>
            GetNode(section, policyType).RequiredValuesCount;

        public bool ValidatePolicy(string section, string policyType, IPolicyValues values)
        {
            Node node = GetNode(section, policyType);
            return node.ValidatePolicy(values);
        }

        public bool ValidatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            Node node = GetNode(section, policyType);
            return valuesList.Count == 0 || valuesList.All(node.ValidatePolicy);
        }

        public PolicyScanner Scan(string section, string policyType) =>
            new(GetNode(section, policyType).Iterate());

        public IEnumerable<IPolicyValues> GetPolicy(string section, string policyType)
            => GetNode(section, policyType).GetPolicy();

        public IEnumerable<string> GetPolicyTypes(string section)
            => GetNodes(section).Select(x => x.Key);

        public IDictionary<string, IEnumerable<string>> GetPolicyTypesAllSections()
        {
            Dictionary<string, IEnumerable<string>> res = new();
            foreach (var keyValuePair in _nodesMap)
            {
                res.Add(keyValuePair.Key, keyValuePair.Value.Select(x => x.Key));
            }

            return res;
        }

        public IDictionary<string, IEnumerable<IPolicyValues>> GetPolicyAllType(string section)
            => GetNodes(section).ToDictionary(kv =>
                kv.Key, x => GetPolicy(section, x.Key));

        public IEnumerable<IPolicyValues> GetFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues)
        {
            if (fieldValues is null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Count is 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return GetPolicy(section, policyType);
            }

            var result = new List<IPolicyValues>();
            Node node = GetNode(section, policyType);
            foreach (IPolicyValues rule in node.GetPolicy())
            {
                // Matched means all the fieldValue equals rule[fieldIndex + i].
                // when fieldValue is empty, this field will skip equals check.
                bool matched = !fieldValues.Where((fieldValue, i) =>
                        !string.IsNullOrWhiteSpace(fieldValue) &&
                        !rule[fieldIndex + i].Equals(fieldValue))
                    .Any();

                if (matched)
                {
                    result.Add(rule);
                }
            }

            return result;
        }

        public IEnumerable<IPolicyValues> RemoveFilteredPolicy(string section, string policyType, int fieldIndex,
            IPolicyValues fieldValues)
        {
            if (fieldValues is null)
            {
                throw new ArgumentNullException(nameof(fieldValues));
            }

            if (fieldValues.Count is 0 || fieldValues.All(string.IsNullOrWhiteSpace))
            {
                return null;
            }

            var newPolicy = new List<IPolicyValues>();
            List<IPolicyValues> effectPolicies = null;

            Node node = GetNode(section, policyType);
            foreach (IPolicyValues values in node.GetPolicy())
            {
                // Matched means all the fieldValue equals rule[fieldIndex + i].
                // when fieldValue is empty, this field will skip equals check.
                bool matched = !fieldValues.Where((fieldValue, i) =>
                        !string.IsNullOrWhiteSpace(fieldValue) &&
                        !values[fieldIndex + i].Equals(fieldValue))
                    .Any();

                if (matched)
                {
                    effectPolicies ??= new List<IPolicyValues>();
                    effectPolicies.Add(values);
                    node.PolicyTextSet.Remove(values.ToText());
                }
                else
                {
                    newPolicy.Add(values);
                }
            }

            node.SetPolicy(newPolicy);
            return effectPolicies;
        }

        public bool SortPolicyByPriority(string section, string policyType)
        {
            Node node = GetNode(section, policyType);
            return node.TrySortPolicyByPriority();
        }

        public bool SortPolicyBySubjectHierarchy(string section, string policyType,
            IDictionary<string, int> subjectHierarchyMap)
        {
            Node node = GetNode(section, policyType);
            return node.TrySortPoliciesBySubjectHierarchy(subjectHierarchyMap, GetNameWithDomain);
        }

        public IEnumerable<string> GetValuesForFieldInPolicyAllTypes(string section, int fieldIndex)
        {
            var values = new List<string>();
            IDictionary<string, Node> nodes = GetNodes(section);
            foreach (KeyValuePair<string, Node> node in nodes)
            {
                values.AddRange(GetValuesForFieldInPolicy(node.Value, fieldIndex));
            }

            return values;
        }

        public IEnumerable<string> GetValuesForFieldInPolicy(string section, string policyType, int fieldIndex)
        {
            Node node = GetNode(section, policyType);
            return GetValuesForFieldInPolicy(node, fieldIndex);
        }

        public bool AddPolicy(string section, string policyType, IPolicyValues values)
        {
            Node node = GetNode(section, policyType);
            return node.TryAddPolicy(values);
        }

        public bool HasPolicy(string section, string policyType, IPolicyValues values)
        {
            Node node = GetNode(section, policyType);
            return node.ContainsPolicy(values);
        }

        public bool HasPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            Node node = GetNode(section, policyType);
            return valuesList.Count == 0 || valuesList.Any(node.ContainsPolicy);
        }

        public bool HasAllPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            Node node = GetNode(section, policyType);
            return valuesList.Count is 0 || valuesList.All(node.ContainsPolicy);
        }

        public bool AddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            if (valuesList is null)
            {
                throw new ArgumentNullException(nameof(valuesList));
            }

            if (valuesList.Count is 0)
            {
                return true;
            }

            Node node = GetNode(section, policyType);
            foreach (IPolicyValues values in valuesList)
            {
                node.TryAddPolicy(values);
            }

            return true;
        }

        public bool UpdatePolicy(string section, string policyType, IPolicyValues oldValues, IPolicyValues newValues)
        {
            Node node = GetNode(section, policyType);
            return node.TryUpdatePolicy(oldValues, newValues);
        }

        public bool UpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> oldValuesList,
            IReadOnlyList<IPolicyValues> newValuesList)
        {
            if (oldValuesList is null)
            {
                throw new ArgumentNullException(nameof(oldValuesList));
            }

            if (newValuesList is null)
            {
                throw new ArgumentNullException(nameof(newValuesList));
            }

            if (oldValuesList.Count != newValuesList.Count)
            {
                return false;
            }

            Node node = GetNode(section, policyType);
            for (int i = 0; i < oldValuesList.Count; i++)
            {
                node.TryUpdatePolicy(oldValuesList[i], newValuesList[i]);
            }

            return true;
        }

        public bool RemovePolicy(string section, string policyType, IPolicyValues values)
        {
            Node node = GetNode(section, policyType);
            return node.TryRemovePolicy(values);
        }

        public bool RemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList)
        {
            if (valuesList is null)
            {
                throw new ArgumentNullException(nameof(valuesList));
            }

            if (valuesList.Count is 0)
            {
                return true;
            }

            Node node = GetNode(section, policyType);
            foreach (IPolicyValues values in valuesList)
            {
                node.TryRemovePolicy(values);
            }

            return true;
        }

        public void ClearPolicy()
        {
            foreach (KeyValuePair<string, IDictionary<string, Node>> nodes in _nodesMap)
            {
                foreach (KeyValuePair<string, Node> node in nodes.Value)
                {
                    node.Value.ClearPolicy();
                }
            }
        }

        private static string GetNameWithDomain(string domain, string name) =>
            domain + PermConstants.SubjectPrioritySeparatorString + name;

        private static IEnumerable<string> GetValuesForFieldInPolicy(Node node, int fieldIndex) =>
            node.GetPolicy().Select(rule => rule[fieldIndex]).Distinct().ToList();

        private IDictionary<string, Node> GetNodes(string section) => _nodesMap[section];

        private Node GetNode(string section, string type) => _nodesMap[section][type];
    }
}
