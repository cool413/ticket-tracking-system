using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Services.DataCommon
{
    public static class ResourceHelper
    {
        private static Assembly _assembly;
        private static string _namespacePrefix;

        public static void Initial(Assembly assembly, string namespacePrefix)
        {
            _assembly = assembly;
            _namespacePrefix = namespacePrefix;
        }

        public static string GetSql(string resourceName)
        {
            var resource = $"{_namespacePrefix}.{resourceName}";

            Console.WriteLine(resource);

            using (var resourceStream = _assembly.GetManifestResourceStream(resource))
            {
                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static List<string> GetAllSqls(string resourceName)
        {
            var resource = $"{_namespacePrefix}.{resourceName}" + (string.IsNullOrEmpty(resourceName) ? "" : ".");
            var res = _assembly.GetManifestResourceNames()
                        .Where(x => x.IndexOf(resource) > -1)
                        .Select(x => x.Replace($"{_namespacePrefix}.", "")).ToList();

            return res;
        }
    }
}
