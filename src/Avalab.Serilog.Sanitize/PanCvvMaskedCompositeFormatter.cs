using Avalab.Serilog.Sanitize.FormatRules;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Avalab.Serilog.Sanitize
{
    public class PanCvvMaskedCompositeFormatter : ITextFormatter
    {
        const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{TraceIdentifier}] {Message}{NewLine}{Exception}";

        private readonly ITextFormatter _formatter;

        public PanCvvMaskedCompositeFormatter(string outputTemplate = DefaultOutputTemplate)
        {
            var customRules = new List<ISanitizingFormatRule>
            {
                new PanUnreadableSanitizingFormatRule(@"[3456]\d{3}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}(?:[- ]?\d{2})?"),
                new CvvHiddenSanitizingFormatRule("cvv\":[ ]?\"?\\d{3}\"?")
            };

            _formatter = new LogSanitizingFormatter(customRules, new MessageTemplateTextFormatter(outputTemplate, null));
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            _formatter.Format(logEvent, output);
        }
    }
}
