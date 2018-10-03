using Avalab.Serilog.Sanitizer.Extensions;

using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;


namespace Avalab.Serilog.Sanitizer
{
    internal static class SanitizerConfigurationStore
    {
        public static IReadOnlyCollection<FormatterMetaInfo> Rules { get; private set; }
        private static IChangeToken _changeToken;

        private static readonly IReadOnlyCollection<Type> _rules;
        private static readonly ConcurrentDictionary<(Type, IReadOnlyDictionary<string,string>), ISanitizingFormatRule> _cachedRules;

        static SanitizerConfigurationStore()
        {
            var interfaceType = typeof(ISanitizingFormatRule);
            _cachedRules = new ConcurrentDictionary<(Type, IReadOnlyDictionary<string,string>), ISanitizingFormatRule>();
            var rules = AppDomain
                    .CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypesWithInterface(interfaceType));
            _rules = rules.ToList();
        }

        public static IReadOnlyCollection<ISanitizingFormatRule> GetFormatters()
        {
            return Rules
                    .Select(setting => GetFormatter(setting.Name, setting.Args))
                    .Where(entry => entry != null)
                .ToList();
        }

        private static ISanitizingFormatRule GetFormatter(string formatterId, IReadOnlyDictionary<string, string> argsForCtor)
        {
            var ruleType = _rules.FirstOrDefault(type => type.Name.Equals(formatterId));
            if (ruleType == null) return null;

            if (_cachedRules.TryGetValue((ruleType, argsForCtor), out var returnInstance))
                return returnInstance;

            var diff = new Dictionary<ConstructorInfo, int>();
            var minCtor = ruleType
                    .GetConstructors()
                    .Aggregate(diff,
                            (seed, ctorInfo) => 
                            {
                                seed.Add(ctorInfo,
                                    ctorInfo.GetParameters()
                                        .Select(parameter => parameter.Name)
                                        .Except(argsForCtor.Keys).Count());
                                return seed;
                            })
                    .Min(record => record.Value);
            var ctor = diff.First(record => record.Value == minCtor).Key;

            var arrayArguments = ctor
                    .GetParameters()
                    .Select(parameter => argsForCtor
                        .FirstOrDefault(argInput => argInput.Key.Equals(parameter.Name)).Value)
                    .Select(entry => entry ?? Type.Missing)
                .ToArray();

            return _cachedRules.GetOrAdd((ruleType, argsForCtor), 
                (ISanitizingFormatRule)ctor.Invoke(BindingFlags.CreateInstance |
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.OptionalParamBinding,
                        null, arrayArguments, CultureInfo.InvariantCulture));
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
                .GetSection("rules");

            if (!sanitizerOptionSection.Exists())
                throw new ArgumentNullException(nameof(sanitizerOptionSection));

            _changeToken = sanitizerOptionSection.GetReloadToken();

            _changeToken.RegisterChangeCallback(
                obj => Rules = configuration.GetSection(sanitizerOptionSection.Path).Get<List<FormatterMetaInfo>>(),
            null);

            Rules = configuration.GetSection(sanitizerOptionSection.Path).Get<List<FormatterMetaInfo>>();
        }

        public static void FromOptions(IReadOnlyCollection<FormatterMetaInfo> configuration) => Rules = configuration;
    }
}
