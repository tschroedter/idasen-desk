using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak;
using Idasen.BluetoothLE.Linak.Interfaces;
using Idasen.SystemTray.Win11.Interfaces;
using JetBrains.Annotations;
using NHotkey;
using NHotkey.Wpf;
using Serilog;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;
using MessageBox = System.Windows.MessageBox;

namespace Idasen.SystemTray.Win11.Utils;

[ExcludeFromCodeCoverage]
public sealed class UiDeskManager : IUiDeskManager
{
    private const uint DeskHeightFactor = 100;

    private const string HotkeyNameStanding = "Standing";
    private const string HotkeyNameSeating = "Seating";
    private const string HotkeyNameCustom1 = "Custom 1";
    private const string HotkeyNameCustom2 = "Custom 2";

    private static readonly KeyGesture StandingGesture = new(Key.Up,
                                                             ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift);

    private static readonly KeyGesture SittingGesture = new(Key.Down,
                                                            ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift);

    private static readonly KeyGesture Custom1Gesture = new(Key.Left,
                                                            ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift);

    private static readonly KeyGesture Custom2Gesture = new(Key.Right,
                                                            ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift);

    private readonly IErrorManager _errorManager;
    private readonly ITaskbarIconProvider _iconProvider;
    private readonly ILogger _logger;
    private readonly ISettingsManager _manager;
    private readonly INotifications _notifications;
    private readonly Func<IDeskProvider>? _providerFactory;
    private readonly IScheduler _scheduler;
    private readonly Subject<StatusBarInfo> _statusBarInfoSubject;
    private readonly object _statusLock = new();

    private IDesk? _desk;
    private IDeskProvider? _deskProvider;
    private IDisposable? _finished;
    private IDisposable? _heightChanged;
    private NotifyIcon? _notifyIcon;

    [UsedImplicitly]
    private IDisposable? _onErrorChanged;

    private CancellationToken? _token;
    private CancellationTokenSource? _tokenSource;
    private bool _disposed;

    public UiDeskManager(
        ILogger logger,
        ISettingsManager manager,
        ITaskbarIconProvider iconProvider,
        INotifications notifications,
        IScheduler scheduler,
        Func<IDeskProvider> deskProviderFactory,
        IErrorManager errorManager)
    {
        Guard.ArgumentNotNull(logger, nameof(logger));
        Guard.ArgumentNotNull(manager, nameof(manager));
        Guard.ArgumentNotNull(iconProvider, nameof(iconProvider));
        Guard.ArgumentNotNull(notifications, nameof(notifications));
        Guard.ArgumentNotNull(scheduler, nameof(scheduler));
        Guard.ArgumentNotNull(deskProviderFactory, nameof(deskProviderFactory));
        Guard.ArgumentNotNull(errorManager, nameof(errorManager));

        _logger = logger;
        _manager = manager;
        _iconProvider = iconProvider;
        _notifications = notifications;
        _scheduler = scheduler;
        _providerFactory = deskProviderFactory;
        _errorManager = errorManager;
        _statusBarInfoSubject = new Subject<StatusBarInfo>();

        LastStatusBarInfo = new StatusBarInfo(
                                               "",
                                               _manager.CurrentSettings.HeightSettings.LastKnownDeskHeight,
                                               "Unknown",
                                               InfoBarSeverity.Informational);
    }

    public IObservable<StatusBarInfo> StatusBarInfoChanged => _statusBarInfoSubject;

