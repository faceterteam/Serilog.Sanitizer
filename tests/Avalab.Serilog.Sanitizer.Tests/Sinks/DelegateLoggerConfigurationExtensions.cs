using System;

using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Avalab.Serilog.Sanitizer.Tests.Sinks
{
    public static class DelegateLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Delegate(
           this LoggerSinkConfiguration loggerSinkConfiguration, Action<LogEvent> logEvent)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return loggerSinkConfiguration.Sink(new DelegatingSink(logEvent));
        }
    }
}
