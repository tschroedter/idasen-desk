using System ;
using Windows.Devices.Bluetooth ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class DeviceTests
    {
        [ TestMethod ]
        public void Constructor_ForInvoked_SetsConnectionStatusChanged ( )
        {
            CreateSut ( ).ConnectionStatusChanged
                         .Should ( )
                         .BeNull ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsGattServicesRefreshed ( )
        {
            CreateSut ( ).GattServicesRefreshed
                         .Should ( )
                         .BeNull ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsGattCommunicationStatus ( )
        {
            CreateSut ( ).GattCommunicationStatus
                         .Should ( )
                         .Be ( GattCommunicationStatus.Unreachable ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsName ( )
        {
            CreateSut ( ).Name
                         .Should ( )
                         .Be ( Device.UnknownDeviceName ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsDeviceId ( )
        {
            CreateSut ( ).DeviceId
                         .Should ( )
                         .Be ( Device.UnknownDeviceId ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsIsPaired ( )
        {
            CreateSut ( ).IsPaired
                         .Should ( )
                         .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsGattServices ( )
        {
            CreateSut ( ).GattServices
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsConnectionStatus ( )
        {
            CreateSut ( ).ConnectionStatus
                         .Should ( )
                         .Be ( BluetoothConnectionStatus.Disconnected ) ;
        }

        [ TestMethod ]
        public void Connect_ForInvoked_DoesNothing ( )
        {
            Action action = ( ) => CreateSut ( ).Connect ( ) ;

            action.Should ( )
                  .NotThrow < Exception > ( ) ;
        }

        [TestMethod]
        public void Dispose_ForInvoked_DoesNothing()
        {
            Action action = () => CreateSut().Dispose (  );

            action.Should()
                  .NotThrow<Exception>();
        }

        private Device CreateSut ( )
        {
            return new Device ( ) ;
        }
    }
}