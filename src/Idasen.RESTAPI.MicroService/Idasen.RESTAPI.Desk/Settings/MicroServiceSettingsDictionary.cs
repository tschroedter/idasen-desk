namespace Idasen.RESTAPI.Desk.Settings ;

public class MicroServiceSettingsDictionary
    : Dictionary < string , MicroServiceSettings >
{
    private readonly ILogger _logger = ApplicationLogging.CreateLogger < MicroServiceSettingsDictionary > ( ) ;

    public MicroServiceSettingsDictionary Initialize ( IList < MicroServiceSettings > settings )
    {
        Clear ( ) ;

        foreach ( var setting in settings )
        {
            if ( ! TryAdd ( setting.Path.ToLower ( ) ,
                            setting ) )
                _logger.LogWarning ( $"Duplicated URL key '{setting.Path}' value is ignored ('{setting}')." ) ;
        }

        return this ;
    }
}