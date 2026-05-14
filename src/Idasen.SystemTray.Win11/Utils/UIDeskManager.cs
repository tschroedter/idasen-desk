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

    private const string HotkeyNameStanding = "Standing" ;
    private const string HotkeyNameSeating  = "Seating" ;
    private const string HotkeyNameCustom1  = "Custom 1" ;
    private const string HotkeyNameCustom2  = "Custom 2" ;

    private static readonly KeyGesture StandingGesture = new(Key.Up ,
                                                             ModifierKeys.Control | ModifierKeys.Alt |
                                                             ModifierKeys.Shift) ;

    private static readonly KeyGesture SittingGesture = new(Key.Down ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly KeyGesture Custom1Gesture = new(Key.Left ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly KeyGesture Custom2Gesture = new(Key.Right ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly char [ ] ModifierSeparators = [ ',' , ' ' ] ;

    private readonly IErrorManager             _errorManager ;
    private readonly ITaskbarIconProvider      _iconProvider ;
    private readonly ILogger                   _logger ;
    private readonly ISettingsManager          _manager ;
    private readonly INotifications            _notifications ;
    private readonly Func < IDeskProvider > ?  _providerFactory ;
    private readonly IScheduler                _scheduler ;
    private readonly IObserveSettingsChanges   _settingsChanges ;
    private readonly Subject < StatusBarInfo > _statusBarInfoSubject ;

    private IDesk ?         _desk ;
    private IDeskProvider ? _deskProvider ;
    private bool            _disposed ;
    private IDisposable ?   _finished ;
    private IDisposable ?   _heightChanged ;
    private NotifyIcon ?    _notifyIcon ;

    [ UsedImplicitly ] private IDisposable ? _onErrorChanged ;
    [ UsedImplicitly ] private IDisposable ? _onHotkeySettingsChanged ;

    private CancellationToken         _token ;
    private CancellationTokenSource ? _tokenSource ;

    public UiDeskManager (
        ILogger                  logger ,
        ISettingsManager         manager ,
        ITaskbarIconProvider     iconProvider ,
        INotifications           notifications ,
        IScheduler               scheduler ,
        Func < IDeskProvider >   deskProviderFactory ,
        IErrorManager            errorManager ,
        IObserveSettingsChanges  settingsChanges )
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
        Guard.ArgumentNotNull ( deskProviderFactory ,
                                nameof ( deskProviderFactory ) ) ;
        Guard.ArgumentNotNull ( errorManager ,
                                nameof ( errorManager ) ) ;
        Guard.ArgumentNotNull ( settingsChanges ,
                                nameof ( settingsChanges ) ) ;

        _logger               = logger ;
        _manager              = manager ;
        _iconProvider         = iconProvider ;
        _notifications        = notifications ;
        _scheduler            = scheduler ;
        _providerFactory      = deskProviderFactory ;
        _errorManager         = errorManager ;
        _settingsChanges      = settingsChanges ;
        _statusBarInfoSubject = new Subject < StatusBarInfo > ( ) ;
    }

    public IObservable < StatusBarInfo > StatusBarInfoChanged => _statusBarInfoSubject ;

    public bool IsInitialize => _notifyIcon is not null ;

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( UiDeskManager ) ) ;

        // Always attempt to unregister hotkeys and event handlers
        try
        {
            _logger.Debug ( "Unregistering global hotkeys..." ) ;
            HotkeyManager.HotkeyAlreadyRegistered -= HotkeyManager_HotkeyAlreadyRegistered ;
            HotkeyManager.Current.Remove ( HotkeyNameStanding ) ;
            HotkeyManager.Current.Remove ( HotkeyNameSeating ) ;
            HotkeyManager.Current.Remove ( HotkeyNameCustom1 ) ;
            HotkeyManager.Current.Remove ( HotkeyNameCustom2 ) ;
        }
        catch
        {
            // ignore cleanup errors - hotkeys may not have been registered
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
                _statusBarInfoSubject.OnCompleted ( ) ;
                _statusBarInfoSubject.Dispose ( ) ;
            }
            catch
            {
                // ignore subject disposal errors
            }

            // Dispose desk and provider
            try
            {
                DisposeDesk ( ) ;
            }
            catch
            {
                // ignore desk disposal errors
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
            _deskProvider   = null ;
            _desk           = null ;
            _tokenSource    = null ;
            _token          = CancellationToken.None ;
            _notifyIcon     = null ;
            // ReSharper restore EmptyGeneralCatchClause
        }
    }

    public bool IsConnected => _desk is not null ;

    public Task DisconnectAsync ( )
    {
        if ( IsDeskConnected ( ) ) DoDisconnectAsync ( ) ;

        return Task.CompletedTask ;
    }

    public Task StopAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _desk?.MoveStopAsync ( ) ) ;
    }

    public Task MoveLockAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _desk?.MoveLockAsync ( ) ) ;
    }

    public Task MoveUnlockAsync ( )
    {
        return ExecuteIfConnected ( ( ) => _desk?.MoveUnlockAsync ( ) ) ;
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

        _onErrorChanged = _errorManager.ErrorChanged
                                       .ObserveOn ( _scheduler )
                                       .Subscribe ( OnErrorChanged ) ;

        _onHotkeySettingsChanged = _settingsChanges.HotkeySettingsChanged
                                                   .ObserveOn ( _scheduler )
                                                   .Subscribe ( OnHotkeySettingsChanged ) ;

        HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered ;

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
        var hotkeySettings = _manager.CurrentSettings.HotkeySettings ;

        _logger.Information ( "Registering global hotkeys..." ) ;

        // Create gestures from settings or use defaults
        var standingGesture = CreateKeyGesture ( hotkeySettings.StandingKey ,
                                                hotkeySettings.StandingModifiers ,
                                                StandingGesture ) ;

        var seatingGesture = CreateKeyGesture ( hotkeySettings.SeatingKey ,
                                               hotkeySettings.SeatingModifiers ,
                                               SittingGesture ) ;

        var custom1Gesture = CreateKeyGesture ( hotkeySettings.Custom1Key ,
                                               hotkeySettings.Custom1Modifiers ,
                                               Custom1Gesture ) ;

        var custom2Gesture = CreateKeyGesture ( hotkeySettings.Custom2Key ,
                                               hotkeySettings.Custom2Modifiers ,
                                               Custom2Gesture ) ;

        // Register hotkeys - safely handle if they don't exist
        SafeAddOrReplaceHotkey ( HotkeyNameStanding , standingGesture , OnGlobalHotKeyStanding ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameSeating , seatingGesture , OnGlobalHotKeySeating ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameCustom1 , custom1Gesture , OnGlobalHotKeyCustom1 ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameCustom2 , custom2Gesture , OnGlobalHotKeyCustom2 ) ;

        _logger.Information ( "Global hotkeys registered successfully" ) ;
    }

    private void SafeAddOrReplaceHotkey ( string name , KeyGesture gesture , EventHandler < HotkeyEventArgs > handler )
    {
        try
        {
            // Try to remove first to avoid the "not registered" error when using AddOrReplace
            try
            {
                HotkeyManager.Current.Remove ( name ) ;
                _logger.Debug ( "Removed existing hotkey: {HotkeyName}" , name ) ;
            }
            catch ( Exception ex )
            {
                // Hotkey doesn't exist, that's fine
                _logger.Debug ( ex , "Hotkey {HotkeyName} was not previously registered" , name ) ;
            }

            // Now add the hotkey
            HotkeyManager.Current.AddOrReplace ( name , gesture , handler ) ;
            _logger.Debug ( "Registered hotkey: {HotkeyName} with gesture: {Gesture}" , name , gesture ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to register hotkey: {HotkeyName}" ,
                            name ) ;
        }
    }

    public async Task StandAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
            return ;

        await MoveToConfiguredHeightAsync ( nameof ( StandAsync ) ,
                                            s => s.HeightSettings.StandingHeightInCm ) ;
    }

    public async Task SitAsync ( )
    {
        if ( ! IsDeskConnected ( ) )
            return ;

        await MoveToConfiguredHeightAsync ( nameof ( SitAsync ) ,
                                            s => s.HeightSettings.SeatingHeightInCm ) ;
    }

    public async Task Custom1Async ( )
    {
        if ( ! IsDeskConnected ( ) )
            return ;

        await MoveToConfiguredHeightAsync ( nameof ( Custom1Async ) ,
                                            s => s.HeightSettings.Custom1HeightInCm ) ;
    }

    public async Task Custom2Async ( )
    {
        if ( ! IsDeskConnected ( ) )
            return ;

        await MoveToConfiguredHeightAsync ( nameof ( Custom2Async ) ,
                                            s => s.HeightSettings.Custom2HeightInCm ) ;
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

            await Connect ( ).ConfigureAwait ( false ) ;
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
            ConnectFailed ( ) ;
        }
    }

    // Helper to run an action only when connected
    private Task ExecuteIfConnected ( Action action )
    {
        if ( ! IsDeskConnected ( ) )
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

    private CancellationToken GetTokenOrThrow ( )
    {
        return _token ;
    }

    private async Task MoveToConfiguredHeightAsync ( string                    methodName ,
                                                     Func < ISettings , uint > pickHeightInCm )
    {
        _logger.Debug ( "Executing {MethodName}( pickHeightInCm = {PickHeightInCm} )..." ,
                        methodName ,
                        pickHeightInCm ) ;

        await _manager.LoadAsync ( CancellationToken.None ).ConfigureAwait ( false ) ;

        var heightInCm = pickHeightInCm ( _manager.CurrentSettings ) ;
        var height     = HeightToDeskHeight ( heightInCm ) ;

        _logger.Information ( "Executing {MethodName} which moves the desk to height {HeightInCm} cm..." ,
                              methodName ,
                              heightInCm ) ;

        _desk?.MoveTo ( height ) ;
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

    private static uint HeightToDeskHeight ( uint heightInCm )
    {
        return heightInCm * DeskHeightFactor ;
    }

    private static uint MmToCm ( uint height )
    {
        // Use double division and specify midpoint rounding to avoid ambiguous overloads and banker's rounding
        return ( uint )Math.Round ( height / ( double )DeskHeightFactor ,
                                    MidpointRounding.AwayFromZero ) ;
    }

    private void DoDisconnectAsync ( )
    {
        try
        {
            _logger.Debug ( "[{DeviceName}] Trying to disconnect from Idasen Desk..." ,
                            _desk?.DeviceName ) ;

            DisposeDesk ( ) ;

            _logger.Debug ( "[{DeviceName}] ...disconnected from Idasen Desk" ,
                            _desk?.DeviceName ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to disconnect" ) ;
            ConnectFailed ( ) ;
        }
    }

    private void HotkeyManager_HotkeyAlreadyRegistered ( object ? sender , HotkeyAlreadyRegisteredEventArgs e )
    {
        _logger.Warning ( "The hotkey {Name} is already registered by another application" ,
                          e.Name ) ;

        _notifications.Show ( "Hotkey already registered" ,
                              $"The hotkey '{e.Name}' is already registered by another application." ,
                              InfoBarSeverity.Warning ) ;
    }

    /// <summary>
    ///     Parses a key string to a Key enum value.
    /// </summary>
    private static Key ParseKey ( string keyString )
    {
        if ( Enum.TryParse < Key > ( keyString , true , out var key ) )
            return key ;

        throw new ArgumentException ( $"Invalid key string: '{keyString}'" ,
                                      nameof ( keyString ) ) ;
    }

    /// <summary>
    ///     Parses a modifier string (e.g., "Control, Alt, Shift") to ModifierKeys enum value.
    /// </summary>
    private static ModifierKeys ParseModifierKeys ( string modifierString )
    {
        var modifiers = ModifierKeys.None ;

        if ( string.IsNullOrWhiteSpace ( modifierString ) )
            return modifiers ;

        var parts = modifierString.Split ( ModifierSeparators ,
                                           StringSplitOptions.RemoveEmptyEntries ) ;

        foreach ( var part in parts )
            if ( Enum.TryParse < ModifierKeys > ( part , true , out var modifier ) )
                modifiers |= modifier ;

        return modifiers ;
    }

    /// <summary>
    ///     Creates a KeyGesture from key and modifier strings, with fallback to default gesture.
    /// </summary>
    private KeyGesture CreateKeyGesture ( string keyString ,
                                         string modifierString ,
                                         KeyGesture defaultGesture )
    {
        try
        {
            var key       = ParseKey ( keyString ) ;
            var modifiers = ParseModifierKeys ( modifierString ) ;

            return new KeyGesture ( key ,
                                   modifiers ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to parse hotkey configuration (Key: '{Key}', Modifiers: '{Modifiers}'). Using default." ,
                             keyString ,
                             modifierString ) ;

            return defaultGesture ;
        }
    }


    private void OnErrorChanged ( IErrorDetails details )
    {
        var deviceName = _desk?.DeviceName ?? "Unknown" ;
        var msg        = $"[{deviceName}] {details.Message}" ;

        _logger.Error ( "Desk error: {Message}" ,
                        msg ) ;

        OnStatusChanged ( 0 ,
                          "Error" ,
                          msg ,
                          InfoBarSeverity.Error ) ;
    }

    // Unified hotkey handler to reduce duplication
    private void HandleHotkey ( string name , Func < Task > action )
    {
        try
        {
            _logger.Information ( "Received global hot key for '{Name}' command..." ,
                                  name ) ;
            _ = ExecuteWithErrorHandlingAsync ( name ,
                                                action ) ;
        }
        catch ( Exception exception )
        {
            _logger.Error ( exception ,
                            "Failed to handle global hot key command for '{Name}'." ,
                            name ) ;
        }
    }

    private void OnGlobalHotKeyStanding ( object ? sender , HotkeyEventArgs e )
    {
        HandleHotkey ( nameof ( StandAsync ) ,
                       StandAsync ) ;
    }

    private void OnGlobalHotKeySeating ( object ? sender , HotkeyEventArgs e )
    {
        HandleHotkey ( nameof ( SitAsync ) ,
                       SitAsync ) ;
    }

    private void OnGlobalHotKeyCustom1 ( object ? sender , HotkeyEventArgs e )
    {
        HandleHotkey ( nameof ( Custom1Async ) ,
                       Custom1Async ) ;
    }

    private void OnGlobalHotKeyCustom2 ( object ? sender , HotkeyEventArgs e )
    {
        HandleHotkey ( nameof ( Custom2Async ) ,
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
                    RegisterGlobalHotkeys ( ) ;
                }
                else
                {
                    _logger.Information ( "Unregistering hotkeys due to settings change" ) ;
                    UnregisterGlobalHotkeys ( ) ;
                }
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to handle hotkey settings change" ) ;
            }
        } ) ;
    }

    private Exception? TryUnregisterGlobalHotkey ( string hotkeyName )
    {
        try
        {
            _logger.Debug ( "Attempting to remove hotkey: {HotkeyName}" , hotkeyName ) ;
            HotkeyManager.Current.Remove ( hotkeyName ) ;
            _logger.Information ( "Successfully removed hotkey: {HotkeyName}" , hotkeyName ) ;
            return null ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to remove hotkey: {HotkeyName}" ,
                            hotkeyName ) ;
            return e ;
        }
    }

    private void UnregisterGlobalHotkeys ( )
    {
        _logger.Information ( "Unregistering global hotkeys..." ) ;

        var failedHotkeys = new System.Collections.Generic.List<string> ( ) ;
        Exception? firstException = null ;

        var standingException = TryUnregisterGlobalHotkey ( HotkeyNameStanding ) ;
        if ( standingException is not null )
        {
            failedHotkeys.Add ( HotkeyNameStanding ) ;
            firstException ??= standingException ;
        }

        var seatingException = TryUnregisterGlobalHotkey ( HotkeyNameSeating ) ;
        if ( seatingException is not null )
        {
            failedHotkeys.Add ( HotkeyNameSeating ) ;
            firstException ??= seatingException ;
        }

        var custom1Exception = TryUnregisterGlobalHotkey ( HotkeyNameCustom1 ) ;
        if ( custom1Exception is not null )
        {
            failedHotkeys.Add ( HotkeyNameCustom1 ) ;
            firstException ??= custom1Exception ;
        }

        var custom2Exception = TryUnregisterGlobalHotkey ( HotkeyNameCustom2 ) ;
        if ( custom2Exception is not null )
        {
            failedHotkeys.Add ( HotkeyNameCustom2 ) ;
            firstException ??= custom2Exception ;
        }

        if ( failedHotkeys.Count == 0 )
        {
            _logger.Information ( "Successfully unregistered all global hotkeys" ) ;
            return ;
        }

        throw new InvalidOperationException (
            $"Failed to unregister global hotkeys: {string.Join ( ", " , failedHotkeys )}. Hotkeys may still be active." ,
            firstException ) ;
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
        _statusBarInfoSubject.OnNext ( info ) ;

        _notifications.Show ( title ,
                              message ,
                              severity ) ;
    }

    private bool IsDeskConnected ( )
    {
        if ( _desk is not null )
            return true ;

        const string message = "Failed to connect to desk!" ;

        _logger.Error ( message ) ;

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
            _logger.Debug ( "Trying to initialize provider..." ) ;

            _deskProvider?.Dispose ( ) ;
            _deskProvider = _providerFactory! ( ) ;
            _deskProvider.Initialize ( _manager.CurrentSettings.DeviceSettings.DeviceName ,
                                       _manager.CurrentSettings.DeviceSettings.DeviceAddress ,
                                       _manager.CurrentSettings.DeviceSettings.DeviceMonitoringTimeout ) ;

            var deviceName = _manager.CurrentSettings.DeviceSettings.DeviceName ;
            _logger.Debug ( "[{DeviceName}] Trying to connect to Idasen Desk..." ,
                            deviceName ) ;

            var token = GetTokenOrThrow ( ) ;

            var (isSuccess , desk) = await _deskProvider.TryGetDesk ( token ).ConfigureAwait ( false ) ;

            if ( isSuccess )
                ConnectSuccessful ( desk! ) ;
            else
                ConnectFailed ( ) ;
        }
        catch ( Exception e )
        {
            var deviceName = _desk?.DeviceName ?? "Unknown" ;
            _logger.Error ( e ,
                            "[{DeviceName}] Failed to connect" ,
                            deviceName ) ;
            ConnectFailed ( ) ;
        }
    }

    private void CheckIfInitialized ( )
    {
        if ( ! IsInitialize )
            throw new InvalidOperationException ( "Initialize needs to be called first!" ) ;
    }

    private void ConnectFailed ( )
    {
        _logger.Debug ( "Connection failed..." ) ;
        _ = DisconnectAsync ( ) ;
        _errorManager.PublishForMessage ( "Failed to connect" ) ;
    }

    private void DisposeDesk ( )
    {
        _logger.Debug ( "[{DeskName}] Disposing desk" ,
                        _desk?.Name ) ;

        _finished?.Dispose ( ) ;
        _desk?.Dispose ( ) ;
        _deskProvider?.Dispose ( ) ;

        _finished     = null ;
        _desk         = null ;
        _deskProvider = null ;
    }

    private void ConnectSuccessful ( IDesk desk )
    {
        _logger.Information ( "[{DeviceName}] Connected with address {BluetoothAddress} (MacAddress {MacAddress})" ,
                              desk.DeviceName ,
                              desk.BluetoothAddress.MaskAddress ( ) ,
                              desk.BluetoothAddress.ToMacAddress ( ).MaskMacAddress ( ) ) ;

        _desk = desk ;

        _finished = _desk.FinishedChanged
                         .ObserveOn ( Scheduler.Default )
                         .SubscribeAsync ( OnFinishedChanged ) ;

        _heightChanged = _desk.HeightChanged
                              .ObserveOn ( Scheduler.Default )
                              .Throttle ( TimeSpan.FromSeconds ( 1 ) )
                              .SubscribeAsync ( OnHeightChanged ) ;

        _iconProvider.Initialize ( _logger ,
                                   _desk ,
                                   _notifyIcon ) ;

        var message = $"Connected successfully to '{_desk.DeviceName}'." ;

        OnStatusChanged ( 0 ,
                          "Connected" ,
                          message ,
                          InfoBarSeverity.Success ) ;

        if ( ! _manager.CurrentSettings.DeviceSettings.DeviceLocked )
            return ;

        _logger.Information ( "Locking desk movement" ) ;

        _desk?.MoveLockAsync ( ) ;
    }
}