using System.Diagnostics.CodeAnalysis ;
using System.Reflection ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class AssemblyVersionProvider
    : IAssemblyVersionProvider
{
    public Version ? GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( )
                       .GetName ( )
                       .Version ;
    }
}