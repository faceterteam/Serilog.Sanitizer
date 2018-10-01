using Avalab.Serilog.Sanitizer.FormatRules;
using Avalab.Serilog.Sanitizer.Sinks;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitizer
{
    public static class SanitizeLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Sanitizer(
           this LoggerSinkConfiguration loggerSinkConfiguration,
            Action<LoggerSinkConfiguration> sinks,
            string panFormat = "[3456]\\d{3}[- ]?\\d{4}[- ]?\\d{4}[- ]?\\d{4}(?:[- ]?\\d{2})?",
            string cvvFormat = "(?i)cvv\"?[ ]?:[ ]?\"?\\d{3}\"?")
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return LoggerSinkConfiguration.Wrap(
                loggerSinkConfiguration,
                wrappedSink => new SanitizeSink(
                new ISanitizingFormatRule[]{
                    new PanUnreadableSanitizingFormatRule(panFormat),
                    new CvvHiddenSanitizingFormatRule(cvvFormat)
                 }
                , wrappedSink),
                sinks);
        }
    }
}
