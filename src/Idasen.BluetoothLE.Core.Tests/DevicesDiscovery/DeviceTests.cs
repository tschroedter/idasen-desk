using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ TestClass ]
    public class DeviceTests
    {
        private const ulong  Address                = 197530862419747 ;
        private const string Name                   = "Name" ;
        private const short  RawSignalStrengthInDBm = - 50 ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            var dateTimeOffset = DateTimeOffset.Parse ( "2007-10-02T13:02:03.0000000-07:30" ) ;
            _broadcastTime = new DateTimeOffsetWrapper ( dateTimeOffset ) ;

            _comparer = new DeviceComparer ( ) ;
        }

        [ TestMethod ]
        public void Constructor_ForDeviceIsNull_Throws ( )
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = ( ) => { new Device ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "device" ) ;
        }

        [ TestMethod ]
        public void Constructor_ForBroadcastTimeNull_Throws ( )
        {
            _broadcastTime = null ;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = ( ) => { CreateSut ( ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "broadcastTime" ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsBroadcastTime ( )
        {
            var sut = CreateSut ( ) ;

            sut.BroadcastTime
               .Should ( )
               .Be ( _broadcastTime ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsAddress ( )
        {
            var sut = CreateSut ( ) ;

            sut.Address
               .Should ( )
               .Be ( Address ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsName ( )
        {
            var sut = CreateSut ( ) ;

            sut.Name
               .Should ( )
               .Be ( Name ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsRawSignalStrengthInDBm ( )
        {
            var sut = CreateSut ( ) ;

            sut.RawSignalStrengthInDBm
               .Should ( )
               .Be ( RawSignalStrengthInDBm ) ;
        }

        [ TestMethod ]
        public void ToString_ForInvoked_ReturnsInstance ( )
        {
            var sut = CreateSut ( ) ;

            sut.ToString ( )
               .Should ( )
               .Be ( "Name = Name, "                                       +
                     "MacAddress = B3:A7:3C:E2:FF:23, "                    +
                     "Address = 197530862419747, "                         +
                     "BroadcastTime = 2007-10-02T13:02:03.0000000-07:30, " +
                     "RawSignalStrengthInDBm = -50dB" ) ;
        }

        [ TestMethod ]
        public void Constructor_ForIDevice_ReturnsInstance ( )
        {
            var device = CreateSut ( ) ;

            var sut = new Device ( device ) ;

            _comparer.Equals ( sut ,
                               device )
                     .Should ( )
                     .BeTrue ( ) ;
        }

        private Device CreateSut ( )
        {
            return new Device ( _broadcastTime ,
                                Address ,
                                Name ,
                                RawSignalStrengthInDBm ) ;
        }

        private IDateTimeOffset _broadcastTime ;
        private DeviceComparer  _comparer ;
    }
}