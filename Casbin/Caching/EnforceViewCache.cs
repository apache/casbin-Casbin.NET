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
using System.Threading;

namespace Casbin.Caching;

public class EnforceViewCache : IEnforceViewCache
{
    public TimeSpan WaitTime { get; set; } = TimeSpan.FromMilliseconds(50);
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<string, EnforceView> _views = new();

    public bool TryAdd(string name, EnforceView view)
    {
        if (view is null)
        {
            return true;
        }

        if (_lock.TryEnterWriteLock(WaitTime) is false)
        {
            return false;
        }

        try
        {
            _views[name] = view;
            return true;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public bool TryGet(string name, out EnforceView view)
    {
        if (_lock.TryEnterReadLock(WaitTime) is false)
        {
            view = null;
            return false;
        }

        try
        {
            return _views.TryGetValue(name, out view);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}
