using Avalab.Serilog.Sanitizer.Rules;
using Serilog;
using Serilog.Configuration;
using System;

namespace Avalab.Serilog.Sanitizer
{
    public static class RegexHiddenConfigurationExtensions
    {
        /// <summary>
        /// Replace content by Regex expression
        /// </summary>
        /// <param name="loggerSinkConfiguration"></param>
        /// <param name="regularExpression">The expression for find CVV</param>
        /// <param name="replaceString">String for replacement</param>
        /// <returns></returns>
        public static LoggerConfiguration RegexHidden(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string regularExpression,
            string replaceExpression,
            string replaceString = "*")
        {
            if (string.IsNullOrEmpty(regularExpression))
                throw new ArgumentNullException(nameof(regularExpression));

            if (string.IsNullOrEmpty(regularExpression))
                throw new ArgumentNullException(nameof(replaceExpression));

            if (loggerSinkConfiguration == null)
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return loggerSinkConfiguration.Sink(
                new RegexHiddenSanitizingRule(regularExpression, replaceExpression, replaceString));
        }
    }
}
