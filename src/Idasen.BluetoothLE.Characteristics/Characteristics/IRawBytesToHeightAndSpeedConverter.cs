using System.Collections.Generic ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public interface IRawBytesToHeightAndSpeedConverter
    {
        bool TryConvert(IEnumerable<byte> bytes,
                        out uint          height,
                        out int           speed) ;
    }
}