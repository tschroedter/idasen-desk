using System ;
using System.Collections ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.Tests ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ AutoDataTestClass ]
    public class RawValueReaderTests
    {
        [ AutoDataTestMethod ]
        public void TryReadValueAsync_ForCharacteristicIsNull_Throws (
            RawValueReader sut )
        {
            Func < Task > action = async ( ) => { await sut.TryReadValueAsync ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "characteristic" ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForNotSupportingRead_False (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithReadValueAsyncResult ( result ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForNotSupportingRead_Empty (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithReadValueAsyncResult ( result ) ;

            var (_ , bytes) = await sut.TryReadValueAsync ( characteristic ) ;

            bytes.Should ( )
                 .BeEmpty ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsSuccess_True (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( result ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsSuccess_Bytes (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic ,
            byte [ ]                   bytes )
        {
            WithTryReadValueResult ( reader ,
                                     bytes ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( result ) ;

            var (_ , value) = await sut.TryReadValueAsync ( characteristic ) ;

            value.Should ( )
                 .BeEquivalentTo ( bytes ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsNotSuccess_False (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Unreachable ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( result ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsNotSuccess_Empty (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( result ) ;

            var (_ , value) = await sut.TryReadValueAsync ( characteristic ) ;

            value.Should ( )
                 .BeEmpty ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatus_SetsProtocolError (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResult            result ,
            IGattCharacteristicWrapper characteristic ,
            byte                       protocolError )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            result.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            result.ProtocolError
                  .Returns ( protocolError ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( result ) ;

            _ = await sut.TryReadValueAsync ( characteristic ) ;

            sut.ProtocolError
               .Should ( )
               .Be ( protocolError ) ;
        }

        private void WithTryReadValueResult ( IBufferReader reader ,
                                              IEnumerable   bytes )
        {
            reader.TryReadValue ( Arg.Any < IBuffer > ( ) ,
                                  out var _ )
                  .Returns ( x =>
                             {
                                 x [ 1 ] = bytes ;

                                 return true ;
                             } ) ;
        }
    }
}