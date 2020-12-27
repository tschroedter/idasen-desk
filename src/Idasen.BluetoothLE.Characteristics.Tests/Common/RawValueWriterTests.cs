using System ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ AutoDataTestClass ]
    public class RawValueWriterTests
    {
        [ AutoDataTestMethod ]
        public void TryWriteValueAsync_ForCharacteristicIsNull_Throws (
            RawValueWriter sut ,
            IBuffer        buffer )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWriteValueAsync ( null! ,
                                                                      buffer ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "characteristic" ) ;
        }

        [ AutoDataTestMethod ]
        public void TryWriteValueAsync_ForBufferIsNull_Throws (
            RawValueWriter             sut ,
            IGattCharacteristicWrapper characteristic )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWriteValueAsync ( characteristic ,
                                                                      null! ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "buffer" ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForGattCommunicationStatusIsSuccess_True (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Write )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.Success ) ;

            var actual = await sut.TryWriteValueAsync ( characteristic ,
                                                        buffer ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForGattCommunicationStatusIsUnreachable_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Write )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.Unreachable ) ;

            var actual = await sut.TryWriteValueAsync ( characteristic ,
                                                        buffer ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForGattCommunicationStatusIsProtocolError_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Write )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.ProtocolError ) ;

            var actual = await sut.TryWriteValueAsync ( characteristic ,
                                                        buffer ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForGattCommunicationStatusIsAccessDenied_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Write )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.AccessDenied ) ;

            var actual = await sut.TryWriteValueAsync ( characteristic ,
                                                        buffer ) ;
            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForCharacteristicDoesNotSupportWrite_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.Write )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.AccessDenied ) ;

            var actual = await sut.TryWriteValueAsync ( characteristic ,
                                                        buffer ) ;
            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteValueAsync_ForCharacteristicDoesNotSupportWrite_DoesNotCallTryWriteValueAsync (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None ) ;

            await sut.TryWriteValueAsync ( characteristic ,
                                           buffer ) ;

            await characteristic.DidNotReceive ( )
                                .WriteValueAsync ( buffer ) ;
        }

        [ AutoDataTestMethod ]
        public void TryWritableAuxiliariesValueAsync_ForCharacteristicIsNull_Throws (
            RawValueWriter sut ,
            IBuffer        buffer )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWritableAuxiliariesValueAsync ( null! ,
                                                                                    buffer ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "characteristic" ) ;
        }

        [ AutoDataTestMethod ]
        public void TryWritableAuxiliariesValueAsync_ForBufferIsNull_Throws (
            RawValueWriter             sut ,
            IGattCharacteristicWrapper characteristic )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                                    null! ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "buffer" ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWritableAuxiliariesValueAsync_ForGattCommunicationStatusIsSuccess_True (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WritableAuxiliaries )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.Success ) ;

            var actual = await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                      buffer ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWritableAuxiliariesValueAsync_ForGattCommunicationStatusIsUnreachable_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WritableAuxiliaries )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.Unreachable ) ;

            var actual = await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                      buffer ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWritableAuxiliariesValueAsync_ForGattCommunicationStatusIsProtocolError_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WritableAuxiliaries )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.ProtocolError ) ;

            var actual = await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                      buffer ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWritableAuxiliariesValueAsync_ForGattCommunicationStatusIsAccessDenied_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WritableAuxiliaries )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.AccessDenied ) ;

            var actual = await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                      buffer ) ;
            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWritableAuxiliariesValueAsync_ForCharacteristicDoesNotSupportWrite_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithWriteValueAsyncResult ( GattCommunicationStatus.AccessDenied ) ;

            var actual = await sut.TryWritableAuxiliariesValueAsync ( characteristic ,
                                                                      buffer ) ;
            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public async Task
            TryWritableAuxiliariesValueAsync_ForCharacteristicDoesNotSupportWritableAuxiliaries_DoesNotCallTryWriteValueAsync (
                RawValueWriter                        sut ,
                [ Freeze ] IGattCharacteristicWrapper characteristic ,
                IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None ) ;

            await sut.TryWriteValueAsync ( characteristic ,
                                           buffer ) ;

            await characteristic.DidNotReceive ( )
                                .WriteValueAsync ( buffer ) ;
        }

        // --
        [ AutoDataTestMethod ]
        public void TryWriteWithoutResponseAsync_ForCharacteristicIsNull_Throws (
            RawValueWriter sut ,
            IBuffer        buffer )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWriteWithoutResponseAsync ( null! ,
                                                                                buffer ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "characteristic" ) ;
        }

        [ AutoDataTestMethod ]
        public void TryWriteWithoutResponseAsync_ForBufferIsNull_Throws (
            RawValueWriter             sut ,
            IGattCharacteristicWrapper characteristic )
        {
            Func < Task > action = async ( ) =>
                                   {
                                       await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                                null! ) ;
                                   } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "buffer" ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteWithoutResponseAsync_ForGattCommunicationStatusIsSuccess_True (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer ,
            IGattWriteResultWrapper                      result )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WriteWithoutResponse )
                          .WithWriteValueWithResultAsync ( result ) ;

            var actual = await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                  buffer ) ;

            actual.Should ( )
                  .Be ( result ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteWithoutResponseAsync_ForGattCommunicationStatusIsUnreachable_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer ,
            IGattWriteResultWrapper                      result )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WriteWithoutResponse )
                          .WithWriteValueWithResultAsync ( result ) ;

            var actual = await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                  buffer ) ;

            actual.Should ( )
                  .Be ( result ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteWithoutResponseAsync_ForGattCommunicationStatusIsProtocolError_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer ,
            IGattWriteResultWrapper                      result )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WriteWithoutResponse )
                          .WithWriteValueWithResultAsync ( result ) ;

            var actual = await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                  buffer ) ;

            actual.Should ( )
                  .Be ( result ) ;
        }

        [ AutoDataTestMethod ]
        public async Task TryWriteWithoutResponseAsync_ForGattCommunicationStatusIsAccessDenied_False (
            RawValueWriter                        sut ,
            [ Freeze ] IGattCharacteristicWrapper characteristic ,
            IBuffer                               buffer ,
            IGattWriteResultWrapper                      result )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.WriteWithoutResponse )
                          .WithWriteValueWithResultAsync ( result ) ;

            var actual = await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                  buffer ) ;
            actual.Should ( )
                  .Be ( result ) ;
        }

        [ AutoDataTestMethod ]
        public async Task
            TryWriteWithoutResponseAsync_ForCharacteristicDoesNotWriteWithoutResponse_GattWriteResultNotSupported (
                RawValueWriter                        sut ,
                [ Freeze ] IGattCharacteristicWrapper characteristic ,
                IBuffer                               buffer ,
                IGattWriteResultWrapper                      result )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None )
                          .WithWriteValueWithResultAsync ( result ) ;

            var actual = await sut.TryWriteWithoutResponseAsync ( characteristic ,
                                                                  buffer ) ;
            actual.Should ( )
                  .Be ( GattWriteResultWrapper.NotSupported ) ;
        }

        [ AutoDataTestMethod ]
        public async Task
            TryWriteWithoutResponseAsync_ForCharacteristicDoesNotSupportWrite_DoesNotCallTryWriteValueAsync (
                RawValueWriter                        sut ,
                [ Freeze ] IGattCharacteristicWrapper characteristic ,
                IBuffer                               buffer )
        {
            characteristic.WithCharacteristicProperties ( GattCharacteristicProperties.None ) ;

            await sut.TryWriteValueAsync ( characteristic ,
                                           buffer ) ;

            await characteristic.DidNotReceive ( )
                                .WriteValueAsync ( buffer ) ;
        }
    }
}