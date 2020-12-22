using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ AutoDataTestClass ]
    public class DeviceMonitorWithExpiryTests
    {
        [ AutoDataTestMethod ]
        public void Constructor_ForLoggerNull_Throws (
            Lazy < DeviceMonitorWithExpiry > sut ,
            [ BeNull ] ILogger               logger )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( logger ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForDateTimeOffsetNull_Throws (
            Lazy < DeviceMonitorWithExpiry > sut ,
            [ BeNull ] IDateTimeOffset       dateTimeOffset )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( dateTimeOffset ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForDateTimeOffsetNull_Throws (
            Lazy < DeviceMonitorWithExpiry > sut ,
            [ BeNull ] IDeviceMonitor        deviceMonitor )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( deviceMonitor ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForFactoryNull_Throws (
            Lazy < DeviceMonitorWithExpiry >   sut ,
            [ BeNull ] IObservableTimerFactory factory )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( factory ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForSchedulerIsNull_Throws (
            Lazy < DeviceMonitorWithExpiry > sut ,
            [ BeNull ] IScheduler            scheduler )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( scheduler ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForDeviceExpiredNull_Throws (
            Lazy < DeviceMonitorWithExpiry > sut ,
            [ BeNull ] ISubject < IDevice >  deviceExpired )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( deviceExpired ) ) ;
        }

        [ AutoDataTestMethod ]
        public void TimeOut_ForValueGreaterZero_SetsTimeOut (
            DeviceMonitorWithExpiry sut )
        {
            var expected = TimeSpan.FromHours ( 1.23 ) ;

            sut.TimeOut = expected ;

            sut.TimeOut
               .Should ( )
               .Be ( expected ) ;
        }

        [ AutoDataTestMethod ]
        [ ExpectedException ( typeof ( ArgumentException ) ) ]
        public void TimeOut_ForValueLessThanZero_SetsTimeOut (
            DeviceMonitorWithExpiry sut )
        {
            sut.TimeOut = TimeSpan.FromHours ( - 0.1 ) ;
        }

        [ AutoDataTestMethod ]
        public void RemoveDevice_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor ,
            IDevice                   device )
        {
            sut.RemoveDevice ( device ) ;

            monitor.Received ( )
                   .RemoveDevice ( device ) ;
        }

        [ AutoDataTestMethod ]
        public void Start_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor )
        {
            sut.Start ( ) ;

            monitor.Received ( )
                   .Start ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Stop_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor )
        {
            sut.Stop ( ) ;

            monitor.Received ( )
                   .Stop ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_DisposesMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor )
        {
            sut.Dispose ( ) ;

            monitor.Received ( )
                   .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Dispose_ForInvoked_DisposesTimer (
            DeviceMonitorWithExpiry sut ,
            [ Freeze ] IDisposable  timer )
        {
            sut.Dispose ( ) ;

            timer.Received ( )
                 .Dispose ( ) ;
        }

        [ AutoDataTestMethod ]
        public void DiscoveredDevices_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry         sut ,
            [ Freeze ] IDeviceMonitor       monitor ,
            IReadOnlyCollection < IDevice > collection )
        {
            monitor.DiscoveredDevices
                   .Returns ( collection ) ;

            sut.DiscoveredDevices
               .Should ( )
               .BeEquivalentTo ( collection ) ;
        }

        [ AutoDataTestMethod ]
        public void IsListening_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor )
        {
            monitor.IsListening
                   .Returns ( true ) ;

            sut.IsListening
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void DeviceUpdated_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor ,
            IObservable < IDevice >   observable )
        {
            monitor.DeviceUpdated
                   .Returns ( observable ) ;

            sut.DeviceUpdated
               .Should ( )
               .Be ( observable ) ;
        }

        [ AutoDataTestMethod ]
        public void DeviceDiscovered_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor ,
            IObservable < IDevice >   observable )
        {
            monitor.DeviceDiscovered
                   .Returns ( observable ) ;

            sut.DeviceDiscovered
               .Should ( )
               .Be ( observable ) ;
        }

        [ AutoDataTestMethod ]
        public void DeviceNameUpdated_ForInvoked_CallsDeviceMonitor (
            DeviceMonitorWithExpiry   sut ,
            [ Freeze ] IDeviceMonitor monitor ,
            IObservable < IDevice >   observable )
        {
            monitor.DeviceNameUpdated
                   .Returns ( observable ) ;

            sut.DeviceNameUpdated
               .Should ( )
               .Be ( observable ) ;
        }

        [ AutoDataTestMethod ]
        public void CleanUp_ForNotExpiredDeviceInCollection_DoesNotRemoveDeviceFromCollection (
            ILogger                logger ,
            IDateTimeOffset        dateTimeOffset ,
            IDeviceMonitor         deviceMonitor ,
            ISubject < IDevice >   deviceExpired ,
            ObservableTimerFactory factory ,
            TestScheduler          scheduler ,
            IDevice                device )
        {
            deviceMonitor.DiscoveredDevices
                         .Returns ( new [ ] { device } ) ;

            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            dateTimeOffset.Ticks
                          .Returns ( sut.TimeOut.Ticks ) ;
            dateTimeOffset.Now
                          .Returns ( dateTimeOffset ) ;

            device.BroadcastTime
                  .Ticks
                  .Returns ( sut.TimeOut.Ticks / 2 ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks ) ;

            deviceMonitor.DidNotReceive ( )
                         .RemoveDevice ( device ) ;
        }

        [ AutoDataTestMethod ]
        public void CleanUp_ForNotExpiredDeviceInCollection_DoesNotNotifyDeviceExpired (
            ILogger                logger ,
            IDateTimeOffset        dateTimeOffset ,
            IDeviceMonitor         deviceMonitor ,
            ISubject < IDevice >   deviceExpired ,
            ObservableTimerFactory factory ,
            TestScheduler          scheduler ,
            IDevice                device )
        {
            deviceMonitor.DiscoveredDevices
                         .Returns ( new [ ] { device } ) ;

            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            dateTimeOffset.Ticks
                          .Returns ( sut.TimeOut.Ticks ) ;
            dateTimeOffset.Now
                          .Returns ( dateTimeOffset ) ;

            device.BroadcastTime
                  .Ticks
                  .Returns ( sut.TimeOut.Ticks / 2 ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks ) ;

            deviceExpired.DidNotReceive ( )
                         .Publish ( device ) ;
        }

        [ AutoDataTestMethod ]
        public void CleanUp_ForOneExpiredDeviceInCollection_RemovesDeviceFromCollection (
            ILogger                logger ,
            IDateTimeOffset        dateTimeOffset ,
            IDeviceMonitor         deviceMonitor ,
            ISubject < IDevice >   deviceExpired ,
            ObservableTimerFactory factory ,
            TestScheduler          scheduler ,
            IDevice                device )
        {
            deviceMonitor.DiscoveredDevices
                         .Returns ( new [ ] { device } ) ;

            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            dateTimeOffset.Ticks
                          .Returns ( sut.TimeOut.Ticks ) ;
            dateTimeOffset.Now
                          .Returns ( dateTimeOffset ) ;

            device.BroadcastTime
                  .Ticks
                  .Returns ( 0 ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks + 1 ) ;

            deviceMonitor.Received ( )
                         .RemoveDevice ( device ) ;
        }

        [ AutoDataTestMethod ]
        public void CleanUp_ForOneExpiredDeviceInCollection_NotifiesDeviceExpired (
            ILogger                logger ,
            IDateTimeOffset        dateTimeOffset ,
            IDeviceMonitor         deviceMonitor ,
            Subject < IDevice >    deviceExpired ,
            ObservableTimerFactory factory ,
            TestScheduler          scheduler ,
            IDevice                device )
        {
            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            IDevice expiredDevice = null ;

            using var disposable = sut.DeviceExpired
                                      .Subscribe ( expired => expiredDevice = expired ) ;

            deviceExpired.OnNext ( device ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks ) ;

            expiredDevice.Should ( )
                         .Be ( device ) ;
        }

        [ AutoDataTestMethod ]
        public void OnCompleted_ForInvoked_CallsStop (
            ILogger                 logger ,
            IDateTimeOffset         dateTimeOffset ,
            IDeviceMonitor          deviceMonitor ,
            ISubject < IDevice >    deviceExpired ,
            IObservableTimerFactory factory ,
            TestScheduler           scheduler )
        {
            factory.Create ( Arg.Any < TimeSpan > ( ) ,
                             Arg.Any < IScheduler > ( ) )
                   .Returns ( Observable.Empty < long > ( ) ) ;

            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks ) ;

            deviceMonitor.Received ( )
                         .Stop ( ) ;
        }

        [ AutoDataTestMethod ]
        public void OnError_ForInvoked_CallsStop (
            ILogger                 logger ,
            IDateTimeOffset         dateTimeOffset ,
            IDeviceMonitor          deviceMonitor ,
            ISubject < IDevice >    deviceExpired ,
            IObservableTimerFactory factory ,
            TestScheduler           scheduler )
        {
            factory.Create ( Arg.Any < TimeSpan > ( ) ,
                             Arg.Any < IScheduler > ( ) )
                   .Returns ( Observable.Throw < long > ( new Exception ( ) ) ) ;

            var sut = new DeviceMonitorWithExpiry ( logger ,
                                                    dateTimeOffset ,
                                                    deviceMonitor ,
                                                    deviceExpired ,
                                                    factory ,
                                                    scheduler ) ;

            scheduler.AdvanceBy ( sut.TimeOut.Ticks ) ;

            deviceMonitor.Received ( )
                         .Stop ( ) ;
        }
    }
}