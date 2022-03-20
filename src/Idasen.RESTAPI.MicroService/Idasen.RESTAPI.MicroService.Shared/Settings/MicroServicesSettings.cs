using System.ComponentModel.DataAnnotations ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServicesSettings
{
    [ Required ]
    public IList < MicroServiceSettings > MicroServices { get ; set ; } = Array.Empty < MicroServiceSettings > ( ) ;
}