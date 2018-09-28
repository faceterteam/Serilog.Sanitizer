using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitize.Configuration
{
    public class ExtendLoggerConfiguration : LoggerConfiguration
    {
        public LoggerSanitizeConfiguration Sanitize { get; }
    }
}
