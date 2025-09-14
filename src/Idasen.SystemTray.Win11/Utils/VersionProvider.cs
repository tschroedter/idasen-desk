using System.Reflection ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public class VersionProvider ( ILogger logger ) : IVersionProvider
{
    public string GetVersion ( )
    {
        var versionAsText = "V-.-.-" ;

        try
        {
            var version = GetAssemblyVersion ( ) ;

            if ( version == null )
            {
                logger.Error ( "Failed to get version" ) ;
            }
            else
            {
                versionAsText = $"V{version.Major}.{version.Minor}.{version.Build}" ;

                logger.Information ( "Version fetched successfully: {Version}" ,
                                     versionAsText ) ;
            }
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to get version" ) ;
        }

        return versionAsText ;
    }

    private static Version ? GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( )
                       .GetName ( )
                       .Version ;
    }
}