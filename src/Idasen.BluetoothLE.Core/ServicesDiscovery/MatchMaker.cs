using System ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
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

            if ( device == null )
            {
                var message = $"Failed to find device with address '{address}'" ;

                throw new ArgumentNullException ( message ) ;
            }

            _logger.Information ( $"DeviceId after FromBluetoothAddressAsync: {device.Id}" ) ;
            _logger.Information ( $"ConnectionStatus after FromBluetoothAddressAsync: {device.ConnectionStatus}" ) ;

            return device ;
        }

        private readonly IDeviceFactory _deviceFactory ;
        private readonly ILogger        _logger ;
    }
}