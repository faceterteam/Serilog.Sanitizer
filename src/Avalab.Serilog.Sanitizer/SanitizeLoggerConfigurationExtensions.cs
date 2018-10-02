using Serilog;
using Serilog.Configuration;

using Avalab.Serilog.Sanitizer.FormatRules;
using Avalab.Serilog.Sanitizer.Sinks;

using System;

namespace Avalab.Serilog.Sanitizer
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
                new ISanitizingFormatRule[]{
                    new PanUnreadableSanitizingFormatRule(ConfigurationStore.SanitizerSinkOptions?.Formatters["panFormat"]),
                    new CvvHiddenSanitizingFormatRule(ConfigurationStore.SanitizerSinkOptions?.Formatters["cvvFormat"]),
                 }
                , wrappedSink),
                sinks);
        }
    }

    public static class ConfigurationStore
    {
        public static SanitizerSinkOptions SanitizerSinkOptions { get; set; }
    }
}
