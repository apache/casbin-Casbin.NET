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

namespace Casbin.Persist;

/// <summary>
///     WatcherEx is the strengthened Casbin watchers.
///     It is used to be compatible with the Golang Casbin version,
///     It is not necessary to use it in C# because the IIncrementalWatcher exists.
/// </summary>
public interface IWatcherEx : IWatcher
{
    /// <summary>
    ///     UpdateForAddPolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.AddPolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    void UpdateForAddPolicy(string section, string policyType, IPolicyValues values);

    /// <summary>
    ///     UpdateForAddPolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.AddPolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    Task UpdateForAddPolicyAsync(string section, string policyType, IPolicyValues values);

    /// <summary>
    ///     UpdateForRemovePolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemovePolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    void UpdateForRemovePolicy(string section, string policyType, IPolicyValues values);

    /// <summary>
    ///     UpdateForRemovePolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemovePolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    Task UpdateForRemovePolicyAsync(string section, string policyType, IPolicyValues values);

    /// <summary>
    ///     UpdateForRemoveFilteredPolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemoveFilteredNamedGroupingPolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="fieldIndex"></param>
    /// <param name="fieldValues"></param>
    void UpdateForRemoveFilteredPolicy(string section, string policyType, int fieldIndex, IPolicyValues fieldValues);

    /// <summary>
    ///     UpdateForRemoveFilteredPolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemoveFilteredNamedGroupingPolicy()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="fieldIndex"></param>
    /// <param name="fieldValues"></param>
    Task UpdateForRemoveFilteredPolicyAsync(string section, string policyType, int fieldIndex,
        IPolicyValues fieldValues);

    /// <summary>
    ///     UpdateForSavePolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemoveFilteredNamedGroupingPolicy()
    /// </summary>
    void UpdateForSavePolicy();

    /// <summary>
    ///     UpdateForSavePolicy calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemoveFilteredNamedGroupingPolicy()
    /// </summary>
    Task UpdateForSavePolicyAsync();

    /// <summary>
    ///     UpdateForAddPolicies calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.AddPolicies()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    void UpdateForAddPolicies(string section, string policyType, IReadOnlyList<IPolicyValues> values);

    /// <summary>
    ///     UpdateForAddPolicies calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.AddPolicies()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    Task UpdateForAddPoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> values);

    /// <summary>
    ///     UpdateForRemovePolicies calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemovePolicies()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    void UpdateForRemovePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> values);

    /// <summary>
    ///     UpdateForRemovePolicies calls the update callback of other instances to synchronize their policy.
    ///     It is called after Enforcer.RemovePolicies()
    /// </summary>
    /// <param name="section"></param>
    /// <param name="policyType"></param>
    /// <param name="values"></param>
    Task UpdateForRemovePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> values);

    void UpdateForUpdatePolicy(string section, string policyType, IPolicyValues values, IPolicyValues newValues);

    Task UpdateForUpdatePolicyAsync(string section, string policyType, IPolicyValues values, IPolicyValues newValues);

    void UpdateForUpdatePolicies(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList,
        IReadOnlyList<IPolicyValues> newValues);

    Task UpdateForUpdatePoliciesAsync(string section, string policyType, IReadOnlyList<IPolicyValues> valuesList,
        IReadOnlyList<IPolicyValues> newValues);
}
