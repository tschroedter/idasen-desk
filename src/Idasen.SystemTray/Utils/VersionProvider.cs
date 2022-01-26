using System.Reflection ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray.Utils
{
    public class VersionProvider
    : IVersionProvider
    {
        public string GetVersion ( )
        {
            var version = Assembly.GetExecutingAssembly ( )
                                  .GetName ( )
                                  .Version ;

            return version is null
                       ? "V0.0.0"
                       : $"V{version.Major}.{version.Minor}.{version.Build}" ;
        }
    }
}