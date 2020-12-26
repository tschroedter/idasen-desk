using System ;
using Windows.Devices.Bluetooth ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ AutoDataTestClass ]
    public class DeviceTests
    {
        [ AutoDataTestMethod ]
        public void ConnectionStatusChanged_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.ConnectionStatusChanged
               .Should ( )
               .Be ( wrapper.ConnectionStatusChanged ) ;
        }

        [ AutoDataTestMethod ]
        public void GattCommunicationStatus_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.GattCommunicationStatus
               .Should ( )
               .Be ( wrapper.GattCommunicationStatus ) ;
        }

        [ AutoDataTestMethod ]
        public void Connect_ForConnected_DoesNotCallConnect (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            wrapper.ConnectionStatus
                   .Returns ( BluetoothConnectionStatus.Connected ) ;

            sut.Connect ( ) ;

            wrapper.DidNotReceive ( )
                   .Connect ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Connect_ForDisconnected_DoesCallConnect (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            wrapper.ConnectionStatus
                   .Returns ( BluetoothConnectionStatus.Disconnected ) ;

            sut.Connect ( ) ;

            wrapper.Received ( )
                   .Connect ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Name_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.Name
               .Should ( )
               .Be ( wrapper.Name ) ;
        }

        [ AutoDataTestMethod ]
        public void DeviceId_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.Id
               .Should ( )
               .Be ( wrapper.DeviceId ) ;
        }

        [ AutoDataTestMethod ]
        public void IsPaired_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.IsPaired
               .Should ( )
               .Be ( wrapper.IsPaired ) ;
        }

        [ AutoDataTestMethod ]
        public void ConnectionStatus_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.ConnectionStatus
               .Should ( )
               .Be ( wrapper.ConnectionStatus ) ;
        }

        [ AutoDataTestMethod ]
        public void GattServices_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.GattServices
               .Should ( )
               .BeSameAs ( wrapper.GattServices ) ;
        }

        [ AutoDataTestMethod ]
        public void GattServicesRefreshed_ForInvoked_Instance (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.GattServicesRefreshed
               .Should ( )
               .BeSameAs ( wrapper.GattServicesRefreshed ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_DisposesWrapper (
            Device                               sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper wrapper )
        {
            sut.Dispose ( ) ;

            wrapper.Received ( )
                   .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_DisposesSubscriber (
            Lazy < Device >                           sut ,
            [ Freeze ] IBluetoothLeDeviceWrapper      wrapper ,
            [ Freeze ] IDisposable                    subscriber ,
            IObservable < BluetoothConnectionStatus > status )
        {
            status.Subscribe ( _ => { } )
                  .ReturnsForAnyArgs ( subscriber ) ;

            wrapper.ConnectionStatusChanged
                   .Returns ( status ) ;

            sut.Value
               .Dispose ( ) ;

            subscriber.Received ( )
                      .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void BluetoothAddress_ForInvoked_BluetoothAddress (
            Device           sut ,
            [ Freeze ] ulong bluetoothAddress )
        {
            sut.BluetoothAddress
               .Should ( )
               .Be ( bluetoothAddress ) ;
        }

        [ AutoDataTestMethod ]
        public void BluetoothAddressType_ForInvoked_BluetoothAddressType (
            Device            sut ,
            [ Freeze ] string bluetoothAddressType )
        {
            sut.BluetoothAddressType
               .Should ( )
               .Be ( bluetoothAddressType ) ;
        }
    }
}