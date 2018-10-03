using Avalab.Serilog.Sanitizer.Extensions;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;

namespace Avalab.Serilog.Sanitizer
{
    internal static class SanitizerConfigurationStore
    {
        public static SanitizerSinkOptions SanitizerSinkOptions { get; private set; }
        private static IChangeToken _changeToken;

        private static readonly IReadOnlyCollection<Type> _rules;
        private static readonly ConcurrentDictionary<(Type, IReadOnlyCollection<string>), ISanitizingFormatRule> _cachedRules;

        static SanitizerConfigurationStore()
        {
            var interfaceType = typeof(ISanitizingFormatRule);
            _cachedRules = new ConcurrentDictionary<(Type, IReadOnlyCollection<string>), ISanitizingFormatRule>();
            var rules = AppDomain
                    .CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypesWithInterface(interfaceType));
            _rules = rules.ToList();
        }

        public static IReadOnlyCollection<ISanitizingFormatRule> GetFormatters()
        {
            return SanitizerSinkOptions
                    .Formatters
                    .Select(setting => GetFormatter(setting.Name, setting.Args.ToArray()))
                    .Where(entry => entry != null)
                .ToList();
        }

        private static ISanitizingFormatRule GetFormatter(string formatterId, string[] argsForCtor)
        {
            var ruleType = _rules.FirstOrDefault(type => type.Name.Equals(formatterId));
            if (ruleType == null) return null;

            if (_cachedRules.TryGetValue((ruleType, argsForCtor), out var returnInstance))
                return returnInstance;

            return _cachedRules.GetOrAdd((ruleType, argsForCtor), (ISanitizingFormatRule)Activator.CreateInstance(ruleType, argsForCtor));
        }

        public static void FromOptions(IConfiguration configuration)
        {
            var sanitizerSectionPath = string.Concat(configuration
                .GetSection("Serilog:WriteTo")
                    .GetChildren()
                    .Select(children => children
                                            .GetChildren()
                                            .First(child => child.Key == "Name" && child.Value == "Sanitizer"))
                    .First()
                    .Path
                    .Reverse()
                    .Skip(":Name".Count())
                .Reverse());

            var sanitizerOptionSection = configuration
                    .GetSection(sanitizerSectionPath)
                    .GetChildren()
                    .First(children => children.Key == "Args")
                .GetSection("SanitizerSinkOptions");

            if (!sanitizerOptionSection.Exists())
                throw new ArgumentNullException(nameof(sanitizerOptionSection));

            _changeToken = sanitizerOptionSection.GetReloadToken();

            _changeToken.RegisterChangeCallback(
                obj => SanitizerSinkOptions = configuration.GetSection(sanitizerOptionSection.Path).Get<SanitizerSinkOptions>(),
            null);

            SanitizerSinkOptions = configuration.GetSection(sanitizerOptionSection.Path).Get<SanitizerSinkOptions>();
        }

        public static void FromOptions(SanitizerSinkOptions configuration) => SanitizerSinkOptions = configuration;
    }
}
