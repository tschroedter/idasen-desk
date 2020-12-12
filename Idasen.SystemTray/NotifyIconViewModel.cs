using System ;
using System.Threading ;
using System.Threading.Tasks ;
using System.Windows ;
using System.Windows.Input ;
using Autofac ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Settings ;
using JetBrains.Annotations ;
using Serilog ;
// ReSharper disable UnusedMember.Global

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Provides bindable properties and commands for the NotifyIcon. In this sample, the
    ///     view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    ///     in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        public NotifyIconViewModel ( )
        {
            var container = ContainerProvider.Create ( "Idasen.SystemTray" ,
                                                        "Idasen.SystemTray.log" ) ;

            _logger   = container.Resolve < ILogger > ( ) ;
            _provider = container.Resolve < IDeskProvider > ( ) ;

            _provider.Initialize ( ) ;

            _manager = new SettingsManager ( _logger ) ; // todo move into container
        }

        /// <summary>
        ///     Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowSettingsCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CanExecuteFunc = ( ) => SettingsWindow == null ,
                           CommandAction = ( ) =>
                                           {
                                               SettingsWindow = new SettingsWindow ( _logger ,
                                                                                     _manager ) ;
                                               SettingsWindow?.Show ( ) ;
                                           }
                       } ;
            }
        }

        /// <summary>
        ///     Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideSettingsCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction = ( ) =>
                                           {
                                               SettingsWindow?.Close ( ) ;
                                               SettingsWindow = null ;
                                           } ,
                           CanExecuteFunc = ( ) => SettingsWindow != null
                       } ;
            }
        }

        /// <summary>
        ///     Connects to the Idasen Desk.
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction  = () => Task.Run(Connect) ,
                           CanExecuteFunc = ( ) => _desk == null
                       } ;
            }
        }

        private async void Connect ( )
        {
            _logger.Debug ( "Trying yo connect to Idasen Desk..." );

            _desk?.Dispose (  );

            var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            _token       = tokenSource.Token;

            var (isSuccess, desk) = await _provider.TryGetDesk(_token);

            if (isSuccess)
            {
                _logger.Information ( $"Connected to desk {desk}" );

                _desk = desk ;
            }
            else
            {
                _logger.Error("Failed to detect desk");

                _desk = null ;
            }
        }


        /// <summary>
        ///     Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = ( ) => Application.Current.Shutdown ( ) } ; }
        }

        [ CanBeNull ]
        private ISettingsWindow SettingsWindow
        {
            get => Application.Current.MainWindow as ISettingsWindow ;
            set => Application.Current.MainWindow = value as Window ;
        }

        private readonly ILogger    _logger ;

        private readonly ISettingsManager  _manager ;
        private readonly IDeskProvider     _provider ;
        [CanBeNull] private          IDesk             _desk ;
        private          CancellationToken _token ;
    }
}