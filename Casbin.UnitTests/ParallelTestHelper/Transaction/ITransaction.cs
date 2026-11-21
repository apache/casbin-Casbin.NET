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
using System.Collections.Generic;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public interface ITransaction<TRequest> where TRequest : IRequestValues
    {
        /// <summary>
        /// Execute the transaction in async environment.
        /// </summary>
        /// <param name="consumer"></param>
        /// <returns> Whether the result from consumer is correct.</returns>
        Task<bool> ExecuteAsync(IConsumer<TRequest> consumer);
        /// <summary>
        /// Set the truth with sync method of enforcer
        /// </summary>
        /// <param name="enforcer"></param>
        void SetTruth(IEnforcer enforcer);
        /// <summary>
        /// The request values. At least one element is expected.
        /// </summary>
        IEnumerable<TRequest> Request { get; }
        TransactionType TransactionType { get; }
        bool ExpectedResult { get; }
        bool ActualResult { get; }
        bool HasCompleted { get; }
    }
}