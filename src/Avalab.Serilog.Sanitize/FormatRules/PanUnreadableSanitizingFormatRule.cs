using System.Linq;
using System.Text.RegularExpressions;

namespace Avalab.Serilog.Sanitize.FormatRules
{
    public class PanUnreadableSanitizingFormatRule : ISanitizingFormatRule
    {
        private readonly Regex _panUnreadableRegex;

        public PanUnreadableSanitizingFormatRule(string regularExperession = @"[3456]\d{3}[- ]?\d{4}[- ]?\d{4}[- ]?\d{4}(?:[- ]?\d{2})?")
        {
            _panUnreadableRegex = new Regex(regularExperession);
        }

        public string Sanitize(string content)
        {
            return _panUnreadableRegex.Replace(content, match =>
            {
                string v = match.Value;

                int count;
                if (!Mod10Check(v, out count))
                    return v;

                int i = 0;
                return Regex.Replace(v, @"\d", match2 =>
                {
                    i++;
                    if (i <= 6 || i > count - 4)
                        return match2.Value;
                    return "*";
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
