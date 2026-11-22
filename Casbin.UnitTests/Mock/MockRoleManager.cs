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
using Casbin.Rbac;

namespace Casbin.UnitTests.Mock;

public class MockCustomRoleManager : IRoleManager
{
    public Func<string, string, bool> MatchingFunc { get; set; }
    public Func<string, string, bool> DomainMatchingFunc { get; set; }
    public bool HasPattern => false;
    public bool HasDomainPattern => false;

    public IEnumerable<string> GetRoles(string name, string domain = null) => null;
    public IEnumerable<string> GetUsers(string name, string domain = null) => null;
    public IEnumerable<string> GetDomains(string name) => [];

    public bool HasLink(string name1, string name2, string domain = null)
    {
        if (name1.Equals("alice") && name2.Equals("alice"))
        {
            return true;
        }

        if (name1.Equals("alice") && name2.Equals("data2_admin"))
        {
            return true;
        }

        if (name1.Equals("bob") && name2.Equals("bob"))
        {
            return true;
        }

        return false;
    }

    public void AddLink(string name1, string name2, string domain = null)
    {
    }

    public void DeleteLink(string name1, string name2, string domain = null)
    {
    }

    public void Clear()
    {
    }
}
