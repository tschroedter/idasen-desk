namespace Idasen.RESTAPI.Desk.Settings ;

public class MicroServiceSettingsProvider
    : IMicroServiceSettingsProvider
{
    private readonly ILogger < MicroServiceSettingsProvider > _logger ;

    public MicroServiceSettingsProvider ( ILogger < MicroServiceSettingsProvider > logger,
                                          IList<MicroServiceSettings> settings)
    {
        _logger = logger ;

        Initialize (settings) ;
    }

    public MicroServiceSettingsDictionary MicroServices { get ; } = new( ) ;

    private MicroServiceSettingsProvider Initialize (IList<MicroServiceSettings> settings)
    {
        _logger.LogInformation ( "Initializing..." ) ;

        MicroServices.Initialize ( settings ) ;

        _logger.LogInformation ( $"...finished using settings: {settings}" ) ;

        return this ;
    }

    public bool TryGetUri ( string   path ,
                            out Uri? uri )
    {
        uri = null ;

        if ( ! MicroServices.TryGetValue ( path ,
                                           out var settings ) )
        {
            _logger.LogWarning ( $"Unknown path '{path}'" ) ;

            return false ;
        }


        var uriString = settings.Protocol +
                        settings.Host     +
                        settings.Path ;

        uri = new Uri ( uriString ) ;

        _logger.LogInformation ( $"Create Uri: {uri}" ) ;

        return true ;
    }
}

public interface IMicroServiceSettingsProvider
{
    MicroServiceSettingsDictionary MicroServices { get ; }

    bool TryGetUri ( string   path ,
                     out Uri? uri ) ;
}