using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    public interface IRawValueWriter
    {
        Task<bool> TryWriteValueAsync(
            IGattCharacteristicWrapper characteristic,
            IBuffer                    buffer);

        Task<bool> TryWritableAuxiliariesValueAsync(
            IGattCharacteristicWrapper characteristic,
            IBuffer                    buffer);

        Task<IGattWriteResult> TryWriteWithoutResponseAsync(
            IGattCharacteristicWrapper characteristic,
            IBuffer                    buffer);
    }
}