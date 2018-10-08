using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitizer.Rules
{
    sealed class PanUnreadableSanitizingRule : AbstractSanitizingRule
    {
        private readonly Regex _panUnreadableRegex;
        private readonly string _replaceString;
        private readonly uint _startReplaceIndex;
        private readonly uint _endReplaceIndex;

        public PanUnreadableSanitizingRule(
            string regularExpression, 
            string replaceString,
            uint startReplaceIndex,
            uint endReplaceIndex)
        {
            if (string.IsNullOrEmpty(regularExpression))
                throw new ArgumentNullException(nameof(regularExpression));

            _panUnreadableRegex = new Regex(regularExpression);
            _replaceString = replaceString;
            _startReplaceIndex = startReplaceIndex;
            _endReplaceIndex = endReplaceIndex;
        }

        public override bool IsMatch(string matchedContent)
        {
            var match = _panUnreadableRegex.Match(matchedContent);
            if (match.Success == false)
                return false;

            int count;
            if (!Mod10Check(match.Value, out count))
                return false;

            return true;
        }

        public override string Sanitize(string content)
        {
            return _panUnreadableRegex.Replace(content, match =>
            {
                int count;
                if (!Mod10Check(match.Value, out count))
                    return match.Value;

                int i = 0;
                return Regex.Replace(match.Value, @"\d", match2 =>
                {
                    i++;
                    if (i <= _startReplaceIndex || i > count - _endReplaceIndex)
                        return match2.Value;
                    return _replaceString;
                });
            });
        }

        private bool Mod10Check(string creditCardNumber, out int count)
        {
            if (string.IsNullOrEmpty(creditCardNumber))
            {
                count = 0;
                return false;
            }

            var digits = creditCardNumber.Where((e) => e >= '0' && e <= '9').Reverse().ToList();
            count = digits.Count;

            int sumOfDigits = digits
                            .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                            .Sum((e) => e / 10 + e % 10);

            return sumOfDigits % 10 == 0;
        }
    }
}
