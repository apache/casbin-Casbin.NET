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
using Casbin.Effect;

namespace Casbin;

public ref struct EnforceSession
{
    internal string ExpressionString { get; set; }
    internal int PolicyIndex { get; set; }

    internal bool HasNextPolicy { get; set; }

    internal PolicyEffect PolicyEffect { get; set; }
    internal List<PolicyEffect> PolicyEffects { get; set; }

    internal bool Determined { get; private set; }
    internal bool EnforceResult { get; set; }

    internal bool ExpressionResult { get; set; }
    internal bool IsChainEffector { get; set; }

    internal int? Priority { get; set; }

    internal void DetermineResult(bool result)
    {
        Determined = true;
        EnforceResult = result;
    }
}
