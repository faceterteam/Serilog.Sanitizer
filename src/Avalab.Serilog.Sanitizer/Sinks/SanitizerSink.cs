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
                    return new LogEventProperty(tuple.Key, MapValue(scalarValue, tuple.Key));
                case StructureValue structureValue:
                case SequenceValue sequenceValue:
                case DictionaryValue dictionaryValue:
                default:
                    return new LogEventProperty(tuple.Key, MapValue(tuple.Value));
            }
        }

        private LogEventPropertyValue MapValue(LogEventPropertyValue value, string key = "")
        {
            switch (value)
            {
                case ScalarValue scalarValue:
                    return new ScalarValue(_processor.Sanitize(value.ToString(), key));
                case StructureValue structureValue:
                    return MapStructure(structureValue);
                case SequenceValue sequenceValue:
                    return MapSequence(sequenceValue);
                case DictionaryValue dictionaryValue:
                    return MapDictionary(dictionaryValue);
                default:
                    return new ScalarValue(
                        new InvalidOperationException($"Invalid type `{value.GetType()}`"));
            }
        }

        private DictionaryValue MapDictionary(DictionaryValue dictionaryValue) =>
            new DictionaryValue(
                dictionaryValue.Elements.Select(
                    property =>
                       new KeyValuePair<ScalarValue, LogEventPropertyValue>(property.Key, MapValue(property.Value))
                    ));

        private SequenceValue MapSequence(SequenceValue sequenceValue) =>
            new SequenceValue(
                sequenceValue.Elements.Select(
                    property => 
                        MapValue(property)
                    ));

        private ScalarValue MapScalar(string key, ScalarValue value)
        {
            if (value?.Value == null)
                return new ScalarValue(value);

            return new ScalarValue(_processor.Sanitize(value.Value.ToString(), key));
        }

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
