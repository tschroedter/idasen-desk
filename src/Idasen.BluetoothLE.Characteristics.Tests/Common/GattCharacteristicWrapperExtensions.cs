using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    public static class GattCharacteristicWrapperExtensions
    {
        public static IGattCharacteristicWrapper WithCharacteristicProperties (
            this IGattCharacteristicWrapper characteristic ,
            GattCharacteristicProperties    properties )
        {
            _ = characteristic.CharacteristicProperties
                              .Returns ( properties ) ;

            return characteristic ;
        }

        public static IGattCharacteristicWrapper WithWriteValueAsyncResult (
            this IGattCharacteristicWrapper characteristic ,
            GattCommunicationStatus         status )
        {
            _ = characteristic.WriteValueAsync ( Arg.Any < IBuffer > ( ) )
                              .Returns ( Task.FromResult ( status ) ) ;

            return characteristic ;
        }

        public static IGattCharacteristicWrapper WithWriteValueWithResultAsync (
            this IGattCharacteristicWrapper characteristic ,
            IGattWriteResultWrapper         result )
        {
            _ = characteristic.WriteValueWithResultAsync ( Arg.Any < IBuffer > ( ) )
                              .Returns ( Task.FromResult ( result ) ) ;

            return characteristic ;
        }

        public static IGattCharacteristicWrapper WithReadValueAsyncResult (
            this IGattCharacteristicWrapper characteristic ,
            IGattReadResultWrapper          result )
        {
            _ = characteristic.ReadValueAsync ( )
                              .Returns ( Task.FromResult ( result ) ) ;

            return characteristic ;
        }
    }
}