using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServiceSettingsProvider
    : IMicroServiceSettingsProvider
{
    private readonly ILogger _logger ;

    public MicroServiceSettingsProvider ( ILogger                         logger ,
                                          IMicroServiceSettingsDictionary dictionary ,
                                          IList < MicroServiceSettings >  settings )
    {
        _logger       = logger ;
        MicroServices = dictionary ;

        Initialize ( settings ) ;
    }

    public IMicroServiceSettingsDictionary MicroServices { get ; }

    public bool TryGetUri ( string   path ,
                            out Uri? uri )
    {
        uri = null ;

        if ( ! MicroServices.TryGetValue ( path ,
                                           out var settings ) )
        {
            _logger.Warning ( $"Unknown path '{path}'" ) ;

            return false ;
        }


        var uriString = settings.Protocol +
                        settings.Host     +
                        settings.Path ;

        uri = new Uri ( uriString ) ;

        _logger.Information ( $"Create Uri: {uri}" ) ;

        return true ;
    }

    private void Initialize ( IList < MicroServiceSettings > settings )
    {
        _logger.Information ( "Initializing..." ) ;

        foreach ( var setting in settings )
            _logger.Information($"{nameof(MicroServiceSettings)}: {setting}");

        MicroServices.Initialize ( settings ) ;

        _logger.Information ( $"...finished using settings: {settings}" ) ;
    }
}