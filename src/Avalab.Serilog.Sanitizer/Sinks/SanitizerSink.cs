using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

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
            if (sink == null)
                throw new ArgumentNullException(nameof(sink));

            _processor = new DefaultProcessor(rules);
            _sink = sink;
            _sanitizeException = sanitizeException;
        }

        public void Emit(LogEvent logEvent)
        {
            MessageTemplateParser parser = new MessageTemplateParser();

            var nle = new LogEvent(
                logEvent.Timestamp,
                logEvent.Level,
                _sanitizeException ? SanitizeException(logEvent.Exception) :logEvent.Exception,
                parser.Parse(_processor.Sanitize((logEvent.MessageTemplate.Text))),
                logEvent.Properties.Select(MapProperties));

            _sink.Emit(nle);
        }

        private LogEventProperty MapProperties(KeyValuePair<string, LogEventPropertyValue> tuple)
        {
            switch (tuple.Value)
            {
                case ScalarValue _:
                    return new LogEventProperty(tuple.Key, MapScalar(tuple.Key, tuple.Value as ScalarValue));
                case StructureValue _:
                    return new LogEventProperty(tuple.Key, MapStructure(tuple.Value as StructureValue));
                default:
                    throw new InvalidEnumArgumentException($"type `{tuple.Value.GetType()}` not switched");
            }
        }

        private ScalarValue MapScalar(string key, ScalarValue value)
        {
            return new ScalarValue(_processor.Sanitize(value.Value.ToString(), key));
        }

        private StructureValue MapStructure(StructureValue value)
        {
            List<LogEventProperty> props = new List<LogEventProperty>();
            foreach(var v in value.Properties)
            {
                if (v.Value is ScalarValue)
                    props.Add(new LogEventProperty(v.Name, MapScalar(v.Name, v.Value as ScalarValue)));
                if (v.Value is StructureValue)
                    props.Add(new LogEventProperty(v.Name, MapStructure(v.Value as StructureValue)));
            }
            return new StructureValue(props);
        }

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
