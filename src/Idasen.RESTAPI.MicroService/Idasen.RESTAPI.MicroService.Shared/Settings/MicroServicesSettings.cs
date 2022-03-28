using System.ComponentModel.DataAnnotations ;
using JetBrains.Annotations ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServicesSettings
{
    [ Required ]
    [ UsedImplicitly ]
    public IList < MicroServiceSettings > MicroServices { get ; set ; } = Array.Empty < MicroServiceSettings > ( ) ;
}