using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitizer
{
    public abstract class AbstractSanitizingRule : ILogEventSink
    {
        public abstract string Sanitize(string content);

        public abstract bool IsMatch(string matchedContent);

        public void Emit(LogEvent logEvent) { }
    }
}
