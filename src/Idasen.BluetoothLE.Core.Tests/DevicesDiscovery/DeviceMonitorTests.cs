using System ;
using System.Collections.Generic ;
using System.Reactive ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Selkie.AutoMocking ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery
{
    [ AutoDataTestClass ]
    public class DeviceMonitorTests
    {
        [ TestInitialize ]
        public void Initialize ( )
        {
            _scheduler = new TestScheduler ( ) ;

            _logger = Substitute.For < ILogger > ( ) ;

            _device = Substitute.For < IDevice > ( ) ;
            _device.Name
                   .Returns ( ( string ) null ) ;

            _deviceNewName = Substitute.For < IDevice > ( ) ;
            _deviceNewName.Name
                          .Returns ( "New Name" ) ;

            _deviceOtherNewName = Substitute.For < IDevice > ( ) ;
            _deviceOtherNewName.Name
                               .Returns ( "Other New Name" ) ;

            _factory = Factory ;

            _subjectStarted = new Subject < IDevice > ( ) ;
            _subjectStopped = new Subject < IDevice > ( ) ;
            _subjectUpdated = new Subject < IDevice > ( ) ;

            _devices = new Devices ( _logger ) ;
            _watcher = Substitute.For < IWatcher > ( ) ;

            _subjects = new Queue < ISubject < IDevice > > ( ) ;
            _subjects.Enqueue ( _subjectStarted ) ;
            _subjects.Enqueue ( _subjectStopped ) ;
            _subjects.Enqueue ( _subjectUpdated ) ;
        }

        private Recorded < Notification < IDevice > > [ ] OnMultipleNext ( IEnumerable < IDevice > devices )
        {
            var list = new List < Recorded < Notification < IDevice > > > ( ) ;

            var time = 0 ;

            foreach ( var device in devices )
            {
                list.Add ( OnNext ( time ++ ,
                                    device ) ) ;
            }

            return list.ToArray ( ) ;
        }


        private Recorded < Notification < IDevice > > OnNext (
            long    time ,
            IDevice device )
        {
            return new Recorded < Notification < IDevice > > ( time ,
                                                               Notification.CreateOnNext ( device ) ) ;
        }

        private ISubject < IDevice > Factory ( )
        {
            return _subjects.Dequeue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForLoggerNull_Throws (
            Lazy < DeviceMonitor > sut ,
            [ BeNull ] ILogger     logger )
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
        public void Constructor_ForFactoryNull_Throws (
            Lazy < DeviceMonitor >                   sut ,
            [ BeNull ] Func < ISubject < IDevice > > factory )
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
        public void Constructor_ForDevicesNull_Throws (
            Lazy < DeviceMonitor > sut ,
            [ BeNull ] IDevices    devices )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( devices ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForWatcherNull_Throws (
            Lazy < DeviceMonitor > sut ,
            [ BeNull ] IWatcher    watcher )
        {
            // ReSharper disable once UnusedVariable
            Action action = ( ) =>
                            {
                                var test = sut.Value ;
                            } ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( nameof ( watcher ) ) ;
        }

        [ AutoDataTestMethod ]
        public void Start_ForInvoked_CallsStart (
            DeviceMonitor       sut ,
            [ Freeze ] IWatcher watcher )
        {
            sut.Start ( ) ;

            watcher.Received ( )
                   .Start ( ) ;
        }

        [ AutoDataTestMethod ]
        public void Stop_ForInvoked_CallsStop (
            DeviceMonitor       sut ,
            [ Freeze ] IWatcher watcher )
        {
            sut.Stop ( ) ;

            watcher.Received ( )
                   .Stop ( ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForNewDevice_AddsDevice ( )
        {
            ConfigureDeviceDiscovered ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            _scheduler.Start ( ) ;

            _devices.ContainsDevice ( _device )
                    .Should ( )
                    .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForNewDevice_RaisesDeviceDiscovered ( )
        {
            ConfigureDeviceDiscovered ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            IDevice discovered = null ;

            using var observer = sut.DeviceDiscovered
                                    .Subscribe ( x => discovered = x ) ;

            _scheduler.Start ( ) ;

            discovered.Should ( )
                      .Be ( _device ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDevice_UpdatesDevices ( )
        {
            ConfigureNameUpdated ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            _scheduler.Start ( ) ;

            _devices.TryGetDevice ( _device.Address ,
                                    out var device )
                    .Should ( )
                    .BeTrue ( ) ;

            device?.Name
                   .Should ( )
                   .Be ( _deviceNewName.Name ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDevice_RaisesDeviceUpdated ( )
        {
            ConfigureSameDevice ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            IDevice updated = null ;

            using var observer = sut.DeviceUpdated
                                    .Subscribe ( x => updated = x ) ;

            _scheduler.Start ( ) ;

            updated.Should ( )
                   .Be ( _device ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDeviceWithNewName_KeepsFirstName ( )
        {
            ConfigureNameUpdatedTwice ( ) ; // maybe, later allow name change?

            using var sut = CreateSutSubscribed ( ) ;

            _scheduler.Start ( ) ;

            _devices.TryGetDevice ( _device.Address ,
                                    out var device )
                    .Should ( )
                    .BeTrue ( ) ;

            device?.Name
                   .Should ( )
                   .Be ( _deviceNewName.Name ) ;
        }

        private DeviceMonitor CreateSutSubscribed ( )
        {
            var sut = CreateSut ( ) ;
            sut.Start ( ) ;

            return sut ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDeviceWithNewName_RaisesDeviceUpdated ( )
        {
            ConfigureNameUpdated ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            IDevice updated = null ;

            using var observer = sut.DeviceUpdated
                                    .Subscribe ( x => updated = x ) ;

            _scheduler.Start ( ) ;

            updated.Should ( )
                   .Be ( _deviceNewName ) ;
        }

        private void ConfigureNameUpdatedTwice ( )
        {
            var messages = new [ ]
                           {
                               _device ,
                               _deviceNewName ,
                               _deviceOtherNewName
                           } ;

            _watcher.Received
                    .Returns ( _scheduler.CreateColdObservable ( OnMultipleNext ( messages ) ) ) ;
        }

        private void ConfigureNameUpdated ( )
        {
            var messages = new [ ]
                           {
                               _device ,
                               _deviceNewName
                           } ;

            _watcher.Received
                    .Returns ( _scheduler.CreateColdObservable ( OnMultipleNext ( messages ) ) ) ;
        }

        private void ConfigureDeviceDiscovered ( )
        {
            var messages = new [ ]
                           {
                               _device
                           } ;

            _watcher.Received
                    .Returns ( _scheduler.CreateColdObservable ( OnMultipleNext ( messages ) ) ) ;
        }

        private void ConfigureSameDevice ( )
        {
            var messages = new [ ]
                           {
                               _device ,
                               _device
                           } ;

            _watcher.Received
                    .Returns ( _scheduler.CreateColdObservable ( OnMultipleNext ( messages ) ) ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDeviceWithNewName_RaisesDeviceNameUpdated ( )
        {
            ConfigureNameUpdated ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            IDevice updated = null ;

            using var observer = sut.DeviceNameUpdated
                                    .Subscribe ( x => updated = x ) ;

            _scheduler.Start ( ) ;

            updated.Should ( )
                   .Be ( _deviceNewName ) ;
        }

        [ TestMethod ]
        public void OnDeviceUpdated_ForExistingDevice_Notifies ( )
        {
            ConfigureSameDevice ( ) ;

            using var sut = CreateSutSubscribed ( ) ;

            IDevice updated = null ;

            using var observer = sut.DeviceUpdated
                                    .Subscribe ( x => updated = x ) ;

            _scheduler.Start ( ) ;

            updated.Should ( )
                   .Be ( _device ) ;
        }

        [ AutoDataTestMethod ]
        public void IsListening_ForInvoked_CallsWatcher (
            DeviceMonitor       sut ,
            [ Freeze ] IWatcher watcher )
        {
            watcher.IsListening
                   .Returns ( true ) ;

            sut.IsListening
               .Should ( )
               .BeTrue ( ) ;
        }

        [ AutoDataTestMethod ]
        public void DiscoveredDevices_ForInvoked_CallsDevices (
            DeviceMonitor       sut ,
            [ Freeze ] IDevices devices )
        {
            devices.DiscoveredDevices
                   .Returns ( new List < IDevice > ( ).AsReadOnly ( ) ) ;

            sut.DiscoveredDevices
               .Should ( )
               .BeEmpty ( ) ;
        }

        [ AutoDataTestMethod ]
        public void RemoveDevice_ForInvoked_CallsDevices (
            DeviceMonitor       sut ,
            [ Freeze ] IDevices devices ,
            IDevice             device )
        {
            sut.RemoveDevice ( device ) ;

            devices.Received ( )
                   .RemoveDevice ( device ) ;
        }

        private DeviceMonitor CreateSut ( )
        {
            var deviceMonitor = new DeviceMonitor ( _logger ,
                                                    _scheduler ,
                                                    _factory ,
                                                    _devices ,
                                                    _watcher ) ;

            return deviceMonitor ;
        }

        private IDevice                        _device ;
        private IDevice                        _deviceNewName ;
        private IDevice                        _deviceOtherNewName ;
        private IDevices                       _devices ;
        private Func < ISubject < IDevice > >  _factory ;
        private ILogger                        _logger ;
        private TestScheduler                  _scheduler ;
        private Queue < ISubject < IDevice > > _subjects ;
        private ISubject < IDevice >           _subjectStarted ;
        private ISubject < IDevice >           _subjectStopped ;
        private ISubject < IDevice >           _subjectUpdated ;
        private IWatcher                       _watcher ;
    }
}