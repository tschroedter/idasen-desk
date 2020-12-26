using System ;
using Windows.Devices.Bluetooth ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns
{
    [ TestClass ]
    public class DeviceTests
    {
        [ TestMethod ]
        public void ConnectionStatusChanged_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).ConnectionStatusChanged
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void GattServicesRefreshed_ForInvoked_Throws ( )
        {
            Action action = ( ) => CreateSut ( ).GattServicesRefreshed
                                                .Subscribe ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public void GattCommunicationStatus_ForInvoked_Unreachable ( )
        {
            CreateSut ( ).GattCommunicationStatus
                         .Should ( )
                         .Be ( GattCommunicationStatus.Unreachable ) ;
        }

        [ TestMethod ]
        public void Name_ForInvoked_UnknownName ( )
        {
            CreateSut ( ).Name
                         .Should ( )
                         .Be ( Device.UnknownName ) ;
        }

        [ TestMethod ]
        public void Id_ForInvoked_UnknownId ( )
        {
            CreateSut ( ).Id
                         .Should ( )
                         .Be ( Device.UnknownId ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_False ( )
        {
            CreateSut ( ).IsPaired
                         .Should ( )
                         .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void GattServices_ForInvoked_Empty ( )
        {
            CreateSut ( ).GattServices
                         .Should ( )
                         .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void ConnectionStatus_ForInvoked_Disconnected ( )
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

        [ TestMethod ]
        public void Dispose_ForInvoked_DoesNothing ( )
        {
            Action action = ( ) => CreateSut ( ).Dispose ( ) ;

            action.Should ( )
                  .NotThrow < Exception > ( ) ;
        }

        private Device CreateSut ( )
        {
            return new Device ( ) ;
        }
    }
}