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

using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.Caching
{
    public interface IEnforceCache
    {
        public bool TryGetResult<TRequest>(in TRequest requestValues, out bool result)
            where TRequest : IRequestValues;

        public Task<bool?> TryGetResultAsync<TRequest>(in TRequest requestValues)
            where TRequest : IRequestValues;

        public bool TrySetResult<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues;

        public Task<bool> TrySetResultAsync<TRequest>(in TRequest requestValues, bool result)
            where TRequest : IRequestValues;

        public void Clear();

        public Task ClearAsync();
    }
}
