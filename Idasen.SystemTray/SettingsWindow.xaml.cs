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

            Task.Run ( new Action ( async ( ) =>
                                    {
                                        await _manager.Load ( ) ;

                                        Update ( _manager.CurrentSettings ) ;
                                    } ) ) ;
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

            _logger.Debug ( $"Storing new settings: {settings}" ) ;

            settings.StandingHeightInCm = _converter.ConvertToUInt ( Standing.Value ,
                                                                     Constants.DefaultHeightStandingInCm ) ;
            settings.SeatingHeightInCm = _converter.ConvertToUInt ( Seating.Value ,
                                                                    Constants.DefaultHeightSeatingInCm ) ;
            Task.Run ( async ( ) => await _manager.Save ( ) ) ;
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

            Standing.Value = settings.StandingHeightInCm ;
            Seating.Value  = settings.SeatingHeightInCm ;
        }

        private readonly IDoubleToUIntConverter _converter = new DoubleToUIntConverter ( ) ;
        private readonly ILogger                _logger ;
        private readonly ISettingsManager       _manager ;
    }
}