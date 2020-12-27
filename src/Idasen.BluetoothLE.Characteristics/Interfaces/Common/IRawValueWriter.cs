using System.Threading.Tasks ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

// ReSharper disable UnusedMemberInSuper.Global

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    public interface IRawValueWriter
    {
        Task < bool > TryWriteValueAsync (
            IGattCharacteristicWrapper characteristic ,
            IBuffer                    buffer ) ;

        Task < bool > TryWritableAuxiliariesValueAsync (
            IGattCharacteristicWrapper characteristic ,
            IBuffer                    buffer ) ;

        Task < IGattWriteResultWrapper > TryWriteWithoutResponseAsync (
            IGattCharacteristicWrapper characteristic ,
            IBuffer                    buffer ) ;
    }
}