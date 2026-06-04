using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Windows.Input ;
using Idasen.Aop ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using JetBrains.Annotations ;
using NHotkey ;
using NHotkey.Wpf ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public sealed partial class UiDeskManager : IUiDeskManager
{
    private const uint DeskHeightFactor = 100 ;

    private readonly IErrorManager                 _errorManager ;
    private readonly ITaskbarIconProvider          _iconProvider ;
    private readonly ILogger                       _logger ;
    private readonly ISettingsManager              _manager ;
    private readonly INotifications                _notifications ;
    private readonly IScheduler                    _scheduler ;
    private readonly IObserveSettingsChanges       _settingsChanges ;
    private readonly IHotkeyManager                _hotkeyManager ;
    private readonly IStatusBarManager             _statusBarManager ;
    private readonly IDeskMovementManager          _deskMovementManager ;
    private readonly IDeskConnectionManager        _deskConnectionManager ;

    private bool            _disposed ;
    private IDisposable ?   _finished ;
    private IDisposable ?   _heightChanged ;
    private NotifyIcon ?    _notifyIcon ;

    [ UsedImplicitly ] private IDisposable ? _onErrorChanged ;
    [ UsedImplicitly ] private IDisposable ? _onHotkeySettingsChanged ;

    private CancellationToken         _token ;
    private CancellationTokenSource ? _tokenSource ;

    public UiDeskManager (
        ILogger                       logger ,
        ISettingsManager              manager ,
        ITaskbarIconProvider          iconProvider ,
        INotifications                notifications ,
        IScheduler                    scheduler ,
        IErrorManager                 errorManager ,
        IObserveSettingsChanges       settingsChanges ,
        IHotkeyManager                hotkeyManager ,
        IStatusBarManager             statusBarManager ,
        IDeskMovementManager          deskMovementManager ,
        IDeskConnectionManager        deskConnectionManager )
    {
        Guard.ArgumentNotNull ( logger ,
                                nameof ( logger ) ) ;
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( iconProvider ,
                                nameof ( iconProvider ) ) ;
        Guard.ArgumentNotNull ( notifications ,
                                nameof ( notifications ) ) ;
        Guard.ArgumentNotNull ( scheduler ,
                                nameof ( scheduler ) ) ;
        Guard.ArgumentNotNull ( errorManager ,
                                nameof ( errorManager ) ) ;
        Guard.ArgumentNotNull ( settingsChanges ,
                                nameof ( settingsChanges ) ) ;
        Guard.ArgumentNotNull ( hotkeyManager ,
                                nameof ( hotkeyManager ) ) ;
        Guard.ArgumentNotNull ( statusBarManager ,
                                nameof ( statusBarManager ) ) ;
        Guard.ArgumentNotNull ( deskMovementManager ,
                                nameof ( deskMovementManager ) ) ;
        Guard.ArgumentNotNull ( deskConnectionManager ,
                                nameof ( deskConnectionManager ) ) ;

        _logger                = logger ;
        _manager               = manager ;
        _iconProvider          = iconProvider ;
        _notifications         = notifications ;
        _scheduler             = scheduler ;
        _errorManager          = errorManager ;
        _settingsChanges       = settingsChanges ;
        _hotkeyManager         = hotkeyManager ;
        _statusBarManager      = statusBarManager ;
        _deskMovementManager   = deskMovementManager ;
        _deskConnectionManager = deskConnectionManager ;
    }

    public IObservable < StatusBarInfo > StatusBarInfoChanged => _statusBarManager.StatusBarInfoChanged ;

    public bool IsInitialize => _notifyIcon is not null ;

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( UiDeskManager ) ) ;

        // Always attempt to dispose hotkey manager
        try
        {
            _hotkeyManager?.Dispose ( ) ;
        }
        catch
        {
            // ignore cleanup errors
        }

        // Attempt to cancel any pending operations, then clean up regardless
        try
        {
            _tokenSource?.Cancel ( ) ;
        }
        catch
        {
            // ignore cancellation errors
        }
        finally
        {
            // Dispose managed resources safely
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                _onErrorChanged?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            try
            {
                _onHotkeySettingsChanged?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            try
            {
                _heightChanged?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            try
            {
                _finished?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            try
            {
                ( _statusBarManager as IDisposable )?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            try
            {
                _deskConnectionManager?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            // Dispose token source
            try
            {
                _tokenSource?.Dispose ( ) ;
            }
            catch
            {
                // ignore cleanup errors
            }

            _onErrorChanged = null ;
            _onHotkeySettingsChanged = null ;
            _heightChanged  = null ;
            _finished       = null ;
            _tokenSource    = null ;
            _token          = CancellationToken.None ;
            _notifyIcon     = null ;
            // ReSharper restore EmptyGeneralCatchClause
        }
    }

    public bool IsConnected => _deskConnectionManager.IsConnected ;

    internal IDesk ? GetDesk ( ) => _deskConnectionManager.CurrentDesk ;

    public Task DisconnectAsync ( )
    {
        return _deskConnectionManager.DisconnectAsync ( ) ;
    }

    public Task StopAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _deskConnectionManager.CurrentDesk?.MoveStopAsync ( ) ) ;
    }

    public Task MoveLockAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _deskConnectionManager.CurrentDesk?.MoveLockAsync ( ) ) ;
    }

    public Task MoveUnlockAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _deskConnectionManager.CurrentDesk?.MoveUnlockAsync ( ) ) ;
    }

    public Task HideAsync ( )
    {
        if ( Application.Current.MainWindow is not null )
            Application.Current.MainWindow.Visibility = Visibility.Hidden ;

        return Task.CompletedTask ;
    }

    public Task ExitAsync ( )
    {
        try
        {
            Application.Current.Shutdown ( ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to shutdown application" ) ;
        }

        return Task.CompletedTask ;
    }

    public UiDeskManager Initialize ( NotifyIcon notifyIcon )
    {
        Guard.ArgumentNotNull ( notifyIcon ,
                                nameof ( notifyIcon ) ) ;

        _notifyIcon = notifyIcon ;

        _logger.Debug ( "UI Desk Manager Initializing..." ) ;

        _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
        _token       = _tokenSource.Token ;

        // Configure desk accessor for movement manager
        if ( _deskMovementManager is DeskMovementManager movementManager )
        {
            movementManager.SetDeskAccessor ( ( ) => _deskConnectionManager.CurrentDesk ) ;
        }

        // Subscribe to connection manager events
        _deskConnectionManager.DeskReady += OnDeskReady ;

        _onErrorChanged = _errorManager.ErrorChanged
                                       .ObserveOn ( _scheduler )
                                       .Subscribe ( OnErrorChanged ) ;

        _onHotkeySettingsChanged = _settingsChanges.HotkeySettingsChanged
                                                   .ObserveOn ( _scheduler )
                                                   .Subscribe ( OnHotkeySettingsChanged ) ;

        // Setup hotkey event handlers
        _hotkeyManager.StandingHotkeyPressed += OnStandingHotkeyPressed ;
        _hotkeyManager.SeatingHotkeyPressed  += OnSeatingHotkeyPressed ;
        _hotkeyManager.Custom1HotkeyPressed  += OnCustom1HotkeyPressed ;
        _hotkeyManager.Custom2HotkeyPressed  += OnCustom2HotkeyPressed ;

        // Do not register hotkeys here because persisted settings may not have been
        // loaded yet during startup. Hotkey registration must happen after the
        // settings load has completed so we do not apply default registrations for
        // users who have disabled global hotkeys in Settings.json.
        _logger.Debug ( "Deferring global hotkey registration until persisted settings are loaded" ) ;

        // Pass a cancellable token to allow clean shutdown
        _notifications.Initialize ( notifyIcon ,
                                    _token ) ;

        _ = AutoConnectAsync ( ) ;

        return this ;
    }

    private void RegisterGlobalHotkeys ( )
    {
        _hotkeyManager.RegisterGlobalHotkeys ( ) ;
    }

    public async Task StandAsync ( )
    {
        if ( ! _deskMovementManager.IsDeskAvailable ( ) )
            return ;

        var heightInCm = _manager.CurrentSettings.HeightSettings.StandingHeightInCm ;
        await _deskMovementManager.MoveToHeightAsync ( heightInCm , nameof ( StandAsync ) ) ;
    }

    public async Task SitAsync ( )
    {
        if ( ! _deskMovementManager.IsDeskAvailable ( ) )
            return ;

        var heightInCm = _manager.CurrentSettings.HeightSettings.SeatingHeightInCm ;
        await _deskMovementManager.MoveToHeightAsync ( heightInCm , nameof ( SitAsync ) ) ;
    }

    public async Task Custom1Async ( )
    {
        if ( ! _deskMovementManager.IsDeskAvailable ( ) )
            return ;

        var heightInCm = _manager.CurrentSettings.HeightSettings.Custom1HeightInCm ;
        await _deskMovementManager.MoveToHeightAsync ( heightInCm , nameof ( Custom1Async ) ) ;
    }

    public async Task Custom2Async ( )
    {
        if ( ! _deskMovementManager.IsDeskAvailable ( ) )
            return ;

        var heightInCm = _manager.CurrentSettings.HeightSettings.Custom2HeightInCm ;
        await _deskMovementManager.MoveToHeightAsync ( heightInCm , nameof ( Custom2Async ) ) ;
    }

    public async Task AutoConnectAsync ( )
    {
        _logger.Debug ( "Auto connecting..." ) ;

        try
        {
            CheckIfInitialized ( ) ;

            // Ensure a new CancellationTokenSource is created if the previous one is canceled or disposed
            if ( _tokenSource == null ||
                 _tokenSource.IsCancellationRequested )
            {
                _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
                _token       = _tokenSource.Token ;
            }

            _logger.Debug ( "Trying to load settings..." ) ;

            await _manager.LoadAsync ( CancellationToken.None ).ConfigureAwait ( false ) ;

            // Register global hotkeys after settings are loaded if they are enabled
            var hotkeySettings = _manager.CurrentSettings.HotkeySettings ;
            if ( hotkeySettings.GlobalHotkeysEnabled )
            {
                _logger.Debug ( "Settings loaded with global hotkeys enabled. Registering hotkeys..." ) ;

                // Hotkey registration must happen on the UI thread
                var dispatcher = Application.Current?.Dispatcher ;
                if ( dispatcher != null )
                {
                    await dispatcher.InvokeAsync ( ( ) =>
                    {
                        try
                        {
                            RegisterGlobalHotkeys ( ) ;
                        }
                        catch ( Exception e )
                        {
                            _logger.Error ( e ,
                                            "Failed to register hotkeys during startup" ) ;
                        }
                    } ) ;
                }
                else
                {
                    _logger.Warning ( "Cannot access UI dispatcher for hotkey registration during startup" ) ;
                }
            }
            else
            {
                _logger.Debug ( "Settings loaded with global hotkeys disabled. Skipping hotkey registration." ) ;
            }

            _logger.Debug ( "Trying to auto connect to Idasen Desk..." ) ;

            await Task.Delay ( 3000 ,
                               _token ).ConfigureAwait ( false ) ;

            _notifications.Show ( "Auto Connect" ,
                                  "Trying to auto connect to Idasen Desk..." ,
                                  InfoBarSeverity.Informational ) ;

            await _deskConnectionManager.ConnectAsync ( _token ).ConfigureAwait ( false ) ;
        }
        catch ( TaskCanceledException e )
        {
            _logger.Warning ( e ,
                              "Auto connect was canceled" ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to auto connect to desk" ) ;
            _errorManager.PublishForMessage ( "Failed to connect" ) ;
        }
    }

    // Helper to run an action only when connected
    private Task ExecuteIfConnected ( Action action )
    {
        if ( ! _deskConnectionManager.IsConnected )
            return Task.CompletedTask ;

        try
        {
            action ( ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Action failed while connected" ) ;
        }

        return Task.CompletedTask ;
    }

    private async Task ExecuteWithErrorHandlingAsync ( string methodName , Func < Task > action )
    {
        try
        {
            _logger.Debug ( "{MethodName}" ,
                            methodName ) ;
            await action ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to call {MethodName}" ,
                            methodName ) ;
            _errorManager.PublishForMessage ( $"Failed to call {methodName}" ) ;
        }
    }

    private static uint MmToCm ( uint height )
    {
        // Use double division and specify midpoint rounding to avoid ambiguous overloads and banker's rounding
        return ( uint )Math.Round ( height / ( double )DeskHeightFactor ,
                                    MidpointRounding.AwayFromZero ) ;
    }

    private void OnDeskReady ( object ? sender , IDesk desk )
    {
        _logger.Debug ( "Desk ready event received, setting up subscriptions..." ) ;

        _finished = desk.FinishedChanged
                        .ObserveOn ( Scheduler.Default )
                        .SubscribeAsync ( OnFinishedChanged ) ;

        _heightChanged = desk.HeightChanged
                              .ObserveOn ( Scheduler.Default )
                              .Throttle ( TimeSpan.FromSeconds ( 1 ) )
                              .SubscribeAsync ( OnHeightChanged ) ;

        _iconProvider.Initialize ( _logger ,
                                   desk ,
                                   _notifyIcon ) ;

        var message = $"Connected successfully to '{desk.DeviceName}'." ;

        OnStatusChanged ( 0 ,
                          "Connected" ,
                          message ,
                          InfoBarSeverity.Success ) ;
    }

    private void OnErrorChanged ( IErrorDetails details )
    {
        var deviceName = _deskConnectionManager.CurrentDesk?.DeviceName ?? "Unknown" ;
        var msg        = $"[{deviceName}] {details.Message}" ;

        _logger.Error ( "Desk error: {Message}" ,
                        msg ) ;

        OnStatusChanged ( 0 ,
                          "Error" ,
                          msg ,
                          InfoBarSeverity.Error ) ;
    }

    private void OnStandingHotkeyPressed ( object ? sender , EventArgs e )
    {
        _ = ExecuteWithErrorHandlingAsync ( nameof ( StandAsync ) ,
                                            StandAsync ) ;
    }

    private void OnSeatingHotkeyPressed ( object ? sender , EventArgs e )
    {
        _ = ExecuteWithErrorHandlingAsync ( nameof ( SitAsync ) ,
                                            SitAsync ) ;
    }

    private void OnCustom1HotkeyPressed ( object ? sender , EventArgs e )
    {
        _ = ExecuteWithErrorHandlingAsync ( nameof ( Custom1Async ) ,
                                            Custom1Async ) ;
    }

    private void OnCustom2HotkeyPressed ( object ? sender , EventArgs e )
    {
        _ = ExecuteWithErrorHandlingAsync ( nameof ( Custom2Async ) ,
                                            Custom2Async ) ;
    }

    private void OnHotkeySettingsChanged ( bool enabled )
    {
        _logger.Information ( "Hotkey settings changed event received. Enabled: {Enabled}" ,
                              enabled ) ;

        // Hotkey registration must happen on the UI thread
        var dispatcher = Application.Current?.Dispatcher ;
        if ( dispatcher == null )
        {
            _logger.Error ( "Cannot access UI dispatcher for hotkey registration" ) ;
            return ;
        }

        dispatcher.Invoke ( ( ) =>
        {
            try
            {
                if ( enabled )
                {
                    _logger.Information ( "Registering hotkeys due to settings change" ) ;
                    _hotkeyManager.RegisterGlobalHotkeys ( ) ;
                }
                else
                {
                    _logger.Information ( "Unregistering hotkeys due to settings change" ) ;
                    _hotkeyManager.UnregisterGlobalHotkeys ( ) ;
                }
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to handle hotkey settings change" ) ;
            }
        } ) ;
    }

    private static string HeightMessage ( uint heightInCm )
    {
        return $"Desk height is {heightInCm} cm" ;
    }

    private async Task NotifyAndPersistHeightAsync ( uint heightInCm , string title , InfoBarSeverity severity )
    {
        OnStatusChanged ( heightInCm ,
                          title ,
                          HeightMessage ( heightInCm ) ,
                          severity ) ;

        await _manager.SetLastKnownDeskHeight ( heightInCm ,
                                                CancellationToken.None ).ConfigureAwait ( false ) ;
    }

    private async Task OnFinishedChanged ( uint height )
    {
        var heightInCm = MmToCm ( height ) ;
        await NotifyAndPersistHeightAsync ( heightInCm ,
                                            "Finished" ,
                                            InfoBarSeverity.Success ).ConfigureAwait ( false ) ;
    }

    private async Task OnHeightChanged ( uint height )
    {
        var heightInCm = MmToCm ( height ) ;
        await NotifyAndPersistHeightAsync ( heightInCm ,
                                            "Height Changed" ,
                                            InfoBarSeverity.Warning ).ConfigureAwait ( false ) ;
    }

    private void OnStatusChanged (
        uint            height   = 0 ,
        string          title    = "" ,
        string          message  = "" ,
        InfoBarSeverity severity = InfoBarSeverity.Informational )
    {
        _logger.Debug ( "Status update Height={Height} Title={Title} Message={Message} Severity={Severity}" ,
                        height ,
                        title ,
                        message ,
                        severity ) ;

        if ( height == 0 ) height = _manager.CurrentSettings.HeightSettings.LastKnownDeskHeight ;

        var info = new StatusBarInfo ( title ,
                                       height ,
                                       message ,
                                       severity ) ;

        // Publish to observers outside the lock to avoid "used inside a lock" warnings and potential deadlocks
        _statusBarManager.UpdateStatus ( info ) ;

        _notifications.Show ( title ,
                              message ,
                              severity ) ;
    }

    private void CheckIfInitialized ( )
    {
        if ( ! IsInitialize )
            throw new InvalidOperationException ( "Initialize needs to be called first!" ) ;
    }
}