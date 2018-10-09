using System;
using System.Linq;
using System.Collections.Generic;

using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Avalab.Serilog.Sanitizer
{
    sealed class SanitizerSink : ILogEventSink
    {
        private readonly ILogEventSink _sink;
        private readonly ISanitizingProcessor _processor;
        private readonly bool _sanitizeException;

        public SanitizerSink(
            IEnumerable<AbstractSanitizingRule> rules, 
            ILogEventSink sink,
            bool sanitizeException)
        {
            _processor = new DefaultProcessor(rules);
            _sink = sink ?? throw new ArgumentNullException(nameof(sink));
            _sanitizeException = sanitizeException;
        }

        public void Emit(LogEvent logEvent)
        {
            MessageTemplateParser parser = new MessageTemplateParser();

            var nle = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                _sanitizeException ? SanitizeException(logEvent.Exception) : logEvent.Exception,
                parser.Parse(_processor.Sanitize((logEvent.MessageTemplate.Text))),
                logEvent.Properties.Select(MapProperties));

            _sink.Emit(nle);
        }

        private LogEventProperty MapProperties(KeyValuePair<string, LogEventPropertyValue> tuple)
        {
            switch (tuple.Value)
            {
                case ScalarValue scalarValue:
                    return new LogEventProperty(tuple.Key, MapScalar(tuple.Key, scalarValue));
                case StructureValue structureValue:
                    return new LogEventProperty(tuple.Key, MapStructure(structureValue));
                default:
                    throw new InvalidOperationException($"Invalid type `{tuple.Value.GetType()}`");
            }
        }

        private ScalarValue MapScalar(string key, ScalarValue value) => new ScalarValue(_processor.Sanitize(value.Value.ToString(), key));

        private StructureValue MapStructure(StructureValue value) => new StructureValue(
                value.Properties.Select(
                    property => MapProperties(
                    new KeyValuePair<string, LogEventPropertyValue>(property.Name, property.Value))));

        private Exception SanitizeException(Exception ex)
        {
            if (ex == null)
                return null;

            Exception inner = null;
            if (ex.InnerException != null)
                inner = SanitizeException(ex.InnerException);

            return new Exception(_processor.Sanitize(ex.Message), inner);
        }
    }
}
