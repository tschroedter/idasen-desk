using System ;
using System.Reflection ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Utils ;

public class VersionProvider
    : IVersionProvider
{
    private readonly ILogger _logger ;

    public VersionProvider ( ILogger logger)
    {
        Guard.ArgumentNotNull ( logger, nameof(logger) );
        
        _logger = logger ;
    }
    
    public string GetVersion ( )
    {
        var versionAsText = "V-.-.-" ;

        try
        {
            var version = GetAssemblyVersion ( ) ;

            versionAsText = $"V{version.Major}.{version.Minor}.{version.Build}" ;

            _logger.Information ( $"Version fetched successfully: {versionAsText}" ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to get version" ) ;
        }

        return versionAsText ;
    }

    private static Version GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( )
                       .GetName ( )
                       .Version ;
    }
}