using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class RawValueWriter
        : IRawValueWriter
    {
        public async Task < bool > TryWriteValueAsync (
            [ NotNull ] IGattCharacteristicWrapper characteristic ,
            [ NotNull ] IBuffer                    buffer )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;
            Guard.ArgumentNotNull ( buffer ,
                                    nameof ( buffer ) ) ;

            if ( SupportsWrite ( characteristic ) )
            {
                Log.Information ( $"GattCharacteristic '{characteristic.Uuid}' " +
                                  "doesn't support 'Write/WritableAuxiliaries/"  +
                                  "WriteWithoutResponse'" ) ;

                return false ;
            }

            var status = await characteristic.WriteValueAsync ( buffer ) ;

            return status == GattCommunicationStatus.Success ;
        }

        public async Task < bool > TryWritableAuxiliariesValueAsync (
            [ NotNull ] IGattCharacteristicWrapper characteristic ,
            [ NotNull ] IBuffer                    buffer )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;
            Guard.ArgumentNotNull ( buffer ,
                                    nameof ( buffer ) ) ;

            if ( SupportsWritableAuxiliaries ( characteristic ) )
            {
                Log.Information ( $"GattCharacteristic '{characteristic.Uuid}' " +
                                  "doesn't support 'Write/WritableAuxiliaries/"  +
                                  "WriteWithoutResponse'" ) ;

                return false ;
            }

            var status = await characteristic.WriteValueAsync ( buffer ) ;

            return status == GattCommunicationStatus.Success ;
        }

        public async Task < IGattWriteResultWrapper > TryWriteWithoutResponseAsync (
            [ NotNull ] IGattCharacteristicWrapper characteristic ,
            [ NotNull ] IBuffer                    buffer )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;
            Guard.ArgumentNotNull ( buffer ,
                                    nameof ( buffer ) ) ;

            if ( SupportsWriteWithoutResponse ( characteristic ) )
            {
                var message = $"GattCharacteristic '{characteristic.Uuid}' " +
                              "doesn't support 'Write/WritableAuxiliaries/"  +
                              "WriteWithoutResponse'" ;

                Log.Information ( message ) ;

                return GattWriteResultWrapper.NotSupported ;
            }

            var status = await characteristic.WriteValueWithResultAsync ( buffer ) ;

            return status ;
        }

        private static bool SupportsWrite ( IGattCharacteristicWrapper characteristic )
        {
            return ( characteristic.CharacteristicProperties & GattCharacteristicProperties.Write ) !=
                   GattCharacteristicProperties.Write ;
        }

        private static bool SupportsWritableAuxiliaries ( IGattCharacteristicWrapper characteristic )
        {
            return ( characteristic.CharacteristicProperties & GattCharacteristicProperties.WritableAuxiliaries ) !=
                   GattCharacteristicProperties.WritableAuxiliaries ;
        }

        private static bool SupportsWriteWithoutResponse ( IGattCharacteristicWrapper characteristic )
        {
            return ( characteristic.CharacteristicProperties & GattCharacteristicProperties.WriteWithoutResponse ) !=
                   GattCharacteristicProperties.WriteWithoutResponse ;
        }
    }
}