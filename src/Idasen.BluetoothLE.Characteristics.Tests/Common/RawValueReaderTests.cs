using System ;
using System.Collections ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
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
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForNotSupportingRead_Empty (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (_ , bytes) = await sut.TryReadValueAsync ( characteristic ) ;

            bytes.Should ( )
                 .BeEmpty ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsSuccess_True (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsSuccess_Bytes (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic ,
            byte [ ]                   bytes )
        {
            WithTryReadValueResult ( reader ,
                                     bytes ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (_ , value) = await sut.TryReadValueAsync ( characteristic ) ;

            value.Should ( )
                 .BeEquivalentTo ( bytes ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsNotSuccess_False (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Unreachable ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (success , _) = await sut.TryReadValueAsync ( characteristic ) ;

            success.Should ( )
                   .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatusIsNotSuccess_Empty (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

            var (_ , value) = await sut.TryReadValueAsync ( characteristic ) ;

            value.Should ( )
                 .BeEmpty ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryReadValueAsync_ForGattCommunicationStatus_SetsProtocolError (
            RawValueReader             sut ,
            [ Freeze ] IBufferReader   reader ,
            IGattReadResultWrapper            resultWrapper ,
            IGattCharacteristicWrapper characteristic ,
            byte                       protocolError )
        {
            WithTryReadValueResult ( reader ,
                                     Array.Empty < byte > ( ) ) ;

            resultWrapper.Status
                  .Returns ( GattCommunicationStatus.Success ) ;

            resultWrapper.ProtocolError
                  .Returns ( protocolError ) ;

            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Read )
                          .WithReadValueAsyncResult ( resultWrapper ) ;

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