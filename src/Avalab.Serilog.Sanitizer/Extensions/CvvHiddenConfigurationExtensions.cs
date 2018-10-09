using Avalab.Serilog.Sanitizer.Rules;
using Serilog;
using Serilog.Configuration;
using System;

namespace Avalab.Serilog.Sanitizer
{
    public static class CvvHiddenConfigurationExtensions
    {
        /// <summary>
        /// Replace CVV numbers
        /// </summary>
        /// <param name="loggerSinkConfiguration"></param>
        /// <param name="regularExpression">The expression for find CVV</param>
        /// <param name="replaceString">String for replacement</param>
        /// <returns></returns>
        public static LoggerConfiguration CvvHidden(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string regularExpression = "(?i)cvv\"?[ ]?:[ ]?\"?\\d{3}\"?",
            string replaceString = "*")
        {
            if (loggerSinkConfiguration == null)
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return loggerSinkConfiguration.Sink(
                new RegexHiddenSanitizingRule(regularExpression, @"\d", replaceString));
        }
    }
}
