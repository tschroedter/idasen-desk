using FluentAssertions ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using NSubstitute ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ AutoDataTestClass ]
    public class DeviceComparerTests
    {
        [ AutoDataTestMethod ]
        public void IsEquivalentTo_ForBothNull_ReturnsFalse (
            DeviceComparer sut )
        {
            sut.IsEquivalentTo ( null ,
                                 null )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_ForDeviceAIsNull_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        device )
        {
            sut.IsEquivalentTo ( null ,
                                 device )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_ForDeviceBIsNull_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        device )
        {
            sut.IsEquivalentTo ( device ,
                                 null )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_ForSameDevice_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        device )
        {
            sut.IsEquivalentTo ( device ,
                                 device )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_For2DeviceDifferentName_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       deviceA.Address ,
                                       "Other Name" ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.IsEquivalentTo ( deviceA ,
                                 deviceB )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_For2DeviceDifferentBroadcastTime_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            var deviceB = new Device ( new DateTimeOffsetWrapper ( ) ,
                                       deviceA.Address ,
                                       deviceA.Name ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.IsEquivalentTo ( deviceA ,
                                 deviceB )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_For2DeviceRawSignalStrengthInDBm_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            deviceA.RawSignalStrengthInDBm
                   .Returns ( ( short ) - 1 ) ;

            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       deviceA.Address ,
                                       deviceA.Name ,
                                       0 ) ;

            sut.IsEquivalentTo ( deviceA ,
                                 deviceB )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void IsEquivalentTo_For2DevicesDifferentAddress_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            deviceA.Address
                   .Returns ( 0ul ) ;

            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       1ul ,
                                       deviceA.Name ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.IsEquivalentTo ( deviceA ,
                                 deviceB )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_ForBothNull_ReturnsFalse (
            DeviceComparer sut )
        {
            sut.Equals ( null ,
                         null )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_ForDeviceAIsNull_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        device )
        {
            sut.Equals ( null ,
                         device )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_ForDeviceBIsNull_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        device )
        {
            sut.Equals ( device ,
                         null )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_ForTheSameDevice_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            sut.Equals ( deviceA ,
                         deviceA )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_For2DeviceDifferentName_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       deviceA.Address ,
                                       "Other Name" ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.Equals ( deviceA ,
                         deviceB )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_For2DeviceDifferentBroadcastTime_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            var deviceB = new Device ( new DateTimeOffsetWrapper ( ) ,
                                       deviceA.Address ,
                                       deviceA.Name ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.Equals ( deviceA ,
                         deviceB )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_For2DeviceRawSignalStrengthInDBm_ReturnsTrue (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            deviceA.RawSignalStrengthInDBm
                   .Returns ( ( short ) - 1 ) ;

            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       deviceA.Address ,
                                       deviceA.Name ,
                                       0 ) ;

            sut.Equals ( deviceA ,
                         deviceB )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Equals_For2DevicesDifferentAddress_ReturnsFalse (
            DeviceComparer sut ,
            IDevice        deviceA )
        {
            deviceA.Address
                   .Returns ( 0ul ) ;

            var deviceB = new Device ( deviceA.BroadcastTime ,
                                       1ul ,
                                       deviceA.Name ,
                                       deviceA.RawSignalStrengthInDBm ) ;

            sut.Equals ( deviceA ,
                         deviceB )
               .Should ( )
               .BeFalse ( ) ;
        }
    }
}