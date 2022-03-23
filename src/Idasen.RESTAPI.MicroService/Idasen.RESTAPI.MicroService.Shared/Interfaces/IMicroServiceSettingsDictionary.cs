using Idasen.RESTAPI.MicroService.Shared.Settings ;

namespace Idasen.RESTAPI.MicroService.Shared.Interfaces ;

public interface IMicroServiceSettingsDictionary : IDictionary < string , MicroServiceSettings >
{
    IMicroServiceSettingsDictionary Initialize ( IList < MicroServiceSettings > settings ) ;
}