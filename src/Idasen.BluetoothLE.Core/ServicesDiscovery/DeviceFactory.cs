using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <inheritdoc />
    public class DeviceFactory
        : IDeviceFactory
    {
        public DeviceFactory (
            [ JetBrains.Annotations.NotNull ] Device.Factory                   deviceFactory ,
            [ JetBrains.Annotations.NotNull ] IBluetoothLeDeviceWrapperFactory deviceWrapperFactory )
        {
            Guard.ArgumentNotNull ( deviceFactory ,
                                    nameof ( deviceFactory ) ) ;
            Guard.ArgumentNotNull ( deviceWrapperFactory ,
                                    nameof ( deviceWrapperFactory ) ) ;

            _deviceFactory        = deviceFactory ;
            _deviceWrapperFactory = deviceWrapperFactory ;
        }

        /// <inheritdoc />
        [ ExcludeFromCodeCoverage ]
        public async Task < IDevice > FromBluetoothAddressAsync ( ulong address )
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync ( address ) ;

            return _deviceFactory ( _deviceWrapperFactory.Create ( device ) ) ;
        }

        private readonly Device.Factory                   _deviceFactory ;
        private readonly IBluetoothLeDeviceWrapperFactory _deviceWrapperFactory ;
    }
}