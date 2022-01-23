using System ;
using System.Threading.Tasks ;
using System.Windows.Input ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow
        : ISettingsWindow
    {
        public SettingsWindow (
            [ NotNull ] ILogger          logger ,
            [ NotNull ] ISettingsManager manager )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;

            _logger  = logger ;
            _manager = manager ;

            InitializeComponent ( ) ;

            Task.Run ( Initialize ) ;
        }

        public event EventHandler AdvancedSettingsChanged;

        private async void Initialize ( )
        {
            try
            {
                await _manager.Load ( ) ;

                Update ( _manager.CurrentSettings ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to initialize" ) ;
            }
        }

        private void ImageClose_MouseDown ( object               sender ,
                                            MouseButtonEventArgs e )
        {
            _logger.Debug ( $"Closing {GetType ( ).Name}..." ) ;

            Close ( ) ;
        }

        private void StoreSettings ( )
        {
            var settings = _manager.CurrentSettings ;

            var newDeviceName    = _nameConverter.DefaultIfEmpty ( DeskName.Text ) ;
            var newDeviceAddress = _addressConverter.DefaultIfEmpty ( DeskAddress.Text ) ;

            var advancedChanged = settings.DeviceName    != newDeviceName ||
                                  settings.DeviceAddress != newDeviceAddress ;

            settings.StandingHeightInCm = _doubleConverter.ConvertToUInt ( Standing.Value ,
                                                                           Constants.DefaultHeightStandingInCm ) ;
            settings.SeatingHeightInCm = _doubleConverter.ConvertToUInt ( Seating.Value ,
                                                                          Constants.DefaultHeightSeatingInCm ) ;
            settings.DeviceName    = newDeviceName ;
            settings.DeviceAddress = newDeviceAddress ;

            Task.Run ( async ( ) =>
                       {
                           _logger.Debug ( $"Storing new settings: {settings}" ) ;

                           await _manager.Save ( ) ;

                           if (advancedChanged)
                               AdvancedSettingsChanged?.Invoke ( this ,
                                                                 EventArgs.Empty ) ;
                       } ) ;
        }

        private void SettingsWindow_OnClosed ( object    sender ,
                                               EventArgs e )
        {
            _logger.Debug ( "Handling 'Closed' event" ) ;

            StoreSettings ( ) ;
        }

        private void Update ( ISettings settings )
        {
            if ( ! Dispatcher.CheckAccess ( ) )
            {
                Dispatcher.BeginInvoke ( new Action ( ( ) => Update ( settings ) ) ) ;

                return ;
            }

            _logger.Debug($"Update settings: {settings}");

            Standing.Value   = settings.StandingHeightInCm ;
            Seating.Value    = settings.SeatingHeightInCm ;
            DeskName.Text    = _nameConverter.EmptyIfDefault ( settings.DeviceName ) ;
            DeskAddress.Text = _addressConverter.EmptyIfDefault ( settings.DeviceAddress ) ;
        }

        private readonly IDoubleToUIntConverter         _doubleConverter  = new DoubleToUIntConverter ( ) ;
        private readonly IDeviceNameConverter           _nameConverter    = new DeviceNameConverter ( ) ;
        private readonly IDeviceAddressToULongConverter _addressConverter = new DeviceAddressToULongConverter ( ) ;
        private readonly ILogger                        _logger ;
        private readonly ISettingsManager               _manager ;
    }
}