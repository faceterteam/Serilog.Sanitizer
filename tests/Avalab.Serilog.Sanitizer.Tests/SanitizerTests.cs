using Avalab.Serilog.Sanitizer.Tests.Enrichers;
using Avalab.Serilog.Sanitizer.Tests.Rules;
using Avalab.Serilog.Sanitizer.Tests.Sinks;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Avalab.Serilog.Sanitizer.Tests
{
    public class SanitizerTests
    {
        public SanitizerTests()
        {
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
        public void WhenRealPanThenDoesNotContainPan(string pan, string maskedPan)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain(pan, resultMessage);
            Assert.Contains(maskedPan, resultMessage);
            Assert.Equal($"Information with {maskedPan} pan", resultMessage);
        }

        [Theory]
        [InlineData("340023435852620", "340023******620")]
        [InlineData("371262827341725", "371262******725")]
        [InlineData("377910762079718", "377910******718")]
        [InlineData("347281327653141", "347281******141")]
        [InlineData("373499269515725", "373499******725")]
        public void WhenRealAmericanExpressPanThenDoesNotContainPan(string pan, string maskedPan)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(
                                        regularExpression: @"[3456]\d{3}[- ]?\d{4}[- ]?\d{4}[- ]?\d{3,4}(?:[- ]?\d{2})?",
                                        endSkipCount: 3),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {pan} pan");

            Assert.DoesNotContain(pan, resultMessage);
            Assert.Contains(maskedPan, resultMessage);
            Assert.Equal($"Information with {maskedPan} pan", resultMessage);
        }

        [Theory]
        [InlineData("3111111111111110")]
        [InlineData("3123123123123120")]
        [InlineData("3123456789012345")]
        [InlineData("4111111111111110")]
        [InlineData("4123123123123123")]
        [InlineData("4123456789012345")]
        [InlineData("5111111111111110")]
        [InlineData("5123123123123123")]
        [InlineData("5123456789012345")]
        [InlineData("6111111111111110")]
        [InlineData("6123123123123123")]
        [InlineData("6123456789012345")]
        public void WhenNotPanThenContainsNumber(string number)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                 .WriteTo.Sanitizer(
                                    r => r.PanUnreadable(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {number} number");

            Assert.Contains(number, resultMessage);
            Assert.Equal($"Information with {number} number", resultMessage);
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
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    r => r.CvvHidden(),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {cvv} cvv");

            Assert.DoesNotContain("123", resultMessage);
            Assert.Contains("***", resultMessage);
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
        public void WhenPanAndCvvFoundThenDoesNotContainPanAndCvv(string pan, string maskedPan, string cvv)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    r => { r.PanUnreadable(); r.CvvHidden(); },
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {pan} pan, and {cvv}");

            Assert.DoesNotContain(pan, resultMessage.ToString());
            Assert.Contains(maskedPan, resultMessage.ToString());
            Assert.DoesNotContain(cvv, resultMessage.ToString());
        }

        [Theory]
        [InlineData("127.0.0.1", "127.***.***.***")]
        [InlineData("192.168.10.1", "192.***.***.***")]
        [InlineData("255.255.255.255", "255.***.***.***")]
        public void WhenSecretIpFoundThenDoesNotContainIp(string ip, string maskedIp)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    r => { r.RegexHidden(
                                        @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)",
                                        @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
                                        "***.***.***");  },
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information($"Information with {ip} secret ip");

            Assert.DoesNotContain(ip, resultMessage.ToString());
            Assert.Contains(maskedIp, resultMessage.ToString());
            Assert.Equal($"Information with {maskedIp} secret ip", resultMessage);
        }

        [Theory]
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")]
        public void CustomRule_WhenTextLongerThenTruncate(string text)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    r => r.Truncate(50, "..."),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information(text);

            Assert.DoesNotContain(text, resultMessage);
            Assert.EndsWith("...", resultMessage);
        }

        [Theory]
        [InlineData("short text")]
        public void CustomRule_WhenTextShorterThenNotTruncate(string text)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                                .WriteTo.Sanitizer(
                                    r => r.Truncate(50, "..."),
                                    s => s.Delegate(evt => resultMessage = evt))
                            .CreateLogger();

            logger.Information(text);

            Assert.Contains(text, resultMessage);
        }

        [Theory]
        [InlineData("4024007111744339", "123")]
        [InlineData("4916337037292704", 123)]
        public void Properties_WhenRealPanThenDoesNotContainPan(string pan, object cvv)
        {
            string resultMessage = string.Empty;
            var logger = new LoggerConfiguration()
                            .Enrich.WithCustomObjectPan(pan, cvv) // 3 times contain Pan
                                .WriteTo.Sanitizer(
                                    r => { r.PanUnreadable(); r.CvvHidden(); },
                                    s => s.Delegate(evt => resultMessage = evt,
                                    "{CustomObjectPan} {Message}"))
                            .CreateLogger();

            logger.Information("Message with CustomObjectPan Enrichers");

            Assert.DoesNotContain(pan, resultMessage);
            Assert.DoesNotContain("123", resultMessage);
        }
    }
}
