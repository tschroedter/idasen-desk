using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class ReferenceInput
        : UnknownBase , IReferenceInput
    {
        public IEnumerable < byte > Ctrl1 { get ; } = RawArrayEmpty ;
    }
}