using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitizer
{
    public interface ISanitizingFormatRule
    {
        string Sanitize(string content);
    }
}
