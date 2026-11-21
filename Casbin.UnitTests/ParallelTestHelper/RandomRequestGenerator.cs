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
using System.Linq;
using System;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public class RandomRequestGenerator<TRequest> where TRequest : IRequestValues
    {
        public IEntropyPool<TRequest> ExistedEntropyPool;
        public IEntropyPool<TRequest> RandomEntropyPool;
        private List<IEntropyHandler<TRequest>> _handlers = [];
        private List<int> _weightPrefix = [];
        public RandomRequestGenerator(IEntropyPool<TRequest> existedEntropyPool, IEntropyPool<TRequest> randomEntropyPool)
        {
            ExistedEntropyPool = existedEntropyPool;
            RandomEntropyPool = randomEntropyPool;
        }
        public RandomRequestGenerator(IEntropyPool<TRequest> existedEntropyPool, IEntropyPool<TRequest> randomEntropyPool, 
            params IEntropyHandler<TRequest>[] handlers)
        {
            ExistedEntropyPool = existedEntropyPool;
            RandomEntropyPool = randomEntropyPool;
            foreach(var handler in handlers)
            {
                AddHandler(handler);
            }
        }
        /// <summary>
        /// Get a random request.
        /// </summary>
        /// <param name="existedPoolPossibility"> 
        /// The possibility of using pure existed pool. The total weight of using pure existed pool and 
        /// mixed pools as resource is 100.
        /// </param>
        /// <returns></returns>
        public TRequest Next(IEntropyHandler<TRequest> handler = null)
        {
            // Take the two pools and mix them as resource
            var requestFromExistedPool = ExistedEntropyPool.Get();
            var requestFromRandomPool = RandomEntropyPool.Get();
            if(handler != null)
            {
                return handler.Handle(requestFromExistedPool, requestFromRandomPool);
            }
            Random rd = new Random(DateTime.Now.Millisecond);
            int num = rd.Next(0, _weightPrefix.Last() + 1);
            int n = _handlers.Count;
            int idx = 0;
            if(n < 4)
            {
                // Sequential search when the count of handlers is small
                for (int i = 0; i < n; i++)
                {
                    if(_weightPrefix[i] > num) idx = i;
                }
            }
            else
            {
                // Binary search when the count of handlers is large
                int l = 0, r = n - 1;
                while(l < r)
                {
                    int mid = (l + r) >> 1;
                    if(_weightPrefix[mid] <= num) l = mid + 1;
                    else r = mid;
                }
                idx = l;
            }
            return _handlers[idx].Handle(requestFromExistedPool, requestFromRandomPool);
        }
        public RandomRequestGenerator<TRequest> AddHandler(IEntropyHandler<TRequest> handler)
        {
            _handlers.Add(handler);
            _weightPrefix.Add((_weightPrefix.Count == 0 ? 0 : _weightPrefix[_weightPrefix.Count - 1]) + handler.Weight);
            return this;
        }
    }
}