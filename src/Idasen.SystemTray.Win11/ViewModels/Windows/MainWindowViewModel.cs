using System.Collections.ObjectModel ;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Windows.Input ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Views.Pages ;
using NHotkey.Wpf ;
using Wpf.Ui.Controls ;
using MessageBox = System.Windows.MessageBox ;
using NHotkey;
using Idasen.SystemTray.Win11.Settings ;
using Idasen.SystemTray.Win11.Utils ;
using ILogger = Serilog.ILogger ;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public partial class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel ( ISettingsManager manager ,
                                 IVersionProvider versionProvider )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( versionProvider ,
                                nameof ( versionProvider ) ) ;

        _manager         = manager ;
        _versionProvider = versionProvider ;
    }

    [ ObservableProperty ]
    private string _applicationTitle = "Idasen Desk" ;

    [ ObservableProperty ]
    private ObservableCollection < object > _footerMenuItems =
    [
        new NavigationViewItem
        {
            Content        = "Settings" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Settings24 } ,
            TargetPageType = typeof ( SettingsPage )
        }
    ] ;

    [ ObservableProperty ]
    private ObservableCollection < object > _menuItems =
    [
        new NavigationViewItem
        {
            Content = "Home" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.Home24
            } ,
            TargetPageType = typeof ( DashboardPage )
        }
    ] ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems =
        [new MenuItem { Header = "Home" , Tag = "tray_home" }] ;

    public MainWindowViewModel Initialize ( ILogger                logger ,
                                            Func < IDeskProvider > providerFactory ,
                                            IErrorManager          errorManager )
    {
        Guard.ArgumentNotNull ( logger ,
                                nameof ( logger ) ) ;
        Guard.ArgumentNotNull ( providerFactory ,
                                nameof ( providerFactory ) ) ;
        Guard.ArgumentNotNull ( errorManager ,
                                nameof ( errorManager ) ) ;

        _logger          = logger ;
        _providerFactory = providerFactory ;
        _errorManager    = errorManager ;

        _logger?.Debug ( "Initializing..." ) ;

        _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
        _token       = _tokenSource.Token ;

        _onErrorChanged = errorManager.ErrorChanged
                                      .ObserveOn ( _scheduler )
                                      .Subscribe ( OnErrorChanged ) ;


        HotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered ;

        HotkeyManager.Current.AddOrReplace ( "Increment" , IncrementGesture , OnGlobalHotKeyStanding ) ;
        HotkeyManager.Current.AddOrReplace ( "Decrement" , DecrementGesture , OnGlobalHotKeySeating ) ;

        // ReSharper disable once AsyncVoidLambda
        Task.Run(new Action(async () =>
                            {
                                await AutoConnect();
                            }));

        return this ;
    }

    private void HotkeyManager_HotkeyAlreadyRegistered ( object ? sender , HotkeyAlreadyRegisteredEventArgs e )
    {
        MessageBox.Show ( $"The hotkey {e.Name} is already registered by another application" ) ;
    }

    private void OnErrorChanged ( IErrorDetails details )
    {
        _logger?.Error ( $"[{_desk?.DeviceName}] {details.Message}" ) ;

        ShowFancyBalloon ( "Error" ,
                           details.Message ,
                           visibilityBulbRed : Visibility.Visible ) ;
    }

    private static readonly KeyGesture IncrementGesture = new KeyGesture ( Key.Up , ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift ) ;

    private static readonly KeyGesture
        DecrementGesture = new KeyGesture ( Key.Down , ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift ) ;

    private          IErrorManager ? _errorManager ;
    private readonly IScheduler      _scheduler = Scheduler.CurrentThread ;

    private IDesk ?                   _desk ;
    private IDisposable ?             _finished ;
    private ILogger ?                 _logger ;
    private ISettingsManager ?        _manager ;
    private TaskbarIcon ?             _notifyIcon ; // todo not sure if this is still needed
    private IDisposable ?             _onErrorChanged ;
    private IVersionProvider ?        _versionProvider ;
    private ITaskbarIconProvider ?    _iconProvider;  // todo not sure if this is still needed
    private IDeskProvider ?           _deskProvider ;
    private Func < IDeskProvider > ?  _providerFactory ;
    private CancellationToken ?       _token ;
    private CancellationTokenSource ? _tokenSource ;


    private void OnGlobalHotKeyStanding ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger?.Information ( "Received global hot key for 'Standing' command..." ) ;

            if ( ! StandingCommand.CanExecute ( this ) )
                return ;

            var task = Standing ( ).ConfigureAwait ( false ) ;

            StandingCommand.Execute ( this ) ;
        }
        catch ( Exception exception )
        {
            _logger?.Error ( exception , "Failed to handle global hot key command for 'Standing'." ) ;
        }
    }

    private void OnGlobalHotKeySeating ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger?.Information ( "Received global hot key for 'Seating' command..." ) ;

            if ( ! SeatingCommand.CanExecute ( this ) )
                return ;

            SeatingCommand.Execute ( this ) ;
        }
        catch ( Exception exception )
        {
            _logger?.Error ( exception , "Failed to handle global hot key command for 'Seating'." ) ;
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
                // ReSharper disable once AsyncVoidLambda
                CommandAction  = async ( ) => await DoStanding ( ) ,
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
                // ReSharper disable once AsyncVoidLambda
                CommandAction  = async ( ) => await DoSeating ( ) ,
                CanExecuteFunc = ( ) => _desk != null
            } ;
        }
    }

    private async Task DoStanding ( )
    {
        try
        {
            _logger?.Debug ( $"{nameof ( StandingCommand )}" ) ;

            await Standing ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( StandingCommand )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( StandingCommand )}" ) ;
        }
    }

    private async Task Standing ( )
    {
        _logger?.Debug ( "Executing Standing..." ) ;

        if ( _manager != null )
        {
            await _manager.Load ( ) ;

            _desk?.MoveTo ( _manager.CurrentSettings.HeightSettings.StandingHeightInCm * 100 ) ;
        }
    }

    private async Task DoSeating ( )
    {
        try
        {
            _logger?.Debug ( $"{nameof ( SeatingCommand )}" ) ;

            if ( _manager != null )
            {
                await _manager.Load ( )
                              .ConfigureAwait ( false ) ;

                _desk?.MoveTo ( _manager.CurrentSettings.HeightSettings.SeatingHeightInCm *
                                100 ) ; // todo duplicate
            }
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( SeatingCommand )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( SeatingCommand )}" ) ;
        }
    }

    private async Task Connect()
    {
        try
        {
            _logger?.Debug("Trying to initialize provider...");

            if (_manager == null)
                throw new Exception("SettingsManager is null");

            _deskProvider?.Dispose();
            _deskProvider = _providerFactory!();
            _deskProvider.Initialize(_manager!.CurrentSettings.DeviceSettings.DeviceName,
                                     _manager!.CurrentSettings.DeviceSettings.DeviceAddress,
                                     _manager!.CurrentSettings.DeviceSettings.DeviceMonitoringTimeout);

            _logger?.Debug($"[{_desk?.DeviceName}] Trying to connect to Idasen Desk...");

            // DisposeDesk ( ) ;

            _tokenSource?.Cancel(false);

            _tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            _token       = _tokenSource.Token;

            if (_token == null)
                throw new Exception("Token is null");

            var (isSuccess, desk) = await _deskProvider.TryGetDesk(_token.Value);

            if (isSuccess)
                ConnectSuccessful(desk!);
            else
                ConnectFailed();
        }
        catch (Exception e)
        {
            _logger?.Error(e,
                          $"[{_desk?.DeviceName}] Failed to connect");

            ConnectFailed();
        }
    }

    public async Task AutoConnect ( )
    {
        _logger?.Debug ( "Auto connecting..." ) ;

        try
        {
            CheckIfInitialized ( ) ;

            _logger?.Debug ( "Trying to load settings..." ) ;

            if (_manager == null)
                throw new Exception("Manager is null");

            await _manager.Load ( ) ;

            _logger?.Debug ( "Trying to auto connect to Idasen Desk..." ) ;

            if (_token == null)
                throw new Exception("Token is null");

            await Task.Delay ( TimeSpan.FromSeconds ( 3 ) ,
                               _token.Value ) ;

            ShowFancyBalloon ( "Auto Connect" ,
                               "Trying to auto connect to Idasen Desk..." ,
                               visibilityBulbYellow : Visibility.Visible ) ;

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

    private void CheckIfInitialized ( )
    {
        if ( ! IsInitialize )
            throw new Exception ( "Initialize needs to be called first!" ) ;
    }

    private void ShowFancyBalloon ( string     title ,
                                    string     text ,
                                    Visibility visibilityBulbGreen  = Visibility.Hidden ,
                                    Visibility visibilityBulbYellow = Visibility.Hidden ,
                                    Visibility visibilityBulbRed    = Visibility.Hidden )
    {
        if (_manager is { CurrentSettings.NotificationsEnabled: false })
        {
            _logger?.Information($"Notifications are disabled. " +
                                $"Title = '{title}' "           +
                                $"Text = '{text}'");

            return;
        }

        _logger?.Debug($"Title = '{title}', "                              +
                      $"Text = '{text}', "                                +
                      $"visibilityBulbGreen = '{visibilityBulbGreen}', "  +
                      $"visibilityBulbYellow = '{visibilityBulbYellow}' " +
                      $"visibilityBulbRed = '{visibilityBulbRed}'");

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var builder = new ToastContentBuilder();
        builder.AddText(title);
        builder.AddText(text);  // todo image balloon

        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        // Try running this code and you should see the notification appear!
        builder.Show();
    }

    public bool IsInitialize => _logger != null && _manager != null; // todo  && _provider != null ;

    private void Disconnect()
    {
        try
        {
            _logger?.Debug($"[{_desk?.DeviceName}] Trying to disconnect from Idasen Desk...");

            DisposeDesk();

            _tokenSource?.Cancel(false);

            _logger?.Debug($"[{_desk?.DeviceName}] ...disconnected from Idasen Desk");
        }
        catch (Exception e)
        {
            _logger?.Error(e,
                           "Failed to disconnect");

            ConnectFailed();
        }
    }


    private void ConnectFailed()
    {
        _logger?.Debug("Connection failed...");

        Disconnect();

        ShowFancyBalloon("Failed to Connect",
                         Idasen.BluetoothLE.Characteristics.Common.Constants.CheckAndEnableBluetooth,
                         visibilityBulbRed: Visibility.Visible);
    }

    private void DisposeDesk()
    {
        _logger?.Debug($"[{_desk?.Name}] Disposing desk");

        _finished?.Dispose();
        _desk?.Dispose();
        _deskProvider?.Dispose();

        _finished     = null;
        _desk         = null;
        _deskProvider = null;
    }

    private void ConnectSuccessful(IDesk desk)
    {
        _logger?.Information($"[{desk.DeviceName}] Connected to {desk.DeviceName} " +
                            $"with address {desk.BluetoothAddress} "               +
                            $"(MacAddress {desk.BluetoothAddress.ToMacAddress()})");

        _desk = desk;

        _finished = _desk.FinishedChanged
                         .ObserveOn(_scheduler)
                         .Subscribe(OnFinishedChanged);

        ShowFancyBalloon("Success",
                         "Connected to desk: " +
                         Environment.NewLine   +
                         $"'{desk.Name}'",
                         Visibility.Visible);

        _iconProvider?.Initialize(_desk);

        _logger?.Debug($"[{_desk?.DeviceName}] Connected successful");

        if (!_manager?.CurrentSettings.DeviceSettings.DeviceLocked ?? true)
            return;

        _logger?.Information("Locking desk movement");

        _desk?.MoveLock();
    }

    private void OnFinishedChanged(uint height)
    {
        _logger?.Debug($"Height = {height}");

        var heightInCm = Math.Round(height / 100.0);

        ShowFancyBalloon("Finished",
                         $"Desk height is {heightInCm:F0} cm",
                         Visibility.Visible);
    }
}