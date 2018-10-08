using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalab.Serilog.Sanitizer.Tests.Enrichers
{
    class CustomObjectPanCvvEnricher : ILogEventEnricher
    {
        private readonly string _pan;
        private readonly object _cvv;

        public CustomObjectPanCvvEnricher(string pan, object cvv)
        {
            _pan = pan;
            _cvv = cvv;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CustomObjectPan", new
            {
                Pan = _pan,
                Cvv = _cvv,
                Child = new
                {
                    Pan2 = _pan,
                    Cvv = _cvv
                }
            }, destructureObjects: true));
        }
    }
}
