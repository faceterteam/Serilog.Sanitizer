using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    public interface ISanitizingProcessor
    {
        string Process(string content, IEnumerable<ISanitizingFormatRule> rules);
    }
}
