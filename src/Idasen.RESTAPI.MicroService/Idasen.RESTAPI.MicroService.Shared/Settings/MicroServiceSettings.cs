using System.ComponentModel.DataAnnotations ;

namespace Idasen.RESTAPI.MicroService.Shared.Settings ;

public class MicroServiceSettings
{
    [ Required ]
    public string Name { get ; set ; } = string.Empty ;

    [ Required ]
    public string Description { get ; set ; } = string.Empty ;

    [ Required ]
    public string Path { get ; set ; } = string.Empty ;

    [ Required ]
    public string Protocol { get ; set ; } = "Http" ;

    [ Required ]
    public string Host { get ; set ; } = "Unknown" ;

    [ Required ]
    [Range( 0,
            65535)]
    public int Port { get ; set ; } = 80 ;

    [ Required ]
    public string Readiness { get ; set ; } = string.Empty ;

    public override string ToString ( )
    {
        return "["                                          +
               $"{nameof ( Name )}: {Name}, "               +
               $"{nameof ( Description )}: {Description}, " +
               $"{nameof ( Path )}: {Path}, "               +
               $"{nameof ( Host )}: {Host}, "               +
               $"{nameof ( Port )}: {Port}, "               +
               $"{nameof ( Readiness )}: {Readiness}"       +
               "]" ;
    }
}