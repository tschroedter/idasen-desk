using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class Dpg
        : UnknownBase , IDpg
    {
        public IEnumerable < byte > RawDpg { get ; } = RawArrayEmpty ;
    }
}