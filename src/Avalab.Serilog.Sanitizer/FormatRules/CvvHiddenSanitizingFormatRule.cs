using System;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitizer.FormatRules
{
    public class CvvHiddenSanitizingFormatRule : ISanitizingFormatRule
    {
        private readonly Regex _regex;
        private readonly string _replaceText;

        public CvvHiddenSanitizingFormatRule(string regularExpression, string replaceChar = "*")
        {
            if (string.IsNullOrEmpty(regularExpression))
                throw new ArgumentNullException(nameof(regularExpression));

            _regex = new Regex(regularExpression);
            _replaceText = replaceChar;
        }

        public string Sanitize(string content)
        {
            return _regex.Replace(content, match =>
            {
                string v = match.Value;

                return Regex.Replace(v, @"\d", match2 =>
                {
                    return _replaceText;
                });
            });
        }
    }
}