    public bool IsInitialize => _notifyIcon is not null;

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( UiDeskManager ) ) ;

        // Unregister static/global hotkeys to avoid retaining this instance
        try
        {
            HotkeyManager.HotkeyAlreadyRegistered -= HotkeyManager_HotkeyAlreadyRegistered ;
            HotkeyManager.Current.Remove ( HotkeyNameStanding ) ;
            HotkeyManager.Current.Remove ( HotkeyNameSeating ) ;
            HotkeyManager.Current.Remove ( HotkeyNameCustom1 ) ;
            HotkeyManager.Current.Remove ( HotkeyNameCustom2 ) ;
        }
        catch
        {
            // ignore cleanup errors
        }

        // Cancel any pending operations
        try
        {
            _tokenSource?.Cancel ( ) ;
        }
        catch
        {
            // ignored }

            DisposeDesk ( ) ;

            _heightChanged?.Dispose ( ) ;
            _finished?.Dispose ( ) ;
            _onErrorChanged?.Dispose ( ) ;
            _deskProvider?.Dispose ( ) ;
            _tokenSource?.Dispose ( ) ;

            // Complete and dispose the subject to release subscriptions
            try
            {
                _statusBarInfoSubject.OnCompleted ( ) ;
                _statusBarInfoSubject.Dispose ( ) ;
            }
            catch
            {
                // ignore
            }
        }
    }

    public bool IsConnected => _desk is not null;

    public Task DisconnectAsync()
    {
        if (IsDeskConnected())
        {
            DoDisconnectAsync();
        }

        return Task.CompletedTask;
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

    public Task StopAsync()
    {
        return ExecuteIfConnected(() => _desk?.MoveStop());
    }

    public Task MoveLockAsync()
    {
        return ExecuteIfConnected(() => _desk?.MoveLock());
    }

    public Task MoveUnlockAsync()
    {
        return ExecuteIfConnected(() => _desk?.MoveUnlock());
    }

    public Task HideAsync()
    {
        if (Application.Current.MainWindow is not null)
            Application.Current.MainWindow.Visibility = Visibility.Hidden;

        return Task.CompletedTask;
    }

    public StatusBarInfo LastStatusBarInfo { get; private set; }

    public Task ExitAsync()
    {
        try
        {
            Application.Current.Shutdown();
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to shutdown application");
        }

        return Task.CompletedTask;
    }

    public UiDeskManager Initialize(NotifyIcon notifyIcon)
    {
        Guard.ArgumentNotNull(notifyIcon, nameof(notifyIcon));

        _notifyIcon = notifyIcon;

        _logger.Debug("UI Desk Manager Initializing...");

        _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        _token = _tokenSource.Token;

        _onErrorChanged = _errorManager.ErrorChanged
                                       .ObserveOn(_scheduler)
                                       .Subscribe(OnErrorChanged);

        HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered;

        HotkeyManager.Current.AddOrReplace(HotkeyNameStanding,
                                           StandingGesture,
                                           OnGlobalHotKeyStanding);
        HotkeyManager.Current.AddOrReplace(HotkeyNameSeating,
                                           SittingGesture,
                                           OnGlobalHotKeySeating);
        HotkeyManager.Current.AddOrReplace(HotkeyNameCustom1,
                                           Custom1Gesture,
                                           OnGlobalHotKeyCustom1);
        HotkeyManager.Current.AddOrReplace(HotkeyNameCustom2,
                                           Custom2Gesture,
                                           OnGlobalHotKeyCustom2);

        // Pass a cancellable token to allow clean shutdown
        _notifications.Initialize(notifyIcon,
                                  _token ?? CancellationToken.None);

        _ = AutoConnectAsync();

        return this;
    }

    public async Task StandAsync()
    {
        if (!IsDeskConnected())
            return;

        await MoveToConfiguredHeightAsync(nameof(StandAsync),
                                          s => s.HeightSettings.StandingHeightInCm);
    }

    public async Task SitAsync()
    {
        if (!IsDeskConnected())
            return;

        await MoveToConfiguredHeightAsync(nameof(SitAsync),
                                          s => s.HeightSettings.SeatingHeightInCm);
    }

    public async Task Custom1Async()
    {
        if (!IsDeskConnected())
            return;

        await MoveToConfiguredHeightAsync(nameof(Custom1Async),
                                          s => s.HeightSettings.Custom1HeightInCm);
    }

    public async Task Custom2Async()
    {
        if (!IsDeskConnected())
            return;

        await MoveToConfiguredHeightAsync(nameof(Custom2Async),
                                          s => s.HeightSettings.Custom2HeightInCm);
    }

    private CancellationToken GetTokenOrThrow()
    {
        return _token ?? throw new Exception("Token is null");
    }

    public async Task AutoConnectAsync()
    {
        _logger.Debug("Auto connecting...");

        try
        {
            CheckIfInitialized();

            _logger.Debug("Trying to load settings...");

            await _manager.LoadAsync(CancellationToken.None).ConfigureAwait(false);

            _logger.Debug("Trying to auto connect to Idasen Desk...");

            var token = GetTokenOrThrow();

            await Task.Delay(TimeSpan.FromSeconds(3), token).ConfigureAwait(false);

            _notifications.Show("Auto Connect",
                                "Trying to auto connect to Idasen Desk...",
                                InfoBarSeverity.Informational);

            await Connect().ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            _logger.Information("Auto connect was canceled");
        }
        catch (Exception e)
        {
            _logger.Error(e,
                          "Failed to auto connect to desk");
            ConnectFailed();
        }
    }

    private async Task MoveToConfiguredHeightAsync(string methodName,
                                                   Func<ISettings, uint> pickHeightInCm)
    {
        _logger.Debug("Executing {MethodName}...", methodName);

        await _manager.LoadAsync(CancellationToken.None).ConfigureAwait(false);

        var heightInCm = pickHeightInCm(_manager.CurrentSettings);
        var height = HeightToDeskHeight(heightInCm);

        _desk?.MoveTo(height);
    }

    private async Task ExecuteWithErrorHandlingAsync(string methodName, Func<Task> action)
    {
        try
        {
            _logger.Debug("{MethodName}", methodName);
            await action().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.Error(e,
                          "Failed to call {MethodName}",
                          methodName);
            _errorManager.PublishForMessage($"Failed to call {methodName}");
        }
    }

    private static uint HeightToDeskHeight(uint heightInCm)
    {
        return heightInCm * DeskHeightFactor;
    }

    private static uint MmToCm(uint height)
    {
        return (uint)Math.Round(height / 100.0);
    }

    private void DoDisconnectAsync()
    {
        try
        {
            _logger.Debug("[{DeviceName}] Trying to disconnect from Idasen Desk...",
                          _desk?.DeviceName);

            DisposeDesk();

            _logger.Debug("[{DeviceName}] ...disconnected from Idasen Desk",
                          _desk?.DeviceName);
        }
        catch (Exception e)
        {
            _logger.Error(e,
                          "Failed to disconnect");
            ConnectFailed();
        }
    }

    private static void HotkeyManager_HotkeyAlreadyRegistered(object? sender, HotkeyAlreadyRegisteredEventArgs e)
    {
        MessageBox.Show($"The hotkey {e.Name} is already registered by another application");
    }

    private void OnErrorChanged(IErrorDetails details)
    {
        var deviceName = _desk?.DeviceName ?? "Unknown";
        var message = $"[{deviceName}] {details.Message}";

        _logger.Error("{ErrorMessage}",
                      message);

        OnStatusChanged(0,
                        "Error",
                        message,
                        InfoBarSeverity.Error);
    }

    // Unified hotkey handler to reduce duplication
    private void HandleHotkey(string name, Func<Task> action)
    {
        try
        {
            _logger.Information("Received global hot key for '{Name}' command...", name);
            _ = ExecuteWithErrorHandlingAsync(name, action);
        }
        catch (Exception exception)
        {
            _logger.Error(exception,
                          "Failed to handle global hot key command for '{Name}'.",
                          name);
        }
    }

    private void OnGlobalHotKeyStanding(object? sender, HotkeyEventArgs e)
    {
        HandleHotkey(nameof(StandAsync), StandAsync);
    }

    private void OnGlobalHotKeySeating(object? sender, HotkeyEventArgs e)
    {
        HandleHotkey(nameof(SitAsync), SitAsync);
    }

    private void OnGlobalHotKeyCustom1(object? sender, HotkeyEventArgs e)
    {
        HandleHotkey(nameof(Custom1Async), Custom1Async);
    }

    private void OnGlobalHotKeyCustom2(object? sender, HotkeyEventArgs e)
    {
        HandleHotkey(nameof(Custom2Async), Custom2Async);
    }

    private static string HeightMessage(uint heightInCm) => $"Desk height is {heightInCm} cm";

    private async Task NotifyAndPersistHeightAsync(uint heightInCm, string title, InfoBarSeverity severity)
    {
        OnStatusChanged(heightInCm,
                        title,
                        HeightMessage(heightInCm),
                        severity);

        await _manager.SetLastKnownDeskHeight(heightInCm,
                                              CancellationToken.None).ConfigureAwait(false);
    }

    private async Task OnFinishedChanged(uint height)
    {
        var heightInCm = MmToCm(height);
        await NotifyAndPersistHeightAsync(heightInCm, "Finished", InfoBarSeverity.Success).ConfigureAwait(false);
    }

    private async Task OnHeightChanged(uint height)
    {
        var heightInCm = MmToCm(height);
        await NotifyAndPersistHeightAsync(heightInCm, "Height Changed", InfoBarSeverity.Warning).ConfigureAwait(false);
    }

    private void OnStatusChanged(
        uint height = 0,
        string title = "",
        string message = "",
        InfoBarSeverity severity = InfoBarSeverity.Informational)
    {
        _logger.Debug("{HeightName} = {height}, {TitleName} = {title}, {Message} = {messageName}, {SeverityName} = {severity}",
                      nameof(height),
                      height,
                      nameof(title),
                      title,
                      nameof(message),
                      message,
                      nameof(severity),
                      severity);

        if (height == 0)
        {
            height = _manager.CurrentSettings.HeightSettings.LastKnownDeskHeight;
        }

        var info = new StatusBarInfo(title,
                                     height,
                                     message,
                                     severity);

        // Update the last known info under lock
        lock (_statusLock)
        {
            LastStatusBarInfo = info;
        }

        // Publish to observers outside the lock to avoid "used inside a lock" warnings and potential deadlocks
        _statusBarInfoSubject.OnNext(info);

        _notifications.Show(title,
                            message,
                            severity);
    }

    private bool IsDeskConnected()
    {
        if (_desk is not null)
            return true;

        const string message = "Failed to connect to desk!";

        _logger.Error(message);

        OnStatusChanged(0,
                        "Not Connected",
                        message,
                        InfoBarSeverity.Error);

        return false;
    }

    private async Task Connect()
    {
        try
        {
            _logger.Debug("Trying to initialize provider...");

            _deskProvider?.Dispose();
            _deskProvider = _providerFactory!();
            _deskProvider.Initialize(_manager.CurrentSettings.DeviceSettings.DeviceName,
                         _manager.CurrentSettings.DeviceSettings.DeviceAddress,
                         _manager.CurrentSettings.DeviceSettings.DeviceMonitoringTimeout);

            var deviceName = _manager.CurrentSettings.DeviceSettings.DeviceName;
            _logger.Debug("[{DeviceName}] Trying to connect to Idasen Desk...",
              deviceName);

            var token = GetTokenOrThrow();

                var (isSuccess, desk) = await _deskProvider.TryGetDesk(token).ConfigureAwait(false);

                if (isSuccess)
                    ConnectSuccessful(desk!);
            else
                ConnectFailed();
        }
        catch (Exception e)
        {
            var deviceName = _desk?.DeviceName ?? "Unknown";
            _logger.Error(e,
                          "[{DeviceName}] Failed to connect",
                          deviceName);
            ConnectFailed();
        }
    }

    private void CheckIfInitialized()
    {
        if (!IsInitialize)
            throw new Exception("Initialize needs to be called first!");
    }

    private void ConnectFailed()
    {
        _logger.Debug("Connection failed...");
        _ = DisconnectAsync();
        _errorManager.PublishForMessage("Failed to connect");
    }

    private void DisposeDesk()
    {
        _logger.Debug("[{DeskName}] Disposing desk",
                      _desk?.Name);

        _finished?.Dispose();
        _desk?.Dispose();
        _deskProvider?.Dispose();

        _finished = null;
        _desk = null;
        _deskProvider = null;
    }

    private void ConnectSuccessful(IDesk desk)
    {
        _logger.Information("[{DeviceName}] Connected with address {BluetoothAddress} (MacAddress {MacAddress})",
                            desk.DeviceName,
                            desk.BluetoothAddress,
                            desk.BluetoothAddress.ToMacAddress());

        _desk = desk;

        _finished = _desk.FinishedChanged
                         .ObserveOn(Scheduler.Default)
                         .SubscribeAsync(OnFinishedChanged);

        _heightChanged = _desk.HeightChanged
                              .ObserveOn(Scheduler.Default)
                              .Throttle(TimeSpan.FromSeconds(1))
                              .SubscribeAsync(OnHeightChanged);

        _iconProvider.Initialize(_logger,
                                 _desk,
                                 _notifyIcon);

        var message = $"Connected successfully to '{_desk?.DeviceName}'.";

        OnStatusChanged(0,
                        "Connected",
                        message,
                        InfoBarSeverity.Success);

        if (!_manager.CurrentSettings.DeviceSettings.DeviceLocked)
            return;

        _logger.Information("Locking desk movement");

        _desk?.MoveLock();
    }
}