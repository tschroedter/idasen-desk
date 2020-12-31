using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    public class DeskDetector
        : IDeskDetector
    {
        public DeskDetector ( [ NotNull ] ILogger                  logger ,
                              [ NotNull ] IScheduler               scheduler ,
                              [ NotNull ] IDeviceMonitorWithExpiry monitor ,
                              [ NotNull ] IDeskFactory             factory ,
                              [ NotNull ] ISubject < IDesk >       deskDetected )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( monitor ,
                                    nameof ( monitor ) ) ;
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;
            Guard.ArgumentNotNull ( deskDetected ,
                                    nameof ( deskDetected ) ) ;

            _logger       = logger ;
            _scheduler    = scheduler ;
            _monitor      = monitor ;
            _factory      = factory ;
            _deskDetected = deskDetected ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _refreshedChanged?.Dispose ( ) ;
            _deskFound?.Dispose ( ) ;
            _nameChanged?.Dispose ( ) ;
            _discovered?.Dispose ( ) ;
            _updated?.Dispose ( ) ;
            _monitor?.Dispose ( ) ;
        }

        /// <inheritdoc />
        public IObservable < IDesk > DeskDetected => _deskDetected ;

        /// <inheritdoc />
        public void Initialize ( string deviceName,
                                 ulong  deviceAddress,
                                 uint   deviceTimeout)
        {
            Guard.ArgumentNotNull ( deviceName ,
                                    nameof ( deviceName ) ) ;

            _monitor.TimeOut = TimeSpan.FromSeconds(deviceTimeout) ;

            _updated = _monitor.DeviceUpdated
                               .ObserveOn ( _scheduler )
                               .Subscribe ( OnDeviceUpdated ) ;

            _discovered = _monitor.DeviceDiscovered
                                  .ObserveOn ( _scheduler )
                                  .Subscribe ( OnDeviceDiscovered ) ;

            _nameChanged = _monitor.DeviceNameUpdated
                                   .ObserveOn ( _scheduler )
                                   .Subscribe ( OnDeviceNameChanged ) ;

            _deskFound = _monitor.DeviceNameUpdated
                                 .ObserveOn ( _scheduler )
                                 .Where ( device => device.Name.StartsWith ( deviceName ) ||
                                                    device.Address == deviceAddress )
                                 .Subscribe ( OnDeskDiscovered ) ;
        }

        /// <inheritdoc />
        public void Start ( )
        {
            _monitor.Start ( ) ;
        }

        /// <inheritdoc />
        public void Stop ( )
        {
            _monitor.Stop ( ) ;
        }

        private async void OnDeskDiscovered ( IDevice device )
        {
            if ( _desk != null )
                return ;

            try
            {
                _logger.Information ( $"[{device.Name}] Desk discovered" );

                _desk = await _factory.CreateAsync ( device.Address ) ;

                _refreshedChanged = _desk.RefreshedChanged
                                         .Subscribe ( OnRefreshedChanged ) ;

                _desk.Connect ( ) ;
            }
            catch ( Exception e )
            {
                Console.WriteLine ( e ) ;
                throw ;
            }
        }

        private void OnDeviceUpdated ( IDevice device )
        {
            _logger.Information ( $"[{device.Name}] Device Updated: {device}" ) ;
        }

        private void OnDeviceDiscovered ( IDevice device )
        {
            _logger.Information ( $"[{device.Name}] Device Discovered: {device}" ) ;
        }

        private void OnDeviceNameChanged ( IDevice device )
        {
            _logger.Information ( $"[{device.Name}] Device Name Changed: {device}" ) ;
        }

        private void OnRefreshedChanged ( bool status )
        {
            _deskDetected.OnNext ( _desk ) ;
        }

        private readonly ISubject < IDesk > _deskDetected ;

        private readonly IDeskFactory _factory ;

        private readonly ILogger                  _logger ;
        private readonly IDeviceMonitorWithExpiry _monitor ;
        private readonly IScheduler               _scheduler ;

        private IDesk       _desk ;
        private IDisposable _deskFound ;
        private IDisposable _discovered ;

        private IDisposable _nameChanged ;
        private IDisposable _refreshedChanged ;
        private IDisposable _updated ;
    }
}