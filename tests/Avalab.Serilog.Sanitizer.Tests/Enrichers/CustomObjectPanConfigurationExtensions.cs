using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitizer.Tests.Enrichers
{
    public static class CustomObjectPanConfigurationExtensions
    {
        public static LoggerConfiguration WithCustomObjectPan(
            this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, 
            string pan,
            object cvv
            )
        {
            return loggerEnrichmentConfiguration.With(new CustomObjectPanCvvEnricher(pan, cvv));
        }
    }
}
