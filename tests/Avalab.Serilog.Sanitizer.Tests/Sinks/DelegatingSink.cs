using System;
using System.IO;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Avalab.Serilog.Sanitizer.Tests.Sinks
{
    class DelegatingSink : ILogEventSink
    {
        private readonly Action<string> _message;
        private readonly ITextFormatter _formatter;

        public DelegatingSink(Action<string> message, string messageTemplate)
        {
            _message = message ?? throw new ArgumentNullException("loggingDelegate");
            _formatter = new MessageTemplateTextFormatter(messageTemplate, null);
        }

        public void Emit(LogEvent logEvent)
        {
            TextWriter writer = new StringWriter();
            _formatter.Format(logEvent, writer);
            _message(writer.ToString());
        }
    }
}
