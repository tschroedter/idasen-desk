using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Windows.Input ;
using Autofac ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using JetBrains.Annotations ;
using NHotkey ;
using NHotkey.Wpf ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;
using MessageBox = System.Windows.MessageBox ;
using ILogger = Serilog.ILogger ;

namespace Idasen.SystemTray.Win11.Utils ;

public class UiDeskManager : IUiDeskManager
{
    private static readonly KeyGesture IncrementGesture = new(Key.Up , ModifierKeys.Control   | ModifierKeys.Alt | ModifierKeys.Shift) ;
    private static readonly KeyGesture DecrementGesture = new(Key.Down , ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) ;

    private readonly ITaskbarIconProvider      _iconProvider ;
    private readonly ISettingsManager ?        _manager ;
    private readonly INotifications            _notifications ;
    private readonly Subject < StatusBarInfo > _statusBarInfoSubject ;

    private IDesk ?         _desk ;
    private IDeskProvider ? _deskProvider ;
    private IErrorManager ? _errorManager ;
    private IDisposable ?   _finished ;
    private IDisposable ?   _heightChanged ;
    private ILogger ?       _logger ;
    private NotifyIcon ?    _notifyIcon ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    private Func < IDeskProvider > ?  _providerFactory ;
    private CancellationToken ?       _token ;
    private CancellationTokenSource ? _tokenSource ;

    public UiDeskManager ( ISettingsManager     manager ,
                           ITaskbarIconProvider iconProvider ,
                           INotifications       notifications )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( iconProvider ,
                                nameof ( iconProvider ) ) ;
        Guard.ArgumentNotNull ( notifications ,
                                nameof ( notifications ) ) ;

