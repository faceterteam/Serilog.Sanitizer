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

        public override bool IsMatch(string matchedContent)
        {
            return _regex.IsMatch(matchedContent);
        }

        public override string Sanitize(string content)
        {
           return Regex.Replace(content, _replaceExpression, _ => _replaceString);
        }
    }
}
