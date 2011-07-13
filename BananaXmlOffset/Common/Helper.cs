using System.IO;
using System.Reflection;

namespace BananaXmlOffset.Common
{
    static class Helper
    {
        public static string GetAssemblyRootedPath(string relativePath)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            string assemblyDirectory = Path.GetDirectoryName(callingAssembly.Location);
            string rootedPath = Path.Combine(assemblyDirectory, relativePath);

            return rootedPath;
        }
    }
}
