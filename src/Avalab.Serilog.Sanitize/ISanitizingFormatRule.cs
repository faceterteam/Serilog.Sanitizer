using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitize
{
    public interface ISanitizingFormatRule
    {
        string Sanitize(string content);
    }
}
