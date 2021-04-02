using System ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class MatchMaker
        : IMatchMaker
    {
        public MatchMaker ( [ NotNull ] ILogger               logger ,
                            [ NotNull ] IOfficialGattServices bluetoothGattServices ,
                            [ NotNull ] IDeviceFactory        deviceFactory )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( bluetoothGattServices ,
                                    nameof ( bluetoothGattServices ) ) ;
            Guard.ArgumentNotNull ( deviceFactory ,
                                    nameof ( deviceFactory ) ) ;

            _logger        = logger ;
            _deviceFactory = deviceFactory ;
        }

        /// <summary>
        ///     Attempts to pair to BLE device by address.
        /// </summary>
        /// <param name="address">The BLE device address.</param>
        /// <returns></returns>
        public async Task < IDevice > PairToDeviceAsync ( ulong address )
        {
            var device = await _deviceFactory.FromBluetoothAddressAsync ( address ) ;

            var macAddress = address.ToMacAddress ( ) ;

            if ( device == null )
            {
                var message = $"Failed to find device with MAC Address '{macAddress}' " +
                              $"(Address {address})" ;

                throw new ArgumentNullException ( message ) ;
            }

            _logger.Information ( $"[{macAddress}] DeviceId after FromBluetoothAddressAsync: {device.Id}" ) ;
            _logger.Information ( $"[{macAddress}] ConnectionStatus after FromBluetoothAddressAsync: {device.ConnectionStatus}" ) ;

            return device ;
        }

        private readonly IDeviceFactory _deviceFactory ;
        private readonly ILogger        _logger ;
    }
}