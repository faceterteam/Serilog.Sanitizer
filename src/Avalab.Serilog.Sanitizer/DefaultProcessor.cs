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

        public string Process(string content, string matchedContent = null)
        {
            return _rules.Aggregate(content, (ct, rule) => {

                return rule.Sanitize(ct);
            } );
        }
    }
}
