using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
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
            [ NotNull ] IDeskProvider    provider ,
            [ NotNull ] IScheduler       scheduler ,
            [ NotNull ] IErrorManager    errorManager )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( errorManager ,
                                    nameof ( errorManager ) ) ;

            _scheduler = scheduler ;
        }

        public void Dispose ( )
        {
            _logger.Information ( "Disposing..." ) ;

            _tokenSource?.Cancel ( ) ;

            DisposeDesk ( ) ;

            _provider?.Dispose ( ) ;
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
        ///     Disconnects from the Idasen Desk.
        /// </summary>
        public ICommand DisconnectCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction  = Disconnect ,
                           CanExecuteFunc = ( ) => _desk != null
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
                           CommandAction  = async ( ) => { await Standing ( ) ; } ,
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

        private void OnErrorChanged ( IErrorDetails details )
        {
            _logger.Error ( $"[{_desk?.DeviceName}] {details.Message}" ) ;

            ShowFancyBalloon ( "Error" ,
                               details.Message     +
                               Environment.NewLine +
                               details.Caller ,
                               visibilityBulbRed: Visibility.Visible ) ;
        }

        private async Task Standing ( )
        {
            await _manager.Load ( ) ;

            _desk?.MoveTo ( _manager.CurrentSettings.StandingHeightInCm * 100 ) ;
        }

        public NotifyIconViewModel Initialize (
            [ NotNull ] ILogger          logger ,
            [ NotNull ] ISettingsManager manager ,
            [ NotNull ] IDeskProvider    provider ,
            [ NotNull ] IErrorManager    errorManager )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;
            Guard.ArgumentNotNull ( errorManager ,
                                    nameof ( errorManager ) ) ;

            _logger       = logger ;
            _manager      = manager ;
            _provider     = provider ;

            _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            _token       = _tokenSource.Token ;

            _onErrorChanged = errorManager.ErrorChanged
                                          .ObserveOn ( _scheduler )
                                          .Subscribe ( OnErrorChanged ) ;

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
                                   "Trying to auto connect to Idasen Desk..." ,
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
                _logger.Debug ( $"[{_desk?.DeviceName}] Trying to connect to Idasen Desk..." ) ;

                DisposeDesk ( ) ;

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
                                $"[{_desk?.DeviceName}] Failed to connect" ) ;

                ConnectFailed ( ) ;
            }
        }

        private void Disconnect ( )
        {
            try
            {
                _logger.Debug ( $"[{_desk?.DeviceName}] Trying to disconnect from Idasen Desk..." ) ;

                DisposeDesk ( ) ;

                _tokenSource?.Cancel ( false ) ;

                _logger.Debug ( $"[{_desk?.DeviceName}] ...disconnected from Idasen Desk" ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to disconnect" ) ;

                ConnectFailed ( ) ;
            }
        }

        private void ConnectFailed ( )
        {
            ShowFancyBalloon ( "Failed to Connect" ,
                               BluetoothLE.Characteristics.Common.Constants.CheckAndEnableBluetooth ,
                               visibilityBulbRed : Visibility.Visible ) ;
        }

        private void DisposeDesk ( )
        {
            _finished?.Dispose ( ) ;
            _desk?.Dispose ( ) ;

            _desk     = null ;
            _finished = null ;
        }

        private void ConnectSuccessful ( IDesk desk )
        {
            _logger.Information ( $"[{desk.DeviceName}] Connected to {desk.DeviceName} with address {desk.BluetoothAddress}" ) ;

            _desk = desk ;

            _finished = _desk.FinishedChanged
                             .ObserveOn ( _scheduler )
                             .Subscribe ( OnFinishedChanged ) ;

            ShowFancyBalloon ( "Success" ,
                               "Connected to desk: " +
                               Environment.NewLine   +
                               $"'{desk.Name}'" ,
                               Visibility.Visible ) ;

            _logger.Debug ( $"[{_desk?.DeviceName}] Connected successful" ) ;
        }

        private void OnFinishedChanged ( uint height )
        {
            ShowFancyBalloon ( "Finished" ,
                               $"Desk height is {height / 100:F0} cm" ,
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

        private readonly IScheduler _scheduler = Scheduler.CurrentThread ;

        [ CanBeNull ] private      IDesk                   _desk ;
        private                    IDisposable             _finished ;
        private                    ILogger                 _logger ;
        private                    ISettingsManager        _manager ;
        private                    TaskbarIcon             _notifyIcon ;
        [ UsedImplicitly ] private IDisposable             _onErrorChanged ;
        private                    IDeskProvider           _provider ;
        private                    CancellationToken       _token ;
        private                    CancellationTokenSource _tokenSource ;
    }
}