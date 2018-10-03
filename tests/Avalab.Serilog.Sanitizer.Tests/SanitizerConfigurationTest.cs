using Avalab.Serilog.Sanitizer.Tests.Sinks;

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;

using System.IO;
using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class SanitizerConfigurationTest
    {
        private readonly ITextFormatter _formatter;

        public SanitizerConfigurationTest()
        {
            _formatter = new MessageTemplateTextFormatter("{Message}", null);
        }

        [Theory]
        [InlineData("4024007111744339")]
        public void WhenReadAllFormatersThenOk(string pan)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/WhenReadAllFormatersThenOk.json")
                                    .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            var writer = new StringWriter();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .WriteTo.Sanitizer(
                                    s => s.Delegate(
                                        lgEvent => _formatter.Format(lgEvent, writer)));

            var logger = config.CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain("123", writer.ToString());
            Assert.DoesNotContain("5677", writer.ToString());
        }
    }
}
