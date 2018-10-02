using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    public class SanitizerSinkOptions
    {
        public IReadOnlyDictionary<string, string> Formatters { get; set; }
    }
}