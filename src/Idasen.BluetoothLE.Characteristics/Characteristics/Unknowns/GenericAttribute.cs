using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class GenericAttribute
        : UnknownBase , IGenericAttribute
    {
        public IEnumerable < byte > RawServiceChanged { get ; } = RawArrayEmpty ;
    }
}