using Serilog;
using Serilog.Configuration;
using System;

namespace Avalab.Serilog.Sanitizer.Tests.Rules
{
    public static class TruncateRuleConfigurationExtensions
    {
        public static LoggerConfiguration Truncate(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            int maxLength = 252,
            string replaceString = "...")
        {
            if (loggerSinkConfiguration == null)
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return loggerSinkConfiguration.Sink(
                new TruncateRule(maxLength, replaceString));
        }
    }
}
