using System;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitize.FormatRules
{
    public class CvvHiddenSanitizingFormatRule : ISanitizingFormatRule
    {
        private readonly Regex _regex;
        private readonly string _replaceText;

        public CvvHiddenSanitizingFormatRule(string regularExperession, char replaceChar = '*')
        {
            if (string.IsNullOrEmpty(regularExperession))
                throw new ArgumentNullException(nameof(regularExperession));

            _regex = new Regex(regularExperession);
            _replaceText = replaceChar.ToString();
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
