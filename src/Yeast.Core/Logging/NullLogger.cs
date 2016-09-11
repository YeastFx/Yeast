using Microsoft.Extensions.Logging;
using System;

namespace Yeast.Core.Logging
{
    /// <summary>
    /// Just an implementation of <see cref="ILogger"/> interface to avoid null tests and exceptions.
    /// </summary>
    public class NullLogger : ILogger
    {
        private static readonly ILogger _instance = new NullLogger();

        /// <summary>
        /// Static instance of <see cref="NullLogger"/>
        /// </summary>
        public static ILogger Instance {
            get { return _instance; }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) { }

        private class NullScope : IDisposable
        {
            public void Dispose() { }
        }
    }
}
