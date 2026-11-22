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

#if !NET452
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Casbin.UnitTests.Mock
{
    public class MockLogger<T> : ILogger<T>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MockLogger(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

        public List<(LogLevel Level, Exception Exception, string Message)> Logs { get; } = new();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            string outPut = formatter(state, exception);
            _testOutputHelper.WriteLine(outPut);
            Logs.Add((logLevel, exception, outPut));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
#endif
