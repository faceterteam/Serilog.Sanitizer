using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalab.Serilog.Sanitizer
{
    sealed class SanitizeSink : ILogEventSink
    {
        private readonly ILogEventSink _sink;
        private readonly ISanitizingProcessor _processor;
        private readonly bool _sanitizeException;

        public SanitizeSink(
            IEnumerable<AbstractSanitizingRule> rules, 
            ILogEventSink sink,
            bool sanitizeException)
        {
            _processor = new DefaultProcessor(rules);
            _sink = sink;
            _sanitizeException = sanitizeException;
        }

        public void Emit(LogEvent logEvent)
        {
            if (_sink == null)
                return;

            MessageTemplateParser parser = new MessageTemplateParser();

            var nle = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                _sanitizeException ? SanitizeException(logEvent.Exception) :logEvent.Exception,
                parser.Parse(_processor.Process((logEvent.MessageTemplate.Text))),
                logEvent.Properties.Select(t => {
                    if (t.Value is ScalarValue)
                    {
                        return new LogEventProperty(t.Key,
                            new ScalarValue(_processor.Process(t.Value.ToString())));
                    }
                    else
                        return new LogEventProperty(t.Key, t.Value);
                        })
                );

            _sink.Emit(nle);
        }

        private Exception SanitizeException(Exception ex)
        {
            if (ex == null)
                return null;

            Exception inner = null;
            if (ex.InnerException != null)
                inner = SanitizeException(ex.InnerException);

            return new Exception(_processor.Process(ex.Message), inner);
        }
    }
}
