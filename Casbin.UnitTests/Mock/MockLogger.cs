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
