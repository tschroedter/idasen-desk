using System.Reflection ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public class VersionProvider
    : IVersionProvider
{
    private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                     Constants.LogFilename ) ;

    public string GetVersion ( )
    {
        var versionAsText = "V-.-.-" ;

        try
        {
            var version = GetAssemblyVersion ( ) ;

            if ( version == null )
            {
                _logger.Error ( "Failed to get version" ) ;
            }
            else
            {
                versionAsText = $"V{version.Major}.{version.Minor}.{version.Build}" ;

                _logger.Information ( $"Version fetched successfully: {versionAsText}" ) ;
            }
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
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