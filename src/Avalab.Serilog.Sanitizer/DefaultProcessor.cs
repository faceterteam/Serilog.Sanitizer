using System.Linq;
using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    public class DefaultProcessor : ISanitizingProcessor
    {
        public string Process(string content, IEnumerable<ISanitizingFormatRule> rules)
        {
            return rules.Aggregate(content, (ct, rule) => rule.Sanitize(ct));
        }
    }
}
