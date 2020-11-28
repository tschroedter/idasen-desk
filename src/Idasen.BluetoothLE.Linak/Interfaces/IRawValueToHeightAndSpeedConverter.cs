using System.Collections.Generic ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IRawValueToHeightAndSpeedConverter
    {
        bool TryConvert ( IEnumerable < byte > bytes ,
                          out uint             height ,
                          out int              speed ) ;
    }
}