using Avalab.Serilog.Sanitizer.Tests.Sinks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
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


            var sanitizerSettings = new SanitizerSinkOptions();
            configuration.GetSection("Serilog:WriteTo:0:Args:SanitizerSinkOptions").Bind(sanitizerSettings);

            ConfigurationStore.SanitizerSinkOptions = sanitizerSettings;


            var writer = new StringWriter();
            var logger = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .WriteTo.Sanitizer(
                                    s => s.Delegate(
                                        lgEvent => _formatter.Format(lgEvent, writer)))
                             .CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain(pan, writer.ToString());
        }
    }
}
