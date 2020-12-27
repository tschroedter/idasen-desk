using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Wrapper for <see cref="GattCharacteristic" /> .
    /// </summary>
    public interface IGattCharacteristicWrapper
        : IDisposable
    {
        /// <summary>
        ///     The Gatt Characteristic's UUID.
        /// </summary>
        Guid Uuid { get ; }

        /// <summary>
        ///     Get the Gatt Characteristic properties.
        /// </summary>
        GattCharacteristicProperties CharacteristicProperties { get ; }

        IReadOnlyList < GattPresentationFormat > PresentationFormats { get ; }

        Guid ServiceUuid { get ; }

        string UserDescription { get ; }

        GattProtectionLevel ProtectionLevel { get ; }

        ushort AttributeHandle { get ; }

        /// <summary>
        ///     Notifies when the Gatt Characteristic value has changed.
        /// </summary>
        IObservable < GattCharacteristicValueChangedDetails > ValueChanged { get ; }

        /// <summary>
        ///     Writes the given raw bytes.
        /// </summary>
        /// <param name="buffer">
        ///     The bytes to be written.
        /// </param>
        /// <returns>
        ///     Result of the write process.
        /// </returns>
        Task < IGattWriteResultWrapper > WriteValueWithResultAsync ( IBuffer buffer ) ;

        /// <summary>
        ///     Writes the given raw bytes async.
        /// </summary>
        /// <param name="buffer">
        ///     The bytes to be written.
        /// </param>
        /// <returns>
        ///     Result of the write process.
        /// </returns>
        Task < GattCommunicationStatus > WriteValueAsync ( IBuffer buffer ) ;

        /// <summary>
        ///     Performs a Characteristic Value read from the value cache
        ///     maintained by Windows.
        /// </summary>
        /// <returns>
        ///     The object required to manage the asynchronous operation,
        ///     which, upon completion, returns a GattReadResult object, which
        ///     in turn contains the completion status of the asynchronous
        ///     operation and, if successful, the data read from the device.
        /// </returns>
        Task < IGattReadResultWrapper > ReadValueAsync ( ) ;

        /// <summary>
        ///     Initializes the instance.
        /// </summary>
        /// <returns>
        ///     Itself.
        /// </returns>
        Task < IGattCharacteristicWrapper > Initialize ( ) ;
    }
}