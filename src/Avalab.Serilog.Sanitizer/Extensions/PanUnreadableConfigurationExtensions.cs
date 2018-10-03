using Avalab.Serilog.Sanitizer.Rules;
using Serilog;
using Serilog.Configuration;
using System;

namespace Avalab.Serilog.Sanitizer
{
    public static class PanUnreadableConfigurationExtensions
    {
        /// <summary>
        /// Replace PAN numbers
        /// </summary>
        /// <param name="loggerSinkConfiguration"></param>
        /// <param name="regularExpression">The expression for find PAN</param>
        /// <param name="replaceString">String for replacement</param>
        /// <param name="startSkipCount">By Start skip count numbers</param>
        /// <param name="endSkipCount">By End skip count numbers</param>
        /// <returns></returns>
        public static LoggerConfiguration PanUnreadable(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string regularExpression = "[3456]\\d{3}[- ]?\\d{4}[- ]?\\d{4}[- ]?\\d{4}(?:[- ]?\\d{2})?",
            string replaceString = "*",
            uint startSkipCount = 6,
            uint endSkipCount = 4)
        {
            if (loggerSinkConfiguration == null)
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return loggerSinkConfiguration.Sink(
                new PanUnreadableSanitizingRule(
                    regularExpression, 
                    replaceString, 
                    startSkipCount, 
                    endSkipCount));
        }
    }
}
