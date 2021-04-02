using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;
using Serilog ;

// [assembly: InternalsVisibleTo("Idasen.BluetoothLE.Tests")]

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    /// <inheritdoc cref="IDeviceMonitor" />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeviceMonitor
        : IDeviceMonitor
    {
        public DeviceMonitor (
            [ NotNull ] ILogger                       logger ,
            [ NotNull ] IScheduler                    scheduler ,
            [ NotNull ] Func < ISubject < IDevice > > factory ,
            [ NotNull ] IDevices                      devices ,
            [ NotNull ] IWatcher                      watcher )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;
            Guard.ArgumentNotNull ( devices ,
                                    nameof ( devices ) ) ;
            Guard.ArgumentNotNull ( watcher ,
                                    nameof ( watcher ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;

            _logger    = logger ;
            _scheduler = scheduler ;
            _devices   = devices ;
            _watcher   = watcher ;

            _deviceUpdated     = factory.Invoke ( ) ;
            _deviceDiscovered  = factory.Invoke ( ) ;
            _deviceNameUpdated = factory.Invoke ( ) ;
        }

        /// <inheritdoc />
        public IReadOnlyCollection < IDevice > DiscoveredDevices => _devices.DiscoveredDevices ;

        /// <inheritdoc />
        public bool IsListening => _watcher.IsListening ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceUpdated => _deviceUpdated ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceDiscovered => _deviceDiscovered ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceNameUpdated => _deviceNameUpdated ;

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
        public void RemoveDevice ( IDevice device )
        {
            _devices.RemoveDevice ( device ) ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            Unsubscribe ( ) ;

            _watcher.Dispose ( ) ;
            _devices.Clear ( ) ;
        }

        private void Subscribe ( )
        {
            Unsubscribe ( ) ;

            _disposableStarted = _watcher.Started
                                         .SubscribeOn ( _scheduler )
                                         .Subscribe ( OnStarted ) ;
            _disposableStopped = _watcher.Stopped
                                         .SubscribeOn ( _scheduler )
                                         .Subscribe ( OnStopped ) ;
            _disposableUpdated = _watcher.Received
                                         .SubscribeOn ( _scheduler )
                                         .Subscribe ( OnDeviceUpdated ) ;
        }

        private void Unsubscribe ( )
        {
            _disposableStarted?.Dispose ( ) ;
            _disposableStopped?.Dispose ( ) ;
            _disposableUpdated?.Dispose ( ) ;
        }

        private void OnDeviceUpdated ( IDevice device )
        {
            if ( ! _devices.TryGetDevice ( device.Address ,
                                           out var storedDevice ) )
            {
                _logger.Information ( $"[{device.MacAddress}] Discovered Device" ) ;

                _devices.AddOrUpdateDevice ( device ) ;

                _deviceDiscovered.OnNext ( device ) ;
            }
            else
            {
                _logger.Information ( $"[{device.MacAddress}] Updated Device " +
                                      $"(Name = {device.Name}, "               +
                                      $"{device.RawSignalStrengthInDBm}DBm, "  +
                                      $"Address = {device.Address})" ) ;

                var hasNameChanged = HasDeviceNameChanged ( device ,
                                                            storedDevice ) ;

                _devices.AddOrUpdateDevice ( device ) ;

                _deviceUpdated.OnNext ( device ) ;

                if ( ! hasNameChanged )
                    return ;

                _logger.Information ( $"[{device.MacAddress}] Device Name Changed" ) ;

                _deviceNameUpdated.OnNext ( device ) ;
            }
        }

        private void OnStopped ( DateTime dateTime )
        {
            _logger.Information ( "Watcher Stopped listening" ) ;
        }

        private void OnStarted ( DateTime dateTime )
        {
            _logger.Information ( "Watcher Started listening" ) ;
        }

        private static bool HasDeviceNameChanged ( IDevice device ,
                                                   IDevice storedDevice )
        {
            if ( ! string.IsNullOrEmpty ( storedDevice.Name ) )
                return storedDevice.Name != device.Name ;

            if ( ! string.IsNullOrEmpty ( device.Name ) )
                return true ;

            return storedDevice.Name != device.Name ;
        }

        private readonly             ISubject < IDevice > _deviceDiscovered ;
        private readonly             ISubject < IDevice > _deviceNameUpdated ;
        private readonly             IDevices             _devices ;
        private readonly             ISubject < IDevice > _deviceUpdated ;
        private readonly             ILogger              _logger ;
        [ NotNull ] private readonly IScheduler           _scheduler ;

        private readonly IWatcher _watcher ;

        private IDisposable _disposableStarted ;
        private IDisposable _disposableStopped ;
        private IDisposable _disposableUpdated ;
    }
}