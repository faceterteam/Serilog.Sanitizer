using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    public sealed class FormatterMetaInfo
    {
        public string Name { get; set; }
        public IReadOnlyDictionary<string, string> Args { get; set;}
    }
}