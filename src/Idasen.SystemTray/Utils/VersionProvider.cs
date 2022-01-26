using System.Reflection ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Utils
{
    public class VersionProvider
    : IVersionProvider
    {
        public string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly()
                                  .GetName()
                                  .Version?
                                  .ToString()
                       ?? "0.0.0.0";

            return "V" + version;
        }
    }
}