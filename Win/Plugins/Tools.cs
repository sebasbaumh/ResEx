using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ResEx.Core.PlugIns;

namespace ResEx.Win.PlugIns
{
    /// <summary>
    /// Tools regarding plug ins
    /// </summary>
    public static class Tools
    {
        public static IEnumerable<PlugInInstance> DiscoverPlugIns(string plugInsPath)
        {
            var result = new List<PlugInInstance>();

            // find all dlls in the plug ins path
            var dlls = Directory.GetFiles(plugInsPath, "*.dll", SearchOption.AllDirectories);

            // browse all that files
            foreach (var dll in dlls)
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(dll);
                }
                catch
                {
                    // If failed to load assembly then ingore exception and
                    // go to the next.
                    // TODO : Log the exception
                    continue;
                }

                // search all types of the assembly
                foreach (var type in assembly.GetTypes())
                {
                    // type is plug in if is decorated with corresponding attribute
                    var plugInAttribute = GetPlugInAttribute(type);
                    if (plugInAttribute != null)
                    {
                        result.Add(new PlugInInstance { ClassType = type, Info = plugInAttribute });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the <see cref="PlugInAttribute"/> instance of the type if the is decorated
        /// with this attribute. If not returns null.
        /// </summary>
        public static PlugInAttribute GetPlugInAttribute(this MemberInfo type)
        {
            var attributes = type.GetCustomAttributes(typeof(PlugInAttribute), true);
            if (attributes.Length == 0)
            {
                return null;
            }
            else
            {
                return (PlugInAttribute)attributes[0];
            }
        }
    }
}