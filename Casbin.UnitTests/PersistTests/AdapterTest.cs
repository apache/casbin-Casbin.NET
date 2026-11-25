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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Casbin.Model;
using Casbin.Persist;
using Casbin.Persist.Adapter.File;
using Casbin.Persist.Adapter.Stream;
using Casbin.Persist.Adapter.Text;
using Xunit;

namespace Casbin.UnitTests.PersistTests;

public class AdapterTest
{
    [Fact]
    public void TestFileAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFileAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredFileAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredFileAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        FileAdapter a = new("Examples/basic_policy.csv");
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestStreamAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        using FileStream stream = File.OpenRead("Examples/basic_policy.csv");
        StreamAdapter a = new(stream);
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestStreamAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        using FileStream stream = File.OpenRead("Examples/basic_policy.csv");
        StreamAdapter a = new(stream);
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredStreamAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        using FileStream stream = File.OpenRead("Examples/basic_policy.csv");
        StreamAdapter a = new(stream);
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredStreamAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

        using FileStream stream = File.OpenRead("Examples/basic_policy.csv");
        StreamAdapter a = new(stream);
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestTextAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadPolicy();

        Assert.True(e.Enforce("alice", "data1", "read"));
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestTextAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

#if NET452 || NET461 || NET462
        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
#else
        TextAdapter a = new(await File.ReadAllTextAsync("Examples/basic_policy.csv"));
#endif
        e.SetAdapter(a);
        await e.LoadPolicyAsync();

        Assert.True(await e.EnforceAsync("alice", "data1", "read"));
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFilteredTextAdapter()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(e.Enforce("alice", "data1", "read"));

        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
        e.SetAdapter(a);
        e.LoadFilteredPolicy(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(e.Enforce("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(e.Enforce("alice", "data1", "write"));
        Assert.False(e.Enforce("alice", "data2", "read"));
        Assert.False(e.Enforce("alice", "data2", "write"));
        Assert.False(e.Enforce("bob", "data1", "read"));
        Assert.False(e.Enforce("bob", "data1", "write"));
        Assert.False(e.Enforce("bob", "data2", "read"));
        Assert.True(e.Enforce("bob", "data2", "write"));
    }

    [Fact]
    public async Task TestFilteredTextAdapterAsync()
    {
        Enforcer e = new("Examples/basic_model.conf");
        Assert.False(await e.EnforceAsync("alice", "data1", "read"));

#if NET452 || NET461 || NET462
        TextAdapter a = new(File.ReadAllText("Examples/basic_policy.csv"));
#else
        TextAdapter a = new(await File.ReadAllTextAsync("Examples/basic_policy.csv"));
#endif
        e.SetAdapter(a);
        await e.LoadFilteredPolicyAsync(new PolicyFilter(PermConstants.DefaultPolicyType, 0, Policy.ValuesFrom(["bob"])));

        Assert.False(await e.EnforceAsync("alice", "data1", "read")); // because "alice" is not in the filtered policy
        Assert.False(await e.EnforceAsync("alice", "data1", "write"));
        Assert.False(await e.EnforceAsync("alice", "data2", "read"));
        Assert.False(await e.EnforceAsync("alice", "data2", "write"));
        Assert.False(await e.EnforceAsync("bob", "data1", "read"));
        Assert.False(await e.EnforceAsync("bob", "data1", "write"));
        Assert.False(await e.EnforceAsync("bob", "data2", "read"));
        Assert.True(await e.EnforceAsync("bob", "data2", "write"));
    }

    [Fact]
    public void TestFileAdapterSavesFieldsWithCommasAndQuotes()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "p, alice, data1, read");
            Enforcer e = new("Examples/basic_model.conf", new FileAdapter(path));
            e.AddPolicy("bob", "data2, data3", "read");
            e.AddPolicy("carol", "\"data4\"", "write");
            e.SavePolicy();

            Enforcer e2 = new("Examples/basic_model.conf", new FileAdapter(path));
            Assert.True(e2.Enforce("alice", "data1", "read"));
            Assert.True(e2.Enforce("bob", "data2, data3", "read"));
            Assert.True(e2.Enforce("carol", "\"data4\"", "write"));
            Assert.Equal(3, e2.GetPolicy().Count());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TestFileAdapterRejectsFieldsWithLineBreaks()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "p, alice, data1, read");
            Enforcer e = new("Examples/basic_model.conf", new FileAdapter(path));
            e.AddPolicy("bob", "data2\np, eve, data1, read", "read");

            Assert.Throws<ArgumentException>(() => e.SavePolicy());
            Assert.Equal("p, alice, data1, read", File.ReadAllText(path));

            Enforcer e2 = new("Examples/basic_model.conf", new FileAdapter(path));
            Assert.False(e2.Enforce("eve", "data1", "read"));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
