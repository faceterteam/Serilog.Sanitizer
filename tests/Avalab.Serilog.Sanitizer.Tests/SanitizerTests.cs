using Avalab.Serilog.Sanitizer.Tests.Sinks;
using Serilog;
using Serilog.Events;
using Serilog.Formatting;
using System;
using System.IO;
using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class SanitizerTests
    {
        private ITextFormatter _formatter; 

        public SanitizerTests()
        {
            _formatter = new PanCvvMaskedCompositeFormatter("{Message}");
        }

        [Theory]
        [InlineData("4024007111744339")]
        [InlineData("4916337037292704")]
        [InlineData("4916976299226965")]
        [InlineData("4024007192667714")]
        [InlineData("4024007133952274")]
        [InlineData("5457563923156875")]
        [InlineData("5486760586327273")]
        [InlineData("5219957877355724")]
        [InlineData("5523737085433088")]
        [InlineData("5461458829025213")]
        public void WhenRealPanThenDoesNotContainPan(string pan)
        {
            LogEvent evt = null;

            var logger = new LoggerConfiguration()
                            .WriteTo.Sink(new DelegatingSink(e => evt = e))
                            .CreateLogger();

            logger.Information($"Information with {pan} pan");

            TextWriter writer = new StringWriter();
            _formatter.Format(evt, writer);

            Assert.DoesNotContain(pan, writer.ToString());
        }

        [Theory]
        [InlineData("4111111111111110")]
        [InlineData("4123123123123123")]
        [InlineData("4123456789012345")]
        public void WhenNotPanThenContainsNumber(string number)
        {
            LogEvent evt = null;

            var logger = new LoggerConfiguration()
                            .WriteTo.Sink(new DelegatingSink(e => evt = e))
                            .CreateLogger();

            logger.Information($"Information with {number} number");

            TextWriter writer = new StringWriter();
            _formatter.Format(evt, writer);

            Assert.Contains(number, writer.ToString());
        }

        [Theory]
        [InlineData("{cvv\":\"123\"}")]
        [InlineData("{cvv\": \"123\"}")]
        [InlineData("{Cvv\":\"123\"}")]
        [InlineData("{Cvv\": \"123\"}")]
        [InlineData("{CVV\":\"123\"}")]
        [InlineData("{CVV\": \"123\"}")]
        [InlineData("{cvv\":123}")]
        [InlineData("{cvv\": 123}")]
        [InlineData("{Cvv\":123}")]
        [InlineData("{Cvv\": 123}")]
        [InlineData("{CVV\":123}")]
        [InlineData("{CVV\": 123}")]
        public void WhenFoundCvvThenDoesNotContainCvv(string cvv)
        {
            LogEvent evt = null;

            var logger = new LoggerConfiguration()
                            .WriteTo.Sink(new DelegatingSink(e => evt = e))
                            .CreateLogger();

            logger.Information($"Information with {cvv} cvv");

            TextWriter writer = new StringWriter();
            _formatter.Format(evt, writer);

            Assert.DoesNotContain("123", writer.ToString());
        }
    }
}
