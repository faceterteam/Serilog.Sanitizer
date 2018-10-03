using Avalab.Serilog.Sanitizer;
using Serilog.Configuration;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serilog
{
    public static class SanitizerConfigurationExtensions
    {
        /// <summary>
        /// Decorate-sink for sanitizing ILogEvent
        /// </summary>
        /// <param name="loggerSinkConfiguration"></param>
        /// <param name="rules">Collection sanitize rules like a Sinks, but casting to AbstractSanitizingRule</param>
        /// <param name="sinks">Collection wrapper target Sinks</param>
        /// <param name="sanitizeException">Need or not processing logEvent.Exception (default: true)</param>
        /// <returns></returns>
        public static LoggerConfiguration Sanitizer(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Action<LoggerSinkConfiguration> rules,
            Action<LoggerSinkConfiguration> sinks,
            bool sanitizeException = true)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            List<AbstractSanitizingRule> sanitizeRules = new List<AbstractSanitizingRule>();
            LoggerSinkConfiguration.Wrap(
                loggerSinkConfiguration,
                wrappedRule => {
                    if (wrappedRule is AbstractSanitizingRule) // serilog 2.6.0
                        sanitizeRules.Add((AbstractSanitizingRule)wrappedRule);
                    else // serilog 2.7.1
                    {
                        var fieldsInfo = wrappedRule
                        .GetType()
                        .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                        .FirstOrDefault(field => typeof(ILogEventSink[]).IsAssignableFrom(field.FieldType)
                            && field.Name == "_sinks");

                        if (fieldsInfo == null)
                            return null;

                        var unwrappedSinks = (ILogEventSink[])fieldsInfo.GetValue(wrappedRule);

                        foreach (var sink in unwrappedSinks)
                        {
                            if (sink is AbstractSanitizingRule)
                                sanitizeRules.Add((AbstractSanitizingRule)sink);
                        }
                    }

                    return default(ILogEventSink);
                },
                rules);

            return LoggerSinkConfiguration.Wrap(
                loggerSinkConfiguration,
                wrappedSink => new SanitizeSink(sanitizeRules, wrappedSink, sanitizeException),
                sinks);
        }
    }
}
