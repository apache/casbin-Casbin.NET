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
using System.Threading.Tasks;

namespace Casbin.Persist;

public static class WatcherExExtension
{
    internal static void NotifyWatcherEx(this IWatcherEx watcherEx, PolicyChangedMessage message)
    {
        switch (message.Operation)
        {
            case PolicyOperation.AddPolicy:
                watcherEx.UpdateForAddPolicy(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.RemovePolicy:
                watcherEx.UpdateForRemovePolicy(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.UpdatePolicy:
                watcherEx.UpdateForUpdatePolicy(message.Section, message.PolicyType, message.Values, message.NewValues);
                break;
            case PolicyOperation.SavePolicy:
                watcherEx.UpdateForSavePolicy();
                break;
            case PolicyOperation.AddPolicies:
                watcherEx.UpdateForAddPolicies(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.RemovePolicies:
                watcherEx.UpdateForRemovePolicies(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.UpdatePolicies:
                watcherEx.UpdateForUpdatePolicies(message.Section, message.PolicyType, message.ValuesList,
                    message.NewValuesList);
                break;
            case PolicyOperation.RemoveFilteredPolicy:
                watcherEx.UpdateForRemoveFilteredPolicy(message.Section, message.PolicyType, message.FieldIndex,
                    message.Values);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal static async Task NotifyWatcherExAsync(this IWatcherEx watcherEx, PolicyChangedMessage message)
    {
        switch (message.Operation)
        {
            case PolicyOperation.AddPolicy:
                await watcherEx.UpdateForAddPolicyAsync(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.RemovePolicy:
                await watcherEx.UpdateForRemovePolicyAsync(message.Section, message.PolicyType, message.Values);
                break;
            case PolicyOperation.UpdatePolicy:
                await watcherEx.UpdateForUpdatePolicyAsync(message.Section, message.PolicyType, message.Values,
                    message.NewValues);
                break;
            case PolicyOperation.SavePolicy:
                await watcherEx.UpdateForSavePolicyAsync();
                break;
            case PolicyOperation.AddPolicies:
                await watcherEx.UpdateForAddPoliciesAsync(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.RemovePolicies:
                await watcherEx.UpdateForRemovePoliciesAsync(message.Section, message.PolicyType, message.ValuesList);
                break;
            case PolicyOperation.UpdatePolicies:
                await watcherEx.UpdateForUpdatePoliciesAsync(message.Section, message.PolicyType, message.ValuesList,
                    message.NewValuesList);
                break;
            case PolicyOperation.RemoveFilteredPolicy:
                await watcherEx.UpdateForRemoveFilteredPolicyAsync(message.Section, message.PolicyType,
                    message.FieldIndex, message.Values);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
