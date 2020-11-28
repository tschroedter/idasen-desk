using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    public interface IRawValueReader
    {
        /// <summary>
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        Task < (bool , byte [ ]) > TryReadValueAsync (
            [ NotNull ] IGattCharacteristicWrapper characteristic ) ;
    }
}