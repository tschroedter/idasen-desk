namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public interface IMicroServiceSettingsUriCreator
{
    Uri GetUri ( MicroServiceSettings settings ) ;
}