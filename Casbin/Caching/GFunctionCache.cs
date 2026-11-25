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
using System.Threading;
using Casbin.Model;

namespace Casbin.Caching;

public class GFunctionCache : IGFunctionCache
{
    private const int MaxEntries = 100000;
    private readonly ConcurrentDictionary<string, bool> _cache = new();
    private int _count;

    public void Set(string name1, string name2, bool result, string domain = null)
    {
        string key = Key(name1, name2, domain);
        if (_cache.TryAdd(key, result) is false)
        {
            _cache[key] = result;
            return;
        }

        if (Interlocked.Increment(ref _count) > MaxEntries)
        {
            Clear();
        }
    }

    public bool TryGet(string name1, string name2, out bool result, string domain = null)
    {
        return _cache.TryGetValue(Key(name1, name2, domain), out result);
    }

    public void Clear()
    {
        _cache.Clear();
        Interlocked.Exchange(ref _count, 0);
    }

    private static string Key(string name1, string name2, string domain = null)
    {
        return domain is not null
            ? Request.GetCacheKey(name1, name2, domain)
            : Request.GetCacheKey(name1, name2);
    }
}
