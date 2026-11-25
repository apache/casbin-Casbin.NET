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
using System.Threading;

namespace Casbin.Model;

public partial class DefaultPolicyStore
{
    internal class Node
    {
        private readonly PolicyAssertion _assertion;

        public List<IPolicyValues> Policy = new();
        public Node(PolicyAssertion assertion) => _assertion = assertion;
        public HashSet<string> PolicyTextSet { get; } = new();
        public ReaderWriterLockSlim Lock { get; } = new();
        public int RequiredValuesCount => _assertion.Tokens.Count;

        public Iterator Iterate() => new(this);

        public IEnumerable<IPolicyValues> GetPolicy()
        {
            Iterator iterator = new(this);
            List<IPolicyValues> policyList = new();
            while (iterator.GetNext(out IPolicyValues policyValues))
            {
                policyList.Add(policyValues);
            }

            return policyList;
        }

        public IReadOnlyList<IPolicyValues> SetPolicy(List<IPolicyValues> valuesList) =>
            Interlocked.Exchange(ref Policy, valuesList);

        public bool ContainsPolicy(IPolicyValues values)
        {
            Lock.EnterReadLock();
            try
            {
                return PolicyTextSet.Contains(values.ToText());
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }

        public bool ValidatePolicy(IPolicyValues values)
        {
            if (_assertion.Section is PermConstants.Section.RoleSection)
            {
                return RequiredValuesCount <= values.Count;
            }

            return RequiredValuesCount == values.Count;
        }

        public bool TryAddPolicy(IPolicyValues values)
        {
            if (ValidatePolicy(values) is false)
            {
                return false;
            }

            if (ContainsPolicy(values))
            {
                return false;
            }

            if (_assertion.TryGetPriorityIndex(out int index))
            {
                return TryAddPolicyByPriority(values, index);
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Add(values);
                PolicyTextSet.Add(values.ToText());
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public bool TryUpdatePolicy(IPolicyValues oldValues, IPolicyValues newValues)
        {
            if (ValidatePolicy(newValues) is false)
            {
                return false;
            }

            if (ContainsPolicy(oldValues) is false)
            {
                return false;
            }

            Lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < Policy.Count; i++)
                {
                    IPolicyValues v = Policy[i];
                    if (v.Equals(oldValues) is false)
                    {
                        continue;
                    }

                    Policy.RemoveAt(i);
                    Policy.Insert(i, newValues);
                    PolicyTextSet.Remove(oldValues.ToText());
                    PolicyTextSet.Add(newValues.ToText());
                    return true;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public bool TryUpdatePolicies(IReadOnlyList<IPolicyValues> oldValuesList,
            IReadOnlyList<IPolicyValues> newValuesList)
        {
            if (newValuesList.Any(values => ValidatePolicy(values) is false))
            {
                return false;
            }

            Lock.EnterWriteLock();
            try
            {
                // Resolve all indexes before writing, since newValuesList may reuse rules of oldValuesList.
                int[] indexes = new int[oldValuesList.Count];
                HashSet<int> resolvedIndexes = new();
                for (int i = 0; i < oldValuesList.Count; i++)
                {
                    IPolicyValues oldValues = oldValuesList[i];
                    int index = Policy.FindIndex(v => v.Equals(oldValues));
                    if (index is -1 || resolvedIndexes.Add(index) is false)
                    {
                        return false;
                    }

                    indexes[i] = index;
                }

                foreach (IPolicyValues oldValues in oldValuesList)
                {
                    PolicyTextSet.Remove(oldValues.ToText());
                }

                for (int i = 0; i < newValuesList.Count; i++)
                {
                    Policy[indexes[i]] = newValuesList[i];
                    PolicyTextSet.Add(newValuesList[i].ToText());
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public bool TryRemovePolicy(IPolicyValues values)
        {
            if (ContainsPolicy(values) is false)
            {
                return false;
            }

            Lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < Policy.Count; i++)
                {
                    IPolicyValues v = Policy[i];
                    if (v.Equals(values) is false)
                    {
                        continue;
                    }

                    Policy.RemoveAt(i);
                    PolicyTextSet.Remove(values.ToText());
                    return true;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public void ClearPolicy()
        {
            Interlocked.Exchange(ref Policy, new List<IPolicyValues>());
            PolicyTextSet.Clear();
        }

        private bool TrySortPoliciesByPriority()
        {
            if (_assertion.TryGetPriorityIndex(out int priorityIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string priorityString1 = p1[priorityIndex];
                string priorityString2 = p2[priorityIndex];

                if (int.TryParse(priorityString1, out int priority1) is false
                    || int.TryParse(priorityString2, out int priority2) is false)
                {
                    return string.CompareOrdinal(priorityString1, priorityString2);
                }

                return priority1 - priority2;
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        private bool TrySortPoliciesBySubjectHierarchy(Dictionary<string, int> subjectHierarchyMap)
        {
            if (_assertion.TryGetDomainIndex(out int domainIndex) is false)
            {
                domainIndex = -1;
            }

            if (_assertion.TryGetSubjectIndex(out int subjectIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string domain1 = "", domain2 = "";
                if (domainIndex != -1)
                {
                    domain1 = p1[domainIndex];
                    domain2 = p2[domainIndex];
                }

                string name1 = RoleAssertion.GetNameWithDomain(domain1, p1[subjectIndex]);
                string name2 = RoleAssertion.GetNameWithDomain(domain2, p2[subjectIndex]);

                return subjectHierarchyMap[name1] - subjectHierarchyMap[name2];
            }


            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        private bool TryAddPolicyByPriority(IPolicyValues values, int priorityIndex)
        {
            if (int.TryParse(values[priorityIndex], out int priority) is false)
            {
                return false;
            }

            bool LastLessOrEqualPriority(IPolicyValues v)
            {
                return int.Parse(v[priorityIndex]) <= priority;
            }

            Lock.EnterWriteLock();
            try
            {
                int lastIndex = Policy.FindLastIndex(LastLessOrEqualPriority);
                Policy.Insert(lastIndex + 1, values);
                PolicyTextSet.Add(values.ToText());
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }


        internal bool TrySortPolicyByPriority()
        {
            if (_assertion.TryGetPriorityIndex(out int priorityIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string priorityString1 = p1[priorityIndex];
                string priorityString2 = p2[priorityIndex];

                if (int.TryParse(priorityString1, out int priority1) is false
                    || int.TryParse(priorityString2, out int priority2) is false)
                {
                    return string.CompareOrdinal(priorityString1, priorityString2);
                }

                return priority1 - priority2;
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        internal bool TrySortPoliciesBySubjectHierarchy(IDictionary<string, int> subjectHierarchyMap,
            Func<string, string, string> nameFormatter)
        {
            if (_assertion.TryGetDomainIndex(out int domainIndex) is false)
            {
                domainIndex = -1;
            }

            if (_assertion.TryGetSubjectIndex(out int subjectIndex) is false)
            {
                return false;
            }


            int GetSubjectLevel(IPolicyValues policy)
            {
                string domain = domainIndex != -1 ? policy[domainIndex] : "";
                string name = nameFormatter(domain, policy[subjectIndex]);
                return subjectHierarchyMap.TryGetValue(name, out int level) ? level : 0;
            }

            Lock.EnterWriteLock();
            try
            {
                List<IPolicyValues> sortedPolicy = Policy.OrderByDescending(GetSubjectLevel).ToList();
                Policy.Clear();
                Policy.AddRange(sortedPolicy);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }
    }
}
