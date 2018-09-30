using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Avalab.Serilog.Sanitizer.Sinks
{
    public class SanitizeSink : ILogEventSink
    {
        private readonly ILogEventSink _sink;
        private readonly ISanitizingProcessor _processor;
        private readonly IEnumerable<ISanitizingFormatRule> _rules;

        public SanitizeSink(IEnumerable<ISanitizingFormatRule> rules, ILogEventSink sink)
        {
            _processor = new DefaultProcessor();
            _rules = rules;
            _sink = sink;
        }

        public void Emit(LogEvent logEvent)
        {
            MessageTemplateParser parser = new MessageTemplateParser();
            var messageTemplate = parser.Parse(_processor.Process(logEvent.MessageTemplate.Text, _rules));

            var nle = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                logEvent.Exception,
                messageTemplate,
                // logEvent.MessageTemplate.Tokens),
                new List<LogEventProperty>());

            //foreach(var sink in _sinks)
                _sink.Emit(nle);
        }

    }
}
