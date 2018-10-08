using System.Linq;
using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer
{
    class DefaultProcessor : ISanitizingProcessor
    {
        private readonly IEnumerable<AbstractSanitizingRule> _rules;

        public DefaultProcessor(IEnumerable<AbstractSanitizingRule> rules)
        {
            _rules = rules;
        }

        public string Sanitize(string content)
        {
            return _rules.Aggregate(content, (ct, rule) => rule.Sanitize(ct));
        }

        public string Sanitize(string content, string key)
        {
            return _rules.Aggregate(content, (ct, rule) =>
                rule.Sanitize($"{key}: {ct}").Substring($"{key}: ".Length));
        }
    }
}
