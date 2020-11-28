using System.Collections.Generic ;
using Idasen.BluetoothLE.Linak.Control ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskCommandsProvider
    {
        bool TryGetValue ( DeskCommands             command ,
                           out IEnumerable < byte > bytes ) ;
    }
}