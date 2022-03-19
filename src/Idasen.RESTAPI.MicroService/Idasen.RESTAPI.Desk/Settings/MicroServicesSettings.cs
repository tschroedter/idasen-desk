using System.ComponentModel.DataAnnotations ;

namespace Idasen.RESTAPI.Desk.Settings ;

public class MicroServicesSettings
{
    [ Required ]
    public IList < MicroServiceSettings > MicroServices { get ; set ; } = Array.Empty < MicroServiceSettings > ( ) ;
}