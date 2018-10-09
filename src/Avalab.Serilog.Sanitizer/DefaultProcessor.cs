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

        /// <summary>
        /// For LogEvent.Message, LogEvent.Exception
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Sanitize(string content)
        {
            return _rules.Aggregate(content, (ct, rule) => rule.Sanitize(ct));
        }

        /// <summary>
        /// For LogEvent.Properties
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key">Properties.Value.Key</param>
        /// <returns></returns>
        public string Sanitize(string content, string key)
        {
            return _rules.Aggregate(content, (ct, rule) =>
                rule.Sanitize($"{key}: {ct}").Substring($"{key}: ".Length));
        }
    }
}
