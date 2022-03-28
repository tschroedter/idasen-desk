using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServiceSettingsProvider
    : IMicroServiceSettingsProvider
{
    private readonly ILogger                         _logger ;
    private readonly IMicroServiceSettingsUriCreator _uriCreator ;

    public MicroServiceSettingsProvider ( ILogger                         logger ,
                                          IMicroServiceSettingsDictionary dictionary ,
                                          IList < MicroServiceSettings >  settings,
                                          IMicroServiceSettingsUriCreator uriCreator)
    {
        _logger       = logger ;
        MicroServices = dictionary ;
        _uriCreator   = uriCreator;

        Initialize( settings ) ;
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

        uri = _uriCreator.GetUri ( settings );

        _logger.Information ( $"Create Uri: {uri}" ) ;

        return true ;
    }

    private void Initialize ( IList < MicroServiceSettings > settings )
    {
        _logger.Information ( $"Initializing {settings.Count}..." ) ;

        MicroServices.Initialize ( settings ) ;

        _logger.Information ( $"...finished using settings: {settings.ToCsv()}" ) ;
    }
}