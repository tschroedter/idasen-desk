using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Subjects ;
using Windows.Devices.Bluetooth.Advertisement ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;
using AdvertisementWatcher = Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher ;
using ExcludeFromCodeCoverage = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    [ Intercept ( typeof ( LogAspect ) ) ]
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
            Subscribe ( ) ;

            _watcher.Start ( ) ;
        }

        /// <inheritdoc />
        public void Stop ( )
        {
            _watcher.Stop ( ) ;

            Unsubscribe ( ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            Unsubscribe ( ) ;
        }

        private void Subscribe ( )
        {
            Unsubscribe ( ) ;

            _watcher.Received += OnReceivedHandler ;
            _watcher.Stopped  += OnStoppedHandler ;
        }

        private void Unsubscribe ( )
        {
            _watcher.Received -= OnReceivedHandler ;
            _watcher.Stopped  -= OnStoppedHandler ;
        }

        private void OnStoppedHandler ( AdvertisementWatcher                            sender ,
                                        BluetoothLEAdvertisementWatcherStoppedEventArgs args )
        {
            _stopped.OnNext ( DateTime.Now ) ;
        }

        private void OnReceivedHandler ( BluetoothLEAdvertisementWatcher           sender ,
                                         BluetoothLEAdvertisementReceivedEventArgs args )
        {
            var dateTimeOffset = _dateTimeFactory.Invoke ( args.Timestamp ) ;

            var device = _deviceFactory.Create ( dateTimeOffset ,
                                                 args.BluetoothAddress ,
                                                 args.Advertisement.LocalName ,
                                                 args.RawSignalStrengthInDBm ) ;

            _received.OnNext ( device ) ;
        }

        private readonly Func < DateTimeOffset , IDateTimeOffset > _dateTimeFactory ;
        private readonly IDeviceFactory                            _deviceFactory ;
        private readonly ISubject < IDevice >                      _received ;
        private readonly IStatusMapper                             _statusMapper ;
        private readonly ISubject < DateTime >                     _stopped ;
        private readonly AdvertisementWatcher                      _watcher ;
    }
}