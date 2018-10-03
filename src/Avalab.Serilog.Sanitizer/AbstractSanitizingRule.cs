using Serilog.Core;
using Serilog.Events;

namespace Avalab.Serilog.Sanitizer
{
    public abstract class AbstractSanitizingRule : ILogEventSink
    {
        public abstract string Sanitize(string content);

        public void Emit(LogEvent logEvent) { }
    }
}
