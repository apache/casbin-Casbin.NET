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

using System.Collections.Concurrent;

namespace Casbin.Caching;

public class GFunctionCache : IGFunctionCache
{
    private readonly ConcurrentDictionary<string, bool> _cache = new();

    public void Set(string name1, string name2, bool result, string domain = null)
    {
        _cache[Key(name1, name2, domain)] = result;
    }

    public bool TryGet(string name1, string name2, out bool result, string domain = null)
    {
        return _cache.TryGetValue(Key(name1, name2, domain), out result);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    private static string Key(string name1, string name2, string domain = null)
    {
        bool hasDomain = domain is not null;
        return hasDomain
            ? string.Join(":", name1, name2, domain)
            : string.Join(":", name1, name2);
    }
}
