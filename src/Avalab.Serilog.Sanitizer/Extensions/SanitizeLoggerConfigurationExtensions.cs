using Serilog;
using Serilog.Configuration;

using Avalab.Serilog.Sanitizer.Sinks;

using System;

namespace Avalab.Serilog.Sanitizer.Extensions
{
    public static class SanitizeLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Sanitizer(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Action<LoggerSinkConfiguration> sinks)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return LoggerSinkConfiguration.Wrap(
                loggerSinkConfiguration,
                wrappedSink => new SanitizeSink(
                SanitizerConfigurationStore.GetFormatters()
                , wrappedSink),
                sinks);
        }
    }
}
