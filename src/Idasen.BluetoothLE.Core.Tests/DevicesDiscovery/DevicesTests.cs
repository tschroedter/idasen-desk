using System ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ AutoDataTestClass ]
    public class DevicesTests
    {
        [ AutoDataTestMethod ]
        public void Constructor_ForLoggerNull_Throws (
            Lazy < Devices >   sutLazy ,
            [ BeNull ] ILogger logger )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sutLazy.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( logger ) ) ;
        }

        [ AutoDataTestMethod ]
        public void DiscoveredDevices_ForInitialized_IsEmpty ( Devices sut )
        {
            sut.DiscoveredDevices
               .Should ( )
               .BeEmpty ( "Should be empty when created" ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForDeviceIsNull_Throws (
            Devices sut )
        {
            Action action = ( ) => { sut.AddOrUpdateDevice ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "device" ) ;
        }

        [ AutoDataTestMethod ]
        public void RemoveDevice_ForDeviceIsNull_Throws (
            Devices sut )
        {
            Action action = ( ) => { sut.RemoveDevice ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "device" ) ;
        }

        [ AutoDataTestMethod ]
        public void ContainsDevice_ForDeviceIsNull_Throws (
            Devices sut )
        {
            Action action = ( ) => { sut.ContainsDevice ( null! ) ; } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "device" ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForNewDeviceAdded_IncreasesCount (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.DiscoveredDevices
               .Count
               .Should ( )
               .Be ( 1 ) ;
        }

        [ AutoDataTestMethod ]
        public void Remove_ForExistingDevice_RemovesDevice (
            Devices        sut ,
            IDevice        device ,
            DeviceComparer comparer )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.RemoveDevice ( device ) ;

            sut.DiscoveredDevices
               .Should ( )
               .NotContain ( x => comparer.Equals ( x ,
                                                    device ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Remove_ForExistingDevice_DoesNotRemovesOtherDevice (
            Devices              sut ,
            [ Populate ] IDevice device1 ,
            [ Populate ] IDevice device2 ,
            DeviceComparer       comparer )
        {
            sut.AddOrUpdateDevice ( device1 ) ;
            sut.AddOrUpdateDevice ( device2 ) ;

            sut.RemoveDevice ( device1 ) ;

            using var scope = new AssertionScope ( ) ;

            sut.DiscoveredDevices
               .Should ( )
               .NotContain ( x => comparer.Equals ( x ,
                                                    device1 ) ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device2 ) ) ;
        }


        [ AutoDataTestMethod ]
        public void Remove_ForExistingDevice_DecreasesCount (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.RemoveDevice ( device ) ;

            sut.DiscoveredDevices
               .Count
               .Should ( )
               .Be ( 0 ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForNewDeviceAdded_DeviceAdded (
            Devices        sut ,
            IDevice        device ,
            DeviceComparer comparer )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device ) ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForTwoNewDevicesAdded_IncreasesCount (
            Devices              sut ,
            [ Populate ] IDevice device1 ,
            [ Populate ] IDevice device2 )
        {
            sut.AddOrUpdateDevice ( device1 ) ;
            sut.AddOrUpdateDevice ( device2 ) ;

            sut.DiscoveredDevices
               .Count
               .Should ( )
               .Be ( 2 ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForTwoNewDevicesAdded_DevicesAdded (
            Devices              sut ,
            [ Populate ] IDevice device1 ,
            [ Populate ] IDevice device2 ,
            DeviceComparer       comparer )
        {
            sut.AddOrUpdateDevice ( device1 ) ;
            sut.AddOrUpdateDevice ( device2 ) ;

            using var scope = new AssertionScope ( ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device1 ) ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device2 ) ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForSameDeviceAddedTwice_CountStaysTheSame (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;
            sut.AddOrUpdateDevice ( device ) ;

            sut.DiscoveredDevices
               .Count
               .Should ( )
               .Be ( 1 ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForSameDeviceAddedTwice_UpdatesDevice (
            Devices        sut ,
            IDevice        device1 ,
            IDevice        device2 ,
            DeviceComparer comparer )
        {
            device2.Address
                   .Returns ( device1.Address ) ;

            sut.AddOrUpdateDevice ( device1 ) ;
            sut.AddOrUpdateDevice ( device2 ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device2 ) ) ;
        }

        [ AutoDataTestMethod ]
        public void AddOrUpdateDevice_ForDeviceWithEmptyName_UpdatesDeviceName (
            Devices        sut ,
            IDevice        device1 ,
            IDevice        device2 ,
            DeviceComparer comparer )
        {
            device1.Name
                   .Returns ( string.Empty ) ;

            var address = device1.Address ;

            device2.Address
                   .Returns ( address ) ;

            sut.AddOrUpdateDevice ( device1 ) ;
            sut.AddOrUpdateDevice ( device2 ) ;

            sut.DiscoveredDevices
               .Should ( )
               .ContainSingle ( x => comparer.Equals ( x ,
                                                       device2 ) ) ;
        }

        [ AutoDataTestMethod ]
        public void ContainsDevice_ForExistingDevice_ReturnsTrue (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.ContainsDevice ( device )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void ContainsDevice_ForNotExistingDevice_ReturnsFalse (
            Devices sut ,
            IDevice device )
        {
            sut.ContainsDevice ( device )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void TryGetDevice_ForNotExistingDevice_ReturnsFalse (
            Devices sut )
        {
            sut.TryGetDevice ( 0ul ,
                               out _ )
               .Should ( )
               .BeFalse ( ) ;
        }

        [ AutoDataTestMethod ]
        public void TryGetDevice_ForExistingDevice_ReturnsTrue (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.TryGetDevice ( device.Address ,
                               out _ )
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void TryGetDevice_ForExistingDevice_ReturnsDevice (
            Devices        sut ,
            IDevice        device ,
            DeviceComparer comparer )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.TryGetDevice ( device.Address ,
                               out var actual ) ;

            comparer.Equals ( actual ,
                              device )
                    .Should ( )
                    .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Clear_ForInvoked_ClearsDiscoveredDevices (
            Devices sut ,
            IDevice device )
        {
            sut.AddOrUpdateDevice ( device ) ;

            sut.Clear ( ) ;

            sut.DiscoveredDevices
               .Should ( )
               .BeEmpty ( ) ;
        }
    }
}