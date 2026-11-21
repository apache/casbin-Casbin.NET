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
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Casbin.UnitTests.Fixtures;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

[Collection("Model collection")]
public class WatcherMessageTest
{
    private readonly TestModelFixture TestModelFixture;

    public WatcherMessageTest(TestModelFixture testModelFixture) => TestModelFixture = testModelFixture;

    private void MessageEquals(IPolicyChangeMessage message, IPolicyChangeMessage message2)
    {
        Assert.Equal(message.Operation, message2.Operation);
        Assert.Equal(message.Section, message2.Section);
        Assert.Equal(message.Values, message2.Values);
        Assert.Equal(message.NewValues, message2.NewValues);
        Assert.Equal(message.PolicyType, message2.PolicyType);
        Assert.Equal(message.FieldIndex, message2.FieldIndex);
        Assert.Equal(message.ValuesList, message2.ValuesList);
        Assert.Equal(message.NewValuesList, message2.NewValuesList);
    }

    [Fact]
    public void TestAddPolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.AddPolicy("alice", "data1", "read");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.AddPolicy("alice", "book1", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateAddPolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public void TestRemovePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.RemovePolicy("alice", "data1", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.RemovePolicy("alice", "data1", "read");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemovePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public void TestUpdatePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.UpdatePolicy(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.WatcherMessage is null);

        enforcer.UpdatePolicy(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicy("p", "p",
                Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public void TestSavePolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);
        enforcer.SavePolicy();
        MessageEquals(sampleWatcher.WatcherMessage, PolicyChangedMessage.CreateSavePolicy());
    }

    [Fact]
    public void TestAddPolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.AddPolicies(new[] { ["data2_admin", "data2", "read"], new[] { "frank", "book4", "read" } });
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["cindy", "book5", "write"], new[] { "frank", "book4", "read" }
        };
        enforcer.AddPolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateAddPolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestRemovePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["data200_admin", "data2", "read"], new[] { "alice", "book3", "read" }
        };
        enforcer.RemovePolicies(rules);
        Assert.True(sampleWatcher.WatcherMessage is null);

