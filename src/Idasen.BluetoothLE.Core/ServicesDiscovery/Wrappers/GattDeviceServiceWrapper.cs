using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    public class GattDeviceServiceWrapper
        : IGattDeviceServiceWrapper
    {
        public GattDeviceServiceWrapper (
            [ JetBrains.Annotations.NotNull ] IGattCharacteristicsResultWrapperFactory characteristicsFactory ,
            [ JetBrains.Annotations.NotNull ] GattDeviceService                        gattDeviceService )
        {
            Guard.ArgumentNotNull ( characteristicsFactory ,
                                    nameof ( characteristicsFactory ) ) ;
            Guard.ArgumentNotNull ( gattDeviceService ,
                                    nameof ( gattDeviceService ) ) ;

            _characteristicsFactory = characteristicsFactory ;
            _gattDeviceService      = gattDeviceService ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _gattDeviceService.Dispose ( ) ;
        }

        /// <inheritdoc />
        public Guid Uuid => _gattDeviceService.Uuid ;

        /// <inheritdoc />
        public string DeviceId => _gattDeviceService.DeviceId ;

        /// <inheritdoc />
        public Task < IGattCharacteristicsResultWrapper > GetCharacteristicsAsync ( )
        {
            var characteristics = _gattDeviceService.GetCharacteristicsAsync ( ).AsTask ( ) ;

            var result = _characteristicsFactory.Create ( characteristics.Result )
                                                .Initialize ( ) ;

            return result ;
        }

        private readonly IGattCharacteristicsResultWrapperFactory _characteristicsFactory ;
        private readonly GattDeviceService                        _gattDeviceService ;
    }
}