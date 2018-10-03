using Avalab.Serilog.Sanitizer.Tests.Sinks;
using Avalab.Serilog.Sanitizer.Extensions;

using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;

using System.IO;
using System.Linq;
using System.Collections.Generic;

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

        [Fact]
        public void WhenReadConfigurationFromClassCorrectlyThenOk()
        {
            SanitizerConfigurationStore.FromOptions(new List<FormatterMetaInfo>()
                {
                    new FormatterMetaInfo()
                    {
                        Name = "PanUnreadableSanitizingFormatRule",
                        Args = new Dictionary<string,string>
                        {
                            { "regularExpression", "[3456]\\d{3}[- ]?\\d{4}[- ]?\\d{4}[- ]?\\d{4}(?:[- ]?\\d{2})?" },
                            { "replaceChar", "*" }
                        }
                    }
            });

            var formatters = SanitizerConfigurationStore
                .Rules;

            Assert.Equal(1, formatters.Count);
            Assert.Contains(formatters,
                formatter => formatter.Name == "PanUnreadableSanitizingFormatRule" &&
                formatter.Args.Values.Except(new string[] 
                {
                    "[3456]\\d{3}[- ]?\\d{4}[- ]?\\d{4}[- ]?\\d{4}(?:[- ]?\\d{2})?", "*"
                }).Count() == 0);
        }

        [Fact]
        public void WhenReadConfigurationFromJsonCorrectlyThenOk()
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/WhenReadConfigurationFromJsonCorrectlyThenOk.json", false, true)
                                    .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            var formatters = SanitizerConfigurationStore
                .Rules;

            Assert.Equal(3, formatters.Count);
            Assert.Contains(formatters, 
                formatter => formatter.Name == "PanUnreadableSanitizingFormatRule" &&
                formatter.Args.Values.Except(new string[] {
                    "[3456]\\d{3}[- ]?\\d{4}[- ]?\\d{4}[- ]?\\d{4}(?:[- ]?\\d{2})?", "*" 
                }).Count() == 0);

            Assert.Contains(formatters, 
                formatter => formatter.Name == "CvvHiddenSanitizingFormatRule" &&
                formatter.Args.Values.Except(new string[] {
                    "(?i)cvv\"?[ ]?:[ ]?\"?\\d{3}\"?", "*"
            }).Count() == 0);

            Assert.Contains(formatters,
                formatter => formatter.Name == "CvvHiddenSanitizingFormatRule" &&
                formatter.Args.Values.Except(new string[] {
                    "(?i)cvv\"?[ ]?:[ ]?\"?\\d{4}\"?", "*"
            }).Count() == 0);
        }

        [Theory]
        [InlineData("4024007111744339")]
        public void WhenReadAllFormatersAndCheckPanThenOk(string pan)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/WhenReadAllFormatersAndCheckPanThenOk.json")
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

            Assert.DoesNotContain(pan, writer.ToString());
        }

        [Theory]
        [InlineData("{cvv : 123}")]
        public void WhenReadAllFormatersAndCheckCvvThenOk(string cvv)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/WhenReadAllFormatersAndCheckCvvThenOk.json")
                                    .Build();

            SanitizerConfigurationStore.FromOptions(configuration);

            var writer = new StringWriter();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .WriteTo.Sanitizer(
                                    s => s.Delegate(
                                        lgEvent => _formatter.Format(lgEvent, writer)));

            var logger = config.CreateLogger();

            logger.Information($"Information with {cvv} pan");

            Assert.DoesNotContain(cvv, writer.ToString());
        }
    }
}
