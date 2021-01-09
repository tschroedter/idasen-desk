using System ;
using System.Threading ;
using System.Threading.Tasks ;
using System.Windows ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;
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
        : IDisposable
    {
        public NotifyIconViewModel ( )
        {
        }

        public NotifyIconViewModel (
            [ NotNull ] ILogger          logger ,
            [ NotNull ] ISettingsManager manager ,
            [ NotNull ] IDeskProvider    provider )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;
        }

        public void Dispose ( )
        {
            _logger.Information ( "Disposing..." ) ;

            _tokenSource?.Cancel ( ) ;

            _provider?.Dispose ( ) ;
            _desk?.Dispose ( ) ;
            _notifyIcon?.Dispose ( ) ;
            _tokenSource?.Dispose ( ) ;
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
                                               await _manager.Load ( ) ;

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
                                               await _manager.Load ( ) ;

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
            get
            {
                return new DelegateCommand
                       {
                           CommandAction = ( ) =>
                                           {
                                               _logger.Information ( "##### Exit..." ) ;

                                               _tokenSource.Cancel ( ) ;
                                               Application.Current.Shutdown ( ) ;
                                           }
                       } ;
            }
        }

        [ CanBeNull ]
        private ISettingsWindow SettingsWindow
        {
            get => Application.Current.MainWindow as ISettingsWindow ;
            set => Application.Current.MainWindow = value as Window ;
        }

        public bool IsInitialize => _logger != null && _manager != null && _provider != null ;

        public NotifyIconViewModel Initialize (
            [ NotNull ] ILogger          logger ,
            [ NotNull ] ISettingsManager manager ,
            [ NotNull ] IDeskProvider    provider )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;

            _logger   = logger ;
            _manager  = manager ;
            _provider = provider ;

            _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            _token       = _tokenSource.Token ;

            return this ;
        }

        public async Task AutoConnect ( )
        {
            try
            {
                CheckIfInitialized ( ) ;

                _logger.Debug ( "Trying to load settings..." ) ;

                await _manager.Load ( ) ;

                _logger.Debug ( "Trying to initialize provider..." ) ;

                _provider.Initialize ( _manager.CurrentSettings.DeviceName ,
                                       _manager.CurrentSettings.DeviceAddress ,
                                       _manager.CurrentSettings.DeviceMonitoringTimeout ) ;

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
                _logger.Information ( "Auto connect was canceled" ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to auto connect to desk" ) ;

                ConnectFailed ( ) ;
            }
        }

        private void CheckIfInitialized ( )
        {
            if ( ! IsInitialize )
                throw new Exception ( "Initialize needs to be called first!" ) ;
        }

        private async Task Connect ( )
        {
            try
            {
                _logger.Debug ( "Trying to connect to Idasen Desk..." ) ;

                _desk?.Dispose ( ) ;

                _tokenSource?.Cancel ( false ) ;

                _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
                _token       = _tokenSource.Token ;

                var (isSuccess , desk) = await _provider.TryGetDesk ( _token ) ;

                if ( isSuccess )
                    ConnectSuccessful ( desk ) ;
                else
                    ConnectFailed ( ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to connect" ) ;

                ConnectFailed ( ) ;
            }
        }

        private void ConnectFailed ( )
        {
            _desk = null ;

            ShowFancyBalloon ( "Failed" ,
                               "Connection to desk failed" ,
                               visibilityBulbRed : Visibility.Visible ) ;
        }

        private void ConnectSuccessful ( IDesk desk )
        {
            _logger.Information ( $"[{desk.DeviceName}] Connected to {desk.DeviceName} with address {desk.BluetoothAddress}" ) ;

            _desk = desk ;

            ShowFancyBalloon ( "Success" ,
                               $"Connected to desk {desk.Name}" ,
                               Visibility.Visible ) ;
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

        [ CanBeNull ] private IDesk                   _desk ;
        private               ILogger                 _logger ;
        private               ISettingsManager        _manager ;
        private               TaskbarIcon             _notifyIcon ;
        private               IDeskProvider           _provider ;
        private               CancellationToken       _token ;
        private               CancellationTokenSource _tokenSource ;
    }
}