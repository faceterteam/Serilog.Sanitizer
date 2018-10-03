using System;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitizer.Rules
{
    sealed class RegexHiddenSanitizingRule : AbstractSanitizingRule
    {
        private readonly Regex _regex;
        private readonly string _replaceString;
        private readonly string _replaceExpression;

        public RegexHiddenSanitizingRule(string regularExpression, 
            string replaceExpression,
            string replaceString)
        {
            if (string.IsNullOrEmpty(regularExpression))
                throw new ArgumentNullException(nameof(regularExpression));

            _regex = new Regex(regularExpression);
            _replaceString = replaceString;
            _replaceExpression = replaceExpression;
        }

        public override string Sanitize(string content)
        {
            return _regex.Replace(content, match => 
                Regex.Replace(match.Value, _replaceExpression, match2 
                    => _replaceString));
        }
    }
}
