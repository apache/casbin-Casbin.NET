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
using Casbin.Model;

namespace Casbin.Persist;

[Obsolete("Please use PolicyFilter instead")]
public class Filter : IPolicyFilter
{
    private PolicyFilter _filterG;


    private PolicyFilter _filterP;
    private IEnumerable<string> _g;
    private IEnumerable<string> _p;

    public IEnumerable<string> P
    {
        get => _p;
        set
        {
            _p = value;
            _filterP = new PolicyFilter(PermConstants.DefaultPolicyType, 0,
                Policy.ValuesFrom(value));
        }
    }

    public IEnumerable<string> G
    {
        get => _g;
        set
        {
            _g = value;
            _filterG = new PolicyFilter(PermConstants.DefaultGroupingPolicyType, 0,
                Policy.ValuesFrom(value));
        }
    }

    public IQueryable<T> Apply<T>(IQueryable<T> policies) where T : IPersistPolicy
    {
        if (_filterP is null && _filterG is null)
        {
            return policies;
        }

        if (_filterP is not null && _filterG is not null)
        {
            IQueryable<T> policiesP = _filterP.Apply(policies);
            IQueryable<T> policiesG = _filterG.Apply(policies);
            return policiesP.Union(policiesG);
        }

        if (_filterP is not null)
        {
            return _filterP.Apply(policies);
        }

        if (_filterG is not null)
        {
            return _filterG.Apply(policies);
        }

        return policies;
    }
}






