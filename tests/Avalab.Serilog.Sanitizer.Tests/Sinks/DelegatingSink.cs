using System;

using Serilog.Core;
using Serilog.Events;

namespace Avalab.Serilog.Sanitizer.Tests.Sinks
{
    public class DelegatingSink : ILogEventSink
    {
        private readonly Action<LogEvent> _loggingDelegate;

        public DelegatingSink(Action<LogEvent> loggingDelegate)
        {
            _loggingDelegate = loggingDelegate ?? throw new ArgumentNullException("loggingDelegate");
        }

        public void Emit(LogEvent logEvent)
        {
            _loggingDelegate(logEvent);
        }
    }
}
