using Serilog;
using Serilog.Configuration;

using Avalab.Serilog.Sanitizer.Sinks;

using Microsoft.Extensions.Configuration;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Avalab.Serilog.Sanitizer
{
    public static class SanitizeLoggerConfigurationExtensions
    {
        public static LoggerConfiguration Sanitizer(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            Action<LoggerSinkConfiguration> sinks)
        {
            if (loggerSinkConfiguration == null) throw new ArgumentNullException(nameof(loggerSinkConfiguration));

            return LoggerSinkConfiguration.Wrap(
                loggerSinkConfiguration,
                wrappedSink => new SanitizeSink(
                SanitizerConfigurationStore.GetFormatters()
                , wrappedSink),
                sinks);
        }
    }

    public static class SanitizerConfigurationStore
    {
        public static SanitizerSinkOptions SanitizerSinkOptions { get; private set; }
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

        public static void FromOptions(IConfiguration configuration) => SanitizerSinkOptions = configuration.GetSection("Serilog:WriteTo:0:Args:SanitizerSinkOptions").Get<SanitizerSinkOptions>();
    }

    internal static class AssemblyExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/a/29379834
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        internal static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
           
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        internal static IEnumerable<Type> GetTypesWithInterface(this Assembly asm, Type interfaceType)
        {
            return asm
                .GetLoadableTypes()
                .Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass)
            .ToList();
        }
    }
}
