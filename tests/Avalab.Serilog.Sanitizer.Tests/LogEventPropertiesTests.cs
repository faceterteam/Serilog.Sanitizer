using Avalab.Serilog.Sanitizer.Tests.Sinks;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class LogEventPropertiesTests
    {
        [Theory]
        [InlineData("4024007111744339", "402400******4339")]
        [InlineData("4916337037292704", "491633******2704")]
        [InlineData("4916976299226965", "491697******6965")]
        public void WhenSanitizeStructureThenValid(string pan, string maskedPan)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information("Structure:{0}", new { Pan = pan });

            Assert.DoesNotContain(pan, resultMessage);
            Assert.Contains(maskedPan, resultMessage);
            Assert.Equal($"Structure:\"\\\"{{ Pan = {maskedPan} }}\\\"\"", resultMessage);
        }

        [Theory]
        [InlineData("4024007111744339", "402400******4339")]
        [InlineData("4916337037292704", "491633******2704")]
        [InlineData("4916976299226965", "491697******6965")]
        public void WhenSanitizeSequentialArrayThenValid(string pan, string maskedPan)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information("Array:{0}", new object[] { new List<string>() { pan } });

            Assert.DoesNotContain(pan, resultMessage);
            Assert.Contains(maskedPan, resultMessage);
            Assert.Equal($"Array:[\"\\\"{maskedPan}\\\"\"]", resultMessage);
        }

        [Theory]
        [InlineData("4024007111744339", "402400******4339")]
        [InlineData("4916337037292704", "491633******2704")]
        [InlineData("4916976299226965", "491697******6965")]
        public void WhenSanitizeDictionaryThenValid(string pan, string maskedPan)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information("Dictionary:{0}", new object[] {
                new Dictionary<string, string>() { { "pan", pan } } });

            Assert.DoesNotContain(pan, resultMessage);
            Assert.Contains(maskedPan, resultMessage);
            Assert.Equal($"Dictionary:[(\"pan\": \"\\\"{maskedPan}\\\"\")]", resultMessage);
        }
    }
}
