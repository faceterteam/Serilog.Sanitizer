using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    public class SanitizerSinkOptions
    {
        public IReadOnlyCollection<FormatterMetaInfo> Formatters { get; set; }
    }
}