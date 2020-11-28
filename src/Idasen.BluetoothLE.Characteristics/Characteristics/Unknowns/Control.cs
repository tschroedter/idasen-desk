using System.Collections.Generic ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class Control
        : UnknownBase , IControl
    {
        public IEnumerable < byte > RawControl2 { get ; } = RawArrayEmpty ;
        public IEnumerable < byte > RawControl3 { get ; } = RawArrayEmpty ;

        public Task < bool > TryWriteRawControl2 ( IEnumerable < byte > bytes )
        {
            return Task.FromResult ( false ) ;
        }
    }
}