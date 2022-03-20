namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface IMicroServiceSettingsProvider
{
    IMicroServiceSettingsDictionary MicroServices { get ; }

    bool TryGetUri ( string   path ,
                     out Uri? uri ) ;
}