        _manager              = manager ;
        _iconProvider         = iconProvider ;
        _notifications        = notifications ;
        _statusBarInfoSubject = new Subject < StatusBarInfo > ( ) ;
    }

    public IObservable < StatusBarInfo > StatusBarInfoChanged => _statusBarInfoSubject ;

    public bool IsInitialize => _logger != null && _manager != null ;

    public void Dispose ( )
    {
        _logger?.Information ( "Disposing..." ) ;

        DisposeDesk ( ) ;

        _heightChanged?.Dispose ( ) ;
        _finished?.Dispose ( ) ;
        _onErrorChanged?.Dispose ( ) ;
        _deskProvider?.Dispose ( ) ;
        _tokenSource?.Dispose ( ) ;
    }

    public bool IsConnected => _desk != null ;

    public UiDeskManager Initialize ( IContainer container , NotifyIcon notifyIcon )
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;
        Guard.ArgumentNotNull ( notifyIcon ,
                                nameof ( notifyIcon ) ) ;

        _notifyIcon = notifyIcon ;

        _logger          = container.Resolve < ILogger > ( ) ;
        _providerFactory = container.Resolve < Func < IDeskProvider > > ( ) ;
        _errorManager    = container.Resolve < IErrorManager > ( ) ;

        _logger?.Debug ( "UI Desk ManagerInitializing..." ) ;

        _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
        _token       = _tokenSource.Token ;

        _onErrorChanged = _errorManager.ErrorChanged
                                       .ObserveOn ( Scheduler.Default )
                                       .Subscribe ( OnErrorChanged ) ;


        HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered ;

        HotkeyManager.Current.AddOrReplace ( "Increment" , IncrementGesture , OnGlobalHotKeyStanding ) ;
        HotkeyManager.Current.AddOrReplace ( "Decrement" , DecrementGesture , OnGlobalHotKeySeating ) ;

        _notifications.Initialize ( container , notifyIcon ) ;

        // ReSharper disable once AsyncVoidLambda
        Task.Run ( new Action ( async ( ) => { await AutoConnectAsync ( ) ; } ) ) ;

        return this ;
    }

    public async Task StandAsync ( )
    {
        _logger?.Debug ( "Executing Stand..." ) ;

        if ( ! IsDeskConnected ( ) )
            return ;

        await _manager!.LoadAsync ( ) ;

        _desk?.MoveTo ( _manager.CurrentSettings.HeightSettings.StandingHeightInCm * 100 ) ;
    }

    public async Task SitAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
            return ;

        await _manager!.LoadAsync ( )
                       .ConfigureAwait ( false ) ;

        _desk?.MoveTo ( _manager.CurrentSettings.HeightSettings.SeatingHeightInCm *
                        100 ) ; // todo duplicate
    }

    public async Task AutoConnectAsync ( )
    {
        _logger?.Debug ( "Auto connecting..." ) ;

        try
        {
            CheckIfInitialized ( ) ;

            _logger?.Debug ( "Trying to load settings..." ) ;

            if ( _manager == null )
                throw new Exception ( "Manager is null" ) ;

            await _manager.LoadAsync ( ) ;

            _logger?.Debug ( "Trying to auto connect to Idasen Desk..." ) ;

            if ( _token == null )
                throw new Exception ( "Token is null" ) ;

            await Task.Delay ( TimeSpan.FromSeconds ( 3 ) ,
                               _token.Value ) ;

            _notifications.Show ( "Auto Connect" ,
                                  "Trying to auto connect to Idasen Desk..." ,
                                  InfoBarSeverity.Informational ) ;

            await Connect ( ) ;
        }
        catch ( TaskCanceledException )
        {
            _logger?.Information ( "Auto connect was canceled" ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             "Failed to auto connect to desk" ) ;

            ConnectFailed ( ) ;
        }
    }

    public Task DisconnectAsync ( )
    {
        if ( IsDeskConnected ( ) )
        {
            DoDisconnectAsync ( ) ;
        }

        return Task.CompletedTask ;
    }

    public Task StopAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
        {
            return Task.CompletedTask ;
        }

        _desk?.MoveStop ( ) ;

        return Task.CompletedTask ;
    }

    public Task MoveLockAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
        {
            return Task.CompletedTask ;
        }

        _desk?.MoveLock ( ) ;

        return Task.CompletedTask ;
    }

    public Task MoveUnlockAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
        {
            return Task.CompletedTask ;
        }

        _desk?.MoveUnlock ( ) ;

        return Task.CompletedTask ;
    }

    public Task HideAsync ( )
    {
        if ( Application.Current.MainWindow != null )
            Application.Current.MainWindow.Visibility = Visibility.Hidden ;

        return Task.CompletedTask ;
    }

    public StatusBarInfo LastStatusBarInfo { get ; private set ; } = new("" ,
                                                                         0 ,
                                                                         "Unknown" ,
                                                                         InfoBarSeverity.Informational) ;

    public Task ExitAsync ( )
    {
        Application.Current.Shutdown ( ) ;

        return Task.CompletedTask ;
    }

    private void DoDisconnectAsync ( )
    {
        try
        {
            _logger?.Debug ( $"[{_desk?.DeviceName}] Trying to disconnect from Idasen Desk..." ) ;

            DisposeDesk ( ) ;

            _logger?.Debug ( $"[{_desk?.DeviceName}] ...disconnected from Idasen Desk" ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             "Failed to disconnect" ) ;

            ConnectFailed ( ) ;
        }
    }

    private void HotkeyManager_HotkeyAlreadyRegistered ( object ? sender , HotkeyAlreadyRegisteredEventArgs e )
    {
        MessageBox.Show ( $"The hotkey {e.Name} is already registered by another application" ) ;
    }

    private void OnErrorChanged ( IErrorDetails details )
    {
        var deviceName = _desk != null
                             ? _desk.DeviceName
                             : "Unknown" ;

        var message = $"[{deviceName}] {details.Message}" ;

        _logger?.Error ( message ) ;

        OnStatusChanged ( 0 ,
                          "Error" ,
                          message,
                          InfoBarSeverity.Error ) ;
    }


    private void OnGlobalHotKeyStanding ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger?.Information ( "Received global hot key for 'Stand' command..." ) ;

            Task.Run ( async ( ) => await DoStandingAsync ( ) ) ;
        }
        catch ( Exception exception )
        {
            _logger?.Error ( exception , "Failed to handle global hot key command for 'Stand'." ) ;
        }
    }

    private void OnGlobalHotKeySeating ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger?.Information ( "Received global hot key for 'Sit' command..." ) ;

            Task.Run ( async ( ) => await DoSeatingAsync ( ) ) ;
        }
        catch ( Exception exception )
        {
            _logger?.Error ( exception , "Failed to handle global hot key command for 'Sit'." ) ;
        }
    }

    private async Task DoStandingAsync ( )
    {
        try
        {
            _logger?.Debug ( $"{nameof ( DoStandingAsync )}" ) ;

            await StandAsync ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( DoStandingAsync )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( DoStandingAsync )}" ) ;
        }
    }

    private async Task DoSeatingAsync ( )
    {
        try
        {
            _logger?.Debug ( $"{nameof ( DoSeatingAsync )}" ) ;

            await SitAsync ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( DoSeatingAsync )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( DoSeatingAsync )}" ) ;
        }
    }

    private bool IsDeskConnected ( )
    {
        if ( _desk    != null &&
             _manager != null )
            return true ;

        var message = "Failed to connect to desk!" ;

        _logger?.Error ( message ) ;

        OnStatusChanged ( 0 ,
                          "Not Connected" ,
                          message ,
                          InfoBarSeverity.Error ) ;

        return false ;
    }

    private async Task Connect ( )
    {
        try
        {
            _logger?.Debug ( "Trying to initialize provider..." ) ;

            if ( _manager == null )
                throw new Exception ( "SettingsManager is null" ) ;

            _deskProvider?.Dispose ( ) ;
            _deskProvider = _providerFactory! ( ) ;
            _deskProvider.Initialize ( _manager!.CurrentSettings.DeviceSettings.DeviceName ,
                                       _manager!.CurrentSettings.DeviceSettings.DeviceAddress ,
                                       _manager!.CurrentSettings.DeviceSettings.DeviceMonitoringTimeout ) ;

            _logger?.Debug ( $"[{_desk?.DeviceName}] Trying to connect to Idasen Desk..." ) ;

            if ( _token == null )
                throw new Exception ( "Token is null" ) ;

            var (isSuccess , desk) = await _deskProvider.TryGetDesk ( _token.Value ) ;

            if ( isSuccess )
                ConnectSuccessful ( desk! ) ;
            else
                ConnectFailed ( ) ;
        }
        catch ( Exception e )
        {
            var deviceName = _desk != null
                                 ? _desk.DeviceName
                                 : "Unknown" ;

            _logger?.Error ( e ,
                             $"[{deviceName}] Failed to connect") ;

            ConnectFailed ( ) ;
        }
    }

    private void CheckIfInitialized ( )
    {
        if ( ! IsInitialize )
            throw new Exception ( "Initialize needs to be called first!" ) ;
    }


    private void ConnectFailed ( )
    {
        _logger?.Debug ( "Connection failed..." ) ;

        DisconnectAsync ( ) ;

        _errorManager?.PublishForMessage ( "Failed to connect" ) ; // todo
    }

    private void DisposeDesk ( )
    {
        _logger?.Debug ( $"[{_desk?.Name}] Disposing desk" ) ;

        _finished?.Dispose ( ) ;
        _desk?.Dispose ( ) ;
        _deskProvider?.Dispose ( ) ;

        _finished     = null ;
        _desk         = null ;
        _deskProvider = null ;
    }

    private void ConnectSuccessful ( IDesk desk )
    {
        _logger?.Information ( $"[{desk.DeviceName}] Connected to {desk.DeviceName} " +
                               $"with address {desk.BluetoothAddress} "               +
                               $"(MacAddress {desk.BluetoothAddress.ToMacAddress ( )})" ) ;

        _desk = desk ;

        _finished = _desk.FinishedChanged
                         .ObserveOn ( Scheduler.Default )
                         .Subscribe ( OnFinishedChanged ) ;

        _heightChanged = _desk.HeightChanged
                              .ObserveOn ( Scheduler.Default )
                              .Throttle ( TimeSpan.FromSeconds ( 1 ) )
                              .Subscribe ( OnHeightChanged ) ;

        _iconProvider.Initialize ( _logger! ,
                                   _desk ,
                                   _notifyIcon ) ;

        var message = $"Connected successfully to '{_desk?.DeviceName}'." ;

        OnStatusChanged ( 0 ,
                          "Connected" ,
                          message ,
                          InfoBarSeverity.Success ) ;

        if ( ! _manager?.CurrentSettings.DeviceSettings.DeviceLocked ?? true )
            return ;

        _logger?.Information ( "Locking desk movement" ) ;

        _desk?.MoveLock ( ) ;
    }

    private void OnFinishedChanged ( uint height )
    {
        var heightInCm = ( uint ) Math.Round ( height / 100.0 ) ;

        OnStatusChanged ( heightInCm ,
                          "Finished" ,
                          $"Desk height is {heightInCm} cm" ,
                          InfoBarSeverity.Success ) ;
    }

    private void OnHeightChanged ( uint height )
    {
        var heightInCm = ( uint ) Math.Round ( height / 100.0 ) ;

        var message = $"Desk height is {heightInCm} cm" ;

        OnStatusChanged ( heightInCm ,
                          "Height Changed" ,
                          message ,
                          InfoBarSeverity.Warning ) ;
    }

    private void OnStatusChanged ( uint            height     = 0 ,
                                   string          title      = "" ,
                                   string          message    = "" ,
                                   InfoBarSeverity severity   = InfoBarSeverity.Informational )
    {
        _logger?.Debug ( $"{nameof ( height )} = {height}, "   +
                         $"{nameof ( title )} = {title}, "     +
                         $"{nameof ( message )} = {message}, " +
                         $"{nameof ( severity )} = {severity}" ) ;

        LastStatusBarInfo = new StatusBarInfo ( title ,
                                                height ,
                                                message ,
                                                severity ) ;

        _statusBarInfoSubject.OnNext ( LastStatusBarInfo ) ;

        _notifications.Show ( title ,
                              message ,
                              severity) ;
    }
}