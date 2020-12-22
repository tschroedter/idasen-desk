using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Text ;
using Idasen.BluetoothLE.Characteristics.Common ;
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
            _subscriberDeskDeviceNameChanged?.Dispose ( ) ;
            _deskFound?.Dispose ( ) ; // todo list
            _nameChanged?.Dispose ( ) ;
            _discovered?.Dispose ( ) ;
            _updated?.Dispose ( ) ;
            _monitor?.Dispose ( ) ;
        }

        /// <inheritdoc />
        public IObservable < IDesk > DeskDetected => _deskDetected ;

        /// <inheritdoc />
        public void Initialize ( string deviceName    = "Desk" ,
                                 ulong  deviceAddress = 250635178951455u )
        {
            Guard.ArgumentNotNull ( deviceName ,
                                    nameof ( deviceName ) ) ;

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
                _desk = await _factory.CreateAsync ( device.Address ) ;

                _subscriberDeskDeviceNameChanged = _desk.DeviceNameChanged.Subscribe ( OnDeskDeviceNameChanged ) ;
                _refreshedChanged = _desk.RefreshedChanged
                                         .Subscribe ( OnRefreshedChanged ) ; // todo rename On..., swapped it with .Connect()

                _desk.Connect ( ) ;
            }
            catch ( Exception e )
            {
                Console.WriteLine ( e ) ;
                throw ;
            }
        }

        private void OnDeskDeviceNameChanged ( IEnumerable < byte > value )
        {
            var array = value.ToArray ( ) ;

            var text = Encoding.UTF8.GetString ( array ) ;

            _logger.Information ( $"Received: {array.ToHex ( )} - '{text}'" ) ;
        }

        private void OnDeviceUpdated ( IDevice device )
        {
            _logger.Information ( $"Device Updated: {device}" ) ;
        }

        private void OnDeviceDiscovered ( IDevice device )
        {
            _logger.Information ( $"Device Discovered: {device}" ) ;
        }

        private void OnDeviceNameChanged ( IDevice device )
        {
            _logger.Information ( $"Device Name Changed: {device}" ) ;
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
        private IDisposable _subscriberDeskDeviceNameChanged ;
        private IDisposable _updated ;
    }
}