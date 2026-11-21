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

using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.Persist;

public interface IPolicyChangeMessage
{
    public PolicyOperation Operation { get; }
    public string Section { get; }
    public string PolicyType { get; }
    public int FieldIndex { get; }
    public IPolicyValues Values { get; }
    public IPolicyValues NewValues { get; }
    public IReadOnlyList<IPolicyValues> ValuesList { get; }
    public IReadOnlyList<IPolicyValues> NewValuesList { get; }
}
