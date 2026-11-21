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

public class AdapterHolder
{
    private IReadOnlyAdapter _adapter;

    public IReadOnlyAdapter Adapter
    {
        get => _adapter;
        set
        {
            _adapter = value;
            DetermineAdapter(value);
        }
    }

    public IBatchAdapter BatchAdapter { get; private set; }

    public IEpochAdapter EpochAdapter { get; private set; }

    public IFilteredAdapter FilteredAdapter { get; private set; }

    public ISingleAdapter SingleAdapter { get; private set; }

    private void DetermineAdapter(IReadOnlyAdapter adapter)
    {
        SingleAdapter = adapter as ISingleAdapter;
        BatchAdapter = adapter as IBatchAdapter;
        EpochAdapter = adapter as IEpochAdapter;
        FilteredAdapter = adapter as IFilteredAdapter;
    }
}