        rules = new[] { ["data2_admin", "data2", "read"], new[] { "alice", "book3", "read" } };
        enforcer.RemovePolicies(rules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemovePolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public void TestUpdatePolicies()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.UpdatePolicies(new[] { ["data2_admin", "data2", "read"], new[] { "frank", "book4", "read" } },
            new[] { ["data3_admin", "data200", "read"], new[] { "frank", "book6", "read" } });
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> oldRules = new[]
        {
            ["data2_admin", "data2", "read"], new[] { "alice", "data1", "read" }
        };
        IEnumerable<IEnumerable<string>> newRules = new[]
        {
            ["data4_admin", "data2", "read"], new[] { "alice", "data3", "read" }
        };
        enforcer.UpdatePolicies(oldRules, newRules);
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicies("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public void TestRemoveFilteredPolicy()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        enforcer.RemoveFilteredPolicy(1, "data3");
        Assert.True(sampleWatcher.WatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["data2_admin", "data2", "read"], new[] { "data2_admin", "data2", "write" }
        };
        enforcer.RemoveFilteredPolicy(0, "data2_admin");
        MessageEquals(sampleWatcher.WatcherMessage,
            PolicyChangedMessage.CreateRemoveFilteredPolicy("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestAddPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.AddPolicyAsync("alice", "data1", "read");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.AddPolicyAsync("alice", "book1", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateAddPolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "book1", "write" })));
    }

    [Fact]
    public async Task TestRemovePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.RemovePolicyAsync("alice", "data1", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.RemovePolicyAsync("alice", "data1", "read");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemovePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" })));
    }

    [Fact]
    public async Task TestUpdatePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "book1", "read" }, "alice", "book2", "write");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        await enforcer.UpdatePolicyAsync(new[] { "alice", "data1", "read" }, "alice", "book2", "write");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicy("p", "p", Policy.ValuesFrom(new[] { "alice", "data1", "read" }),
                Policy.ValuesFrom(new[] { "alice", "book2", "write" })));
    }

    [Fact]
    public async Task TestSavePolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.SavePolicyAsync();
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateSavePolicy());
    }

    [Fact]
    public async Task TestAddPoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.AddPoliciesAsync(new[]
        {
            ["data2_admin", "data2", "read"], new[] { "frank", "book4", "read" }
        });
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["cindy", "book5", "write"], new[] { "frank", "book4", "read" }
        };
        await enforcer.AddPoliciesAsync(rules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateAddPolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestRemovePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["data200_admin", "data2", "read"], new[] { "alice", "book3", "read" }
        };
        await enforcer.RemovePoliciesAsync(rules);
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        rules = new[] { ["data2_admin", "data2", "read"], new[] { "alice", "book3", "read" } };
        await enforcer.RemovePoliciesAsync(rules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemovePolicies("p", "p", Policy.ValuesListFrom(rules)));
    }

    [Fact]
    public async Task TestUpdatePoliciesAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.UpdatePoliciesAsync(
            new[] { ["data2_admin", "data2", "read"], new[] { "frank", "book4", "read" } },
            new[] { ["data3_admin", "data200", "read"], new[] { "frank", "book6", "read" } });
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> oldRules = new[]
        {
            ["data2_admin", "data2", "read"], new[] { "alice", "data1", "read" }
        };
        IEnumerable<IEnumerable<string>> newRules = new[]
        {
            ["data4_admin", "data2", "read"], new[] { "alice", "data3", "read" }
        };
        await enforcer.UpdatePoliciesAsync(oldRules, newRules);
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateUpdatePolicies("p", "p", Policy.ValuesListFrom(oldRules),
                Policy.ValuesListFrom(newRules)));
    }

    [Fact]
    public async Task TestRemoveFilteredPolicyAsync()
    {
        SampleWatcher sampleWatcher = new();

        Enforcer enforcer = new(TestModelFixture.GetNewRbacTestModel(),
            new FileAdapter(TestModelFixture.GetTestFile("rbac_policy_for_watcher_test.csv")));

        enforcer.SetWatcher(sampleWatcher);

        await enforcer.RemoveFilteredPolicyAsync(1, "data3");
        Assert.True(sampleWatcher.AsyncWatcherMessage is null);

        IEnumerable<IEnumerable<string>> rules = new[]
        {
            ["data2_admin", "data2", "read"], new[] { "data2_admin", "data2", "write" }
        };
        await enforcer.RemoveFilteredPolicyAsync(0, "data2_admin");
        MessageEquals(sampleWatcher.AsyncWatcherMessage,
            PolicyChangedMessage.CreateRemoveFilteredPolicy("p", "p", 0, Policy.ValuesListFrom(rules)));
    }

    private class SampleWatcher : IWatcher
    {
        private Func<Task> _asyncCallback;
        private Action _callback;

        public IPolicyChangeMessage WatcherMessage { get; private set; }

        public IPolicyChangeMessage AsyncWatcherMessage { get; private set; }

        public void SetUpdateCallback(Action callback) => _callback = callback;

        public void SetUpdateCallback(Func<Task> callback) => _asyncCallback = callback;

        public void Update() => _callback?.Invoke();

        public async Task UpdateAsync()
        {
            if (_asyncCallback is not null)
            {
                await _asyncCallback.Invoke();
            }
        }

        public void SetUpdateCallback(Action<IPolicyChangeMessage> callback) => throw new NotImplementedException();

        public void SetUpdateCallback(Func<IPolicyChangeMessage, Task> callback) => throw new NotImplementedException();

        public void Update(IPolicyChangeMessage watcherMessage)
        {
            _callback?.Invoke();
            WatcherMessage = watcherMessage;
        }

        public async Task UpdateAsync(IPolicyChangeMessage watcherMessage)
        {
            if (_asyncCallback is not null)
            {
                await _asyncCallback.Invoke();
            }

            AsyncWatcherMessage = watcherMessage;
        }

        public void Close() => _callback = null;

        public Task CloseAsync()
        {
            _asyncCallback = null;
#if !NET452
            return Task.CompletedTask;
#else
            return Task.FromResult(true);
#endif
        }
    }
}
