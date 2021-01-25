using System.Linq ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class RawValueReader
        : IRawValueReader
    {
        public RawValueReader (
            [ NotNull ] ILogger       logger ,
            [ NotNull ] IBufferReader reader )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( reader ,
                                    nameof ( reader ) ) ;

            _logger = logger ;
            _reader = reader ;
        }

        public async Task < (bool , byte [ ]) > TryReadValueAsync (
            IGattCharacteristicWrapper characteristic )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;

            if ( SupportsNotify ( characteristic ) )
            {
                _logger.Warning ( $"GattCharacteristic '{characteristic.Uuid}' " +
                                  "doesn't support 'Read' but supports 'Notify'" ) ;

                return ( false , ArrayEmpty ) ; // need to subscribe to value change
            }

            if ( SupportsRead ( characteristic ) )
                return await ReadValue ( characteristic ) ;

            _logger.Information ( $"GattCharacteristic '{characteristic.Uuid}' " +
                                  "doesn't support 'Read'" ) ;

            return ( false , ArrayEmpty ) ;
        }

        private static readonly byte [ ] ArrayEmpty = Enumerable.Empty < byte > ( )
                                                                .ToArray ( ) ;

        public byte?                   ProtocolError { get ; private set ; }
        public GattCommunicationStatus Status        { get ; private set ; } = GattCommunicationStatus.Unreachable ;

        private static bool SupportsRead ( IGattCharacteristicWrapper characteristic )
        {
            return ( characteristic.CharacteristicProperties & GattCharacteristicProperties.Read ) ==
                   GattCharacteristicProperties.Read ;
        }

        private static bool SupportsNotify ( IGattCharacteristicWrapper characteristic )
        {
            return ( characteristic.CharacteristicProperties & GattCharacteristicProperties.Notify ) ==
                   GattCharacteristicProperties.Notify ;
        }

        private async Task < (bool , byte [ ]) > ReadValue (
            [ NotNull ] IGattCharacteristicWrapper characteristic )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;

            var readValue = await characteristic.ReadValueAsync ( ) ;

            ProtocolError = readValue.ProtocolError ;
            Status        = readValue.Status ;

            if ( GattCommunicationStatus.Success != Status )
                return ( false , ArrayEmpty ) ;

            return _reader.TryReadValue ( readValue.Value ,
                                          out var bytes )
                       ? ( true , bytes )
                       : ( false , ArrayEmpty ) ;
        }

        private readonly ILogger       _logger ;
        private readonly IBufferReader _reader ;
    }
}