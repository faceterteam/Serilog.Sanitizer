using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Avalab.Serilog.Sanitizer.AspNetCore
{
    public static class IWebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureSanitizer(this IWebHostBuilder webHostBuilder, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            SanitizerConfigurationStore.FromOptions(configuration);
            return webHostBuilder;
        } 
    }
}
