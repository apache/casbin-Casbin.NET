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

using Casbin.Persist;

namespace Casbin.Model.Holder;

public class WatcherHolder
{
    private IReadOnlyWatcher _watcher;

    public IReadOnlyWatcher Watcher
    {
        get => _watcher;
        set
        {
            _watcher = value;
            DetermineWatcher(value);
        }
    }

    public IFullWatcher FullWatcher { get; private set; }
    public IIncrementalWatcher IncrementalWatcher { get; private set; }
    public IWatcherEx WatcherEx { get; private set; }

    private void DetermineWatcher(IReadOnlyWatcher watcher)
    {
        FullWatcher = watcher as IFullWatcher;
        IncrementalWatcher = watcher as IIncrementalWatcher;
        WatcherEx = watcher as IWatcherEx;
    }
}

