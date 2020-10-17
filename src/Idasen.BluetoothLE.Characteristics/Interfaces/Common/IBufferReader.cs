using Windows.Storage.Streams ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    public interface IBufferReader
    {
        bool TryReadValue (
            [ NotNull ] IBuffer  buffer ,
            out         byte [ ] bytes ) ;
    }
}