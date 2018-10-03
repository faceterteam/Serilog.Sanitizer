using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Avalab.Serilog.Sanitizer.Extensions
{
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
