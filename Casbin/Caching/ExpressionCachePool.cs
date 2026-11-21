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
using System.Collections.Concurrent;
using System.Threading;

namespace Casbin.Caching;

public class ExpressionCachePool : IExpressionCachePool
{
    private ConcurrentDictionary<Type, IExpressionCache> _cachePool = new();

    public void SetFunc<TFunc>(string expression, TFunc func) where TFunc : Delegate
    {
        Type type = typeof(TFunc);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache<TFunc>();
            _cachePool[type] = cache;
        }

        var cacheImpl = (IExpressionCache<TFunc>)cache;
        cacheImpl.Set(expression, func);
    }

    public bool TryGetFunc<TFunc>(string expression, out TFunc func) where TFunc : Delegate
    {
        Type type = typeof(TFunc);
        if (_cachePool.TryGetValue(type, out IExpressionCache cache) is false)
        {
            cache = new ExpressionCache<TFunc>();
            _cachePool[type] = cache;
        }

        var cacheImpl = (IExpressionCache<TFunc>)cache;
        return cacheImpl.TryGet(expression, out func);
    }

    public void Clear()
    {
        ConcurrentDictionary<Type, IExpressionCache> cachePool = new ConcurrentDictionary<Type, IExpressionCache>();
        Interlocked.Exchange(ref _cachePool, cachePool);
    }
}
