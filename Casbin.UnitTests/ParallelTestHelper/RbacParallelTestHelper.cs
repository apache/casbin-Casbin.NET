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
using System.Threading.Tasks;
using Casbin.Model;

namespace Casbin.UnitTests.ParallelTest
{
    public sealed class RbacParallelTestHelper<TRequest> where TRequest : IRequestValues
    {
        private readonly int _maxIOCount = 1;
        private readonly int _maxThreadCount = 6;
        private IConsumer<TRequest> _consumer;
        public Enforcer _referedEnforcer;
        private ITransactionFactory _transactionFactory;
        private List<ITransaction<TRequest>> _transactions = [];

        public RbacParallelTestHelper(IConsumer<TRequest> consumer, Enforcer enforcer,
            ITransactionFactory transactionFactory,
            RandomRequestGenerator<TRequest> randomRequestGenerator)
        {
            _consumer = consumer;
            _referedEnforcer = enforcer;
            _transactionFactory = transactionFactory;
            RandomRequestGenerator = randomRequestGenerator;
        }

        public RandomRequestGenerator<TRequest> RandomRequestGenerator { get; set; }

        public Task<bool> TestCorrectness(int getActionCount, int addActionCount, int updateActionCount,
            int removeActionCount,
            int maxThreadCount = -1, IEnumerable<ITransaction<TRequest>> customizedTransactions = null)
        {
            _transactions.Clear();
            GenerateRandomTransactions(getActionCount, addActionCount, updateActionCount, removeActionCount);
            if (customizedTransactions != null)
            {
                _transactions.AddRange(customizedTransactions);
            }

            ShuffleTransactions();

            foreach (var transaction in _transactions)
            {
                transaction.SetTruth(_referedEnforcer);
            }

            ThreadPool.SetMaxThreads(_maxThreadCount, _maxIOCount);
            foreach (var transaction in _transactions)
            {
                ThreadPool.QueueUserWorkItem((x) => { transaction.ExecuteAsync(_consumer); });
            }

            // Note: this is only a compromise now.
            // We need to find a way to make sure all the tasks are completed
            Thread.Sleep(15000);
            foreach (var transaction in _transactions)
            {
                if (transaction.ExpectedResult != transaction.ActualResult)
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(true);
        }

        private void GenerateRandomTransactions(int getActionCount, int addActionCount, int updateActionCount,
            int removeActionCount)
        {
            while (addActionCount-- > 0)
            {
                var randomRequest = RandomRequestGenerator.Next();
                var transaction = _transactionFactory.CreateTransaction(TransactionType.AddPolicy, randomRequest);
                _transactions.Add(transaction);
                // Add the request to the cache of the pool
                RandomRequestGenerator.ExistedEntropyPool.Add(randomRequest);
            }

            while (getActionCount-- > 0)
            {
                var randomRequest = RandomRequestGenerator.Next();
                var transaction = _transactionFactory.CreateTransaction(TransactionType.GetAccess, randomRequest);
                _transactions.Add(transaction);
            }

            while (updateActionCount-- > 0)
            {
                // TODO
            }

            while (removeActionCount-- > 0)
            {
                var randomRequest = RandomRequestGenerator.Next();
                _transactions.Add(_transactionFactory.CreateTransaction(TransactionType.RemovePolicy, randomRequest));
            }
        }

        private void ShuffleTransactions()
        {
            int n = _transactions.Count;
            Random rd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < n - 1; i++)
            {
                int selectedIdx = rd.Next(i, n);
                (_transactions[i], _transactions[selectedIdx]) = (_transactions[selectedIdx], _transactions[i]);
            }
        }
    }
}
