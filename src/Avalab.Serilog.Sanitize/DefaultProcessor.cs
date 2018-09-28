using System.Collections.Generic;

namespace Avalab.Serilog.Sanitize
{
    public class DefaultProcessor : ISanitizingProcessor
    {
        public string Process(string content, IEnumerable<ISanitizingFormatRule> rules)
        {
            string sanitizedContent = content;
            foreach (var sanitizingFormatRule in rules)
                sanitizedContent = sanitizingFormatRule.Sanitize(sanitizedContent);

            return sanitizedContent;
        }
    }
}
