using Windows.Devices.Bluetooth.Advertisement ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ TestClass ]
    public class StatusMapperTests
    {
        [ DataTestMethod ]
        [ DataRow ( BluetoothLEAdvertisementWatcherStatus.Started ,
                    Status.Started ) ]
        [ DataRow ( BluetoothLEAdvertisementWatcherStatus.Aborted ,
                    Status.Aborted ) ]
        [ DataRow ( BluetoothLEAdvertisementWatcherStatus.Created ,
                    Status.Created ) ]
        [ DataRow ( BluetoothLEAdvertisementWatcherStatus.Stopped ,
                    Status.Stopped ) ]
        [ DataRow ( BluetoothLEAdvertisementWatcherStatus.Stopping ,
                    Status.Stopping ) ]
        public void Map_ForStatus_ReturnsWatcherStatus (
            BluetoothLEAdvertisementWatcherStatus bluetoothStatus ,
            Status                                status )
        {
            CreateSut ( ).Map ( bluetoothStatus )
                         .Should ( )
                         .Be ( status ) ;
        }

        private StatusMapper CreateSut ( )
        {
            var sut = new StatusMapper ( ) ;

            return sut ;
        }
    }
}