using Avalab.Serilog.Sanitizer.Tests.Sinks;
using Avalab.Serilog.Sanitizer.Extensions;

using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;

using System.IO;

using Microsoft.Extensions.Configuration;

using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class SanitizerTests
    {
        private readonly ITextFormatter _formatter;

        public SanitizerTests()
        {
            _formatter = new MessageTemplateTextFormatter("{Message}", null);
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
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("assets/sanitizer-general.json")
                        .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            TextWriter writer = new StringWriter();
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    s => s.Delegate(evt => _formatter.Format(evt, writer)))
                            .CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain(pan, writer.ToString());
        }

        [Theory]
        [InlineData("4111111111111110")]
        [InlineData("4123123123123123")]
        [InlineData("4123456789012345")]
        public void WhenNotPanThenContainsNumber(string number)
        {
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("assets/sanitizer-general.json")
                        .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            TextWriter writer = new StringWriter();
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    s => s.Delegate(evt => _formatter.Format(evt, writer)))
                            .CreateLogger();

            logger.Information($"Information with {number} number");

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
        [InlineData("{CVV: 123}")]
        [InlineData("{cvv :123}")]
        [InlineData("{cvv : 123}")]
        public void WhenFoundCvvThenDoesNotContainCvv(string cvv)
        {
            var configuration = new ConfigurationBuilder()
                            .AddJsonFile("assets/sanitizer-general.json")
                        .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            TextWriter writer = new StringWriter();
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    s => s.Delegate(evt => _formatter.Format(evt, writer)))
                            .CreateLogger();

            logger.Information($"Information with {cvv} cvv");

            Assert.DoesNotContain("123", writer.ToString());
        }
    }
}
