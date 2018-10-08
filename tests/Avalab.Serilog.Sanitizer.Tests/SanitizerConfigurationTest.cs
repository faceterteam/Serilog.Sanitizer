using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class SanitizerConfigurationTest
    {
        private readonly ITextFormatter _formatter;
        private readonly TextWriter _writer;

        public SanitizerConfigurationTest()
        {
            _formatter = new MessageTemplateTextFormatter("{Message}", null);

            _writer = new StringWriter(CultureInfo.InvariantCulture);
            Trace.Listeners.Clear();
            var listener = new TextWriterTraceListener(_writer);
            listener.TraceOutputOptions = TraceOptions.None;
            Trace.Listeners.Add(listener);
        }

        [Theory]
        [InlineData("4024007111744339", "402400******4339")]
        [InlineData("4916337037292704", "491633******2704")]
        [InlineData("4916976299226965", "491697******6965")]
        [InlineData("4024007192667714", "402400******7714")]
        [InlineData("4024007133952274", "402400******2274")]
        [InlineData("5457563923156875", "545756******6875")]
        [InlineData("5486760586327273", "548676******7273")]
        [InlineData("5219957877355724", "521995******5724")]
        [InlineData("5523737085433088", "552373******3088")]
        [InlineData("5461458829025213", "546145******5213")]
        [InlineData("6011476719734606", "601147******4606")]
        [InlineData("6011988355763293", "601198******3293")]
        [InlineData("6011732295251781", "601173******1781")]
        [InlineData("6011117091981544", "601111******1544")]
        [InlineData("6011763731314420", "601176******4420")]
        public void PanUnreadable_To_Trace(string pan, string maskedPan)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/PanUnreadable_To_Trace.json")
                                    .Build();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration);
            var logger = config.CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain(pan, _writer.ToString());
            Assert.Contains(maskedPan, _writer.ToString());
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
                                        .AddJsonFile("assets/CvvHidden_To_Trace.json")
                                    .Build();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration);
            var logger = config.CreateLogger();

            logger.Information($"Information with {cvv} cvv");

            Assert.DoesNotContain("123", _writer.ToString());
            Assert.Contains("***", _writer.ToString());
        }

        [Theory]
        [InlineData("4024007111744339", "402400******4339", "{cvv\":\"123\"}")]
        [InlineData("4916337037292704", "491633******2704", "{cvv\": \"123\"}")]
        [InlineData("4916976299226965", "491697******6965", "{Cvv\":\"123\"}")]
        [InlineData("4024007192667714", "402400******7714", "{Cvv\": \"123\"}")]
        [InlineData("4024007133952274", "402400******2274", "{CVV\":\"123\"}")]
        [InlineData("5457563923156875", "545756******6875", "{CVV\": \"123\"}")]
        [InlineData("5486760586327273", "548676******7273", "{cvv\":123}")]
        [InlineData("5219957877355724", "521995******5724", "{cvv\": 123}")]
        [InlineData("5523737085433088", "552373******3088", "{Cvv\":123}")]
        [InlineData("5461458829025213", "546145******5213", "{Cvv\": 123}")]
        [InlineData("6011476719734606", "601147******4606", "{CVV\":123}")]
        [InlineData("6011988355763293", "601198******3293", "{CVV: 123}")]
        [InlineData("6011732295251781", "601173******1781", "{cvv :123}")]
        [InlineData("6011117091981544", "601111******1544", "{cvv : 123}")]
        public void PanUnreadable_and_CvvHidden_To_Trace(string pan, string maskedPan, string cvv)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/PanUnreadable_and_CvvHidden_To_Trace.json")
                                    .Build();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration);
            var logger = config.CreateLogger();

            logger.Information($"Information with {pan} pan, and {cvv}");

            Assert.DoesNotContain(pan, _writer.ToString());
            Assert.Contains(maskedPan, _writer.ToString());
            Assert.DoesNotContain(cvv, _writer.ToString());
        }

        [Theory]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public void CustomRule_Truncate50_To_Trace_longText(string text)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/Truncate50_To_Trace.json")
                                    .Build();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration);
            var logger = config.CreateLogger();

            logger.Information(text);

            Assert.DoesNotContain(text, _writer.ToString());
            Assert.EndsWith("..." + Environment.NewLine, _writer.ToString());
        }

        [Theory]
        [InlineData("short text")]
        public void CustomRule_Truncate50_To_Trace_shortText(string text)
        {
            var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("assets/Truncate50_To_Trace.json")
                                    .Build();
            var config = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration);
            var logger = config.CreateLogger();

            logger.Information(text);

            Assert.Contains(text, _writer.ToString());
        }
    }
}
