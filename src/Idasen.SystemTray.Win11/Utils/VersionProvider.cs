using System.Reflection ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public class VersionProvider ( ILogger logger ) : IVersionProvider
{
    private string ? _cachedVersion ;

    public string GetVersion ( )
    {
        if ( _cachedVersion != null )
            return _cachedVersion ;

        var versionAsText = "v-.-.-" ;

        try
        {
            var version = GetAssemblyVersion ( ) ;

            if ( version == null )
            {
                logger.Error ( "Failed to get version" ) ;
            }
            else
            {
                versionAsText = $"v{version.Major}.{version.Minor}.{version.Build}" ;

                logger.Information ( "Version fetched successfully: {Version}" ,
                                     versionAsText ) ;
            }
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to get version" ) ;
        }

        _cachedVersion = versionAsText ;

        return versionAsText ;
    }

    private static Version ? GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( )
                       .GetName ( )
                       .Version ;
    }
}