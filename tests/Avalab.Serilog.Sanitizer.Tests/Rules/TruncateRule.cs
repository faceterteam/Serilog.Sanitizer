using System;

namespace Avalab.Serilog.Sanitizer.Tests.Rules
{
    class TruncateRule : AbstractSanitizingRule
    {
        private readonly int _maxLength;
        private readonly string _endString;

        public TruncateRule(int maxLength, string endString)
        {
            if (maxLength <= endString.Length)
                throw new ArgumentException("maxLength shuld be Longer for endString.Length");

            _maxLength = maxLength;
            _endString = endString;
        }

        public override string Sanitize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return content;

            if (content.Length <= _maxLength)
                return content;

            return content?.Substring(0, _maxLength - _endString.Length) + _endString;
        }
    }
}
