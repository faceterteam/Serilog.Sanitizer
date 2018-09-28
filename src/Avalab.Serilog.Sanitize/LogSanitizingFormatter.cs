using Serilog.Events;
using Serilog.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Avalab.Serilog.Sanitize
{
    public class LogSanitizingFormatter : ITextFormatter
    {
        private readonly ISanitizingProcessor _processor;
        private readonly IEnumerable<ISanitizingFormatRule> _sanitizingFormatRules;
        private readonly ITextFormatter _formatter;

        public LogSanitizingFormatter(
            ISanitizingProcessor processor, 
            IEnumerable<ISanitizingFormatRule> sanitizingFormatRules, 
            ITextFormatter jsonFormatter)
        {
            _processor = processor;
            _sanitizingFormatRules = sanitizingFormatRules;
            _formatter = jsonFormatter;
        }

        /// <summary>
        /// Use default processor with your own rules
        /// </summary>
        /// <param name="rawFormatter">Json Formatter</param>
        /// <param name="sanitizeLogContent">flag to turn sanitising on or off</param>
        public LogSanitizingFormatter(IEnumerable<ISanitizingFormatRule> saninitizerRules, ITextFormatter rawFormatter)
            : this(new DefaultProcessor(), saninitizerRules, rawFormatter)
        {
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
                Sanitize(logEvent, output);
        }

        private void Sanitize(LogEvent logEvent, TextWriter output)
        {
            var tempTextWriter = new StringWriter();

            _formatter.Format(logEvent, tempTextWriter);

            var processedLogEvent = _processor.Process(
                tempTextWriter.GetStringBuilder().ToString(),
                _sanitizingFormatRules);

            output.Write(processedLogEvent);
        }
    }
}
