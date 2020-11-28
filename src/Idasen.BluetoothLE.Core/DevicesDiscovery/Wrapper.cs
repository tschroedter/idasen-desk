using System ;
using System.Reactive ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Windows.Devices.Bluetooth.Advertisement ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;
using AdvertisementWatcher = Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher ;
using ExcludeFromCodeCoverage = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public sealed class Wrapper
        : IWrapper
    {
        public Wrapper ( [ NotNull ] IScheduler                                scheduler ,
                         [ NotNull ] IDeviceFactory                            deviceFactory ,
                         [ NotNull ] Func < DateTimeOffset , IDateTimeOffset > dateTimeFactory ,
                         [ NotNull ] ISubject < IDevice >                      received ,
                         [ NotNull ] ISubject < DateTime >                     stopped ,
                         [ NotNull ] IStatusMapper                             statusMapper )
        {
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( deviceFactory ,
                                    nameof ( deviceFactory ) ) ;
            Guard.ArgumentNotNull ( dateTimeFactory ,
                                    nameof ( dateTimeFactory ) ) ;
            Guard.ArgumentNotNull ( received ,
                                    nameof ( received ) ) ;
            Guard.ArgumentNotNull ( stopped ,
                                    nameof ( stopped ) ) ;
            Guard.ArgumentNotNull ( statusMapper ,
                                    nameof ( statusMapper ) ) ;

            _deviceFactory   = deviceFactory ;
            _dateTimeFactory = dateTimeFactory ;
            _received        = received ;
            _stopped         = stopped ;
            _statusMapper    = statusMapper ;

            _watcher = new AdvertisementWatcher
                       {
                           ScanningMode = BluetoothLEScanningMode.Active
                       } ;

            var fromReceivedEvent =
                Observable.FromEventPattern < BluetoothLEAdvertisementReceivedEventArgs > ( _watcher ,
                                                                                            "Received" ) ;
            var fromStoppedEvent =
                Observable.FromEventPattern < BluetoothLEAdvertisementWatcherStoppedEventArgs > ( _watcher ,
                                                                                                  "Stopped" ) ;

            _subscriberReceived = fromReceivedEvent.SubscribeOn ( scheduler )
                                                   .Subscribe ( OnReceived ) ;
            _subscriberStopped = fromStoppedEvent.SubscribeOn ( scheduler )
                                                 .Subscribe ( OnStopped ) ;
        }

        /// <inheritdoc />
        public IObservable < DateTime > Stopped => _stopped ;

        /// <inheritdoc />
        public IObservable < IDevice > Received => _received ;

        /// <inheritdoc />
        public Status Status => _statusMapper.Map ( _watcher.Status ) ;

        /// <inheritdoc />
        public void Start ( )
        {
            _watcher.Start ( ) ;
        }

        /// <inheritdoc />
        public void Stop ( )
        {
            _watcher.Stop ( ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _subscriberReceived.Dispose ( ) ;
            _subscriberStopped.Dispose ( ) ;
        }

        private void OnReceived ( EventPattern < BluetoothLEAdvertisementReceivedEventArgs > args )
        {
            var dateTimeOffset = _dateTimeFactory.Invoke ( args.EventArgs.Timestamp ) ;

            var device = _deviceFactory.Create ( dateTimeOffset ,
                                                 args.EventArgs.BluetoothAddress ,
                                                 args.EventArgs.Advertisement.LocalName ,
                                                 args.EventArgs.RawSignalStrengthInDBm ) ;

            _received.OnNext ( device ) ;
        }

        private void OnStopped ( EventPattern < BluetoothLEAdvertisementWatcherStoppedEventArgs > args )
        {
            _stopped.OnNext ( DateTime.Now ) ;
        }

        private readonly Func < DateTimeOffset , IDateTimeOffset > _dateTimeFactory ;
        private readonly IDeviceFactory                            _deviceFactory ;
        private readonly ISubject < IDevice >                      _received ;
        private readonly IStatusMapper                             _statusMapper ;
        private readonly ISubject < DateTime >                     _stopped ;
        private readonly IDisposable                               _subscriberReceived ;
        private readonly IDisposable                               _subscriberStopped ;
        private readonly AdvertisementWatcher                      _watcher ;
    }
}