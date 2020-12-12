using System ;
using System.Threading ;
using System.Threading.Tasks ;
using System.Windows ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using Autofac ;
using Hardcodet.Wpf.TaskbarNotification ;
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

            _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            _token = _tokenSource.Token;

            Task.Run ( AutoConnect ) ;
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
                           CommandAction  = async ( ) => { await Connect ( ) ; } ,
                           CanExecuteFunc = ( ) => _desk == null
                       } ;
            }
        }

        /// <summary>
        ///     Moves the desk to the standing height.
        /// </summary>
        public ICommand StandingCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction = async ( ) =>
                                           {
                                               await _manager.Load ( ) ; // todo loading on UI thread

                                               _desk?.MoveTo ( _manager.CurrentSettings.StandingHeightInCm * 100 ) ;
                                           } ,
                           CanExecuteFunc = ( ) => _desk != null
                       } ;
            }
        }

        /// <summary>
        ///     Moves the desk to the seating height.
        /// </summary>
        public ICommand SeatingCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction = async ( ) =>
                                           {
                                               await _manager.Load ( ) ; // todo loading on UI thread

                                               _desk?.MoveTo ( _manager.CurrentSettings.SeatingHeightInCm * 100 ) ;
                                           } ,
                           CanExecuteFunc = ( ) => _desk != null
                       } ;
            }
        }

        /// <summary>
        ///     Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = ( ) =>
                                                               {
                                                                   _tokenSource.Cancel();
                                                                   Application.Current.Shutdown ( ) ;
                                                               }
                                             } ; }
        }

        [ CanBeNull ]
        private ISettingsWindow SettingsWindow
        {
            get => Application.Current.MainWindow as ISettingsWindow ;
            set => Application.Current.MainWindow = value as Window ;
        }

        private async void AutoConnect ( )
        {
            try
            {
                _logger.Debug ( "Trying to auto connect to Idasen Desk..." ) ;

                await Task.Delay ( TimeSpan.FromSeconds ( 5 ) ,
                                   _token ) ;

                ShowFancyBalloon ( "Auto Connect" ,
                                   "Trying to connected to a desk" ,
                                   visibilityBulbYellow : Visibility.Visible ) ;

                await Connect ( ) ;
            }
            catch ( TaskCanceledException )
            {
                _logger.Information ( "Auto connect was canceled" );
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to auto connect to desk" ) ;
            }
        }

        private async Task Connect ( )
        {
            _logger.Debug ( "Trying to connect to Idasen Desk..." ) ;

            _desk?.Dispose ( ) ;

            _tokenSource?.Cancel(false);

            _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            _token = _tokenSource.Token ;

            var (isSuccess , desk) = await _provider.TryGetDesk ( _token ) ;

            if ( isSuccess )
            {
                _logger.Information ( $"Connected to {desk}" ) ;

                _desk = desk ;

                ShowFancyBalloon ( "Success" ,
                                   $"Connected to desk {desk.Name}" ,
                                   Visibility.Visible ) ;
            }
            else
            {
                _logger.Error ( "Failed to detect desk" ) ;

                _desk = null ;

                ShowFancyBalloon ( "Failed" ,
                                   "Connection to desk failed" ,
                                   visibilityBulbRed : Visibility.Visible ) ;
            }
        }

        private void ShowFancyBalloon ( string     title ,
                                        string     text ,
                                        Visibility visibilityBulbGreen  = Visibility.Hidden ,
                                        Visibility visibilityBulbYellow = Visibility.Hidden ,
                                        Visibility visibilityBulbRed    = Visibility.Hidden )
        {
            _notifyIcon ??= ( TaskbarIcon ) Application.Current.FindResource ( "NotifyIcon" ) ;

            if ( _notifyIcon == null )
                return ;

            if ( ! _notifyIcon.Dispatcher.CheckAccess ( ) )
            {
                _notifyIcon.Dispatcher.BeginInvoke ( new Action ( ( ) => ShowFancyBalloon ( title ,
                                                                                            text ,
                                                                                            visibilityBulbGreen ,
                                                                                            visibilityBulbYellow ,
                                                                                            visibilityBulbRed ) ) ) ;

                return ;
            }

            var balloon = new FancyBalloon
                          {
                              BalloonTitle         = title ,
                              BalloonText          = text ,
                              VisibilityBulbGreen  = visibilityBulbGreen ,
                              VisibilityBulbYellow = visibilityBulbYellow ,
                              VisibilityBulbRed    = visibilityBulbRed
                          } ;

            _notifyIcon.ShowCustomBalloon ( balloon ,
                                            PopupAnimation.Slide ,
                                            4000 ) ;
        }

        private readonly ILogger _logger ;

        private readonly      ISettingsManager        _manager ;
        private readonly      IDeskProvider           _provider ;
        [ CanBeNull ] private IDesk                   _desk ;
        private               TaskbarIcon             _notifyIcon ;
        private               CancellationToken       _token ;
        private               CancellationTokenSource _tokenSource ;
    }
}