using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeviceMonitorWithExpiry
        : IDeviceMonitorWithExpiry
    {
        public DeviceMonitorWithExpiry (
            [ NotNull ] ILogger                 logger ,
            [ NotNull ] IDateTimeOffset         dateTimeOffset ,
            [ NotNull ] IDeviceMonitor          deviceMonitor ,
            [ NotNull ] ISubject < IDevice >    deviceExpired ,
            [ NotNull ] IObservableTimerFactory factory ,
            [ NotNull ] IScheduler              scheduler )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( dateTimeOffset ,
                                    nameof ( dateTimeOffset ) ) ;
            Guard.ArgumentNotNull ( deviceMonitor ,
                                    nameof ( deviceMonitor ) ) ;
            Guard.ArgumentNotNull ( deviceExpired ,
                                    nameof ( deviceExpired ) ) ;
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;

            _logger         = logger ;
            _dateTimeOffset = dateTimeOffset ;
            _deviceMonitor  = deviceMonitor ;
            _deviceExpired  = deviceExpired ;

            _timer = factory.Create ( TimeOut ,
                                      scheduler )
                            .SubscribeOn ( scheduler )
                            .Subscribe ( CleanUp ,
                                         OnError ,
                                         OnCompleted ) ;
        }

        /// <inheritdoc />
        public TimeSpan TimeOut
        {
            get => _timeOut ;
            set
            {
                if ( value.TotalSeconds < 0 )
                    throw new ArgumentException ( "Value must be >= 0" ) ;

                _timeOut = value ;

                _logger.Information ( $"TimeOut = {value}" ) ;
            }
        }

        /// <inheritdoc />
        public IObservable < IDevice > DeviceExpired => _deviceExpired ;

        /// <inheritdoc />
        public void Dispose ( )
        {
            _timer.Dispose ( ) ;
            _deviceMonitor.Dispose ( ) ;
        }

        /// <inheritdoc />
        public IReadOnlyCollection < IDevice > DiscoveredDevices => _deviceMonitor.DiscoveredDevices ;

        /// <inheritdoc />
        public bool IsListening => _deviceMonitor.IsListening ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceUpdated => _deviceMonitor.DeviceUpdated ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceDiscovered => _deviceMonitor.DeviceDiscovered ;

        /// <inheritdoc />
        public IObservable < IDevice > DeviceNameUpdated => _deviceMonitor.DeviceNameUpdated ;

        /// <inheritdoc />
        public void Start ( )
        {
            _deviceMonitor.Start ( ) ;
        }

        /// <inheritdoc />
        public void Stop ( )
        {
            _deviceMonitor.Stop ( ) ;
        }

        /// <inheritdoc />
        public void RemoveDevice ( IDevice device )
        {
            _deviceMonitor.RemoveDevice ( device ) ;
        }

        internal const int SixtySeconds = 60 ;

        private void OnCompleted ( )
        {
            Stop ( ) ;
        }

        private void OnError ( Exception ex )
        {
            _logger.Error ( ex.Message ) ;

            Stop ( ) ;
        }

        private void CleanUp ( long l )
        {
            foreach ( var device in DiscoveredDevices )
            {
                var delta = _dateTimeOffset.Now.Ticks - device.BroadcastTime.Ticks ;

                if ( ! ( delta >= TimeOut.Ticks ) )
                    continue ;

                RemoveDevice ( device ) ;

                _deviceExpired.Publish ( device ) ;
            }
        }

        private readonly IDateTimeOffset _dateTimeOffset ;

        private readonly ISubject < IDevice > _deviceExpired ;
        private readonly IDeviceMonitor       _deviceMonitor ;
        private readonly ILogger              _logger ;
        private readonly IDisposable          _timer ;
        private          TimeSpan             _timeOut = TimeSpan.FromSeconds ( SixtySeconds ) ;
    }
}