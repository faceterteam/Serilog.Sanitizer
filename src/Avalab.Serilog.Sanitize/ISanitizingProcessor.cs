using System;
using System.Collections.Generic;

namespace Avalab.Serilog.Sanitize
{
    public interface ISanitizingProcessor
    {
        string Process(string content, IEnumerable<ISanitizingFormatRule> rules);
    }
}
