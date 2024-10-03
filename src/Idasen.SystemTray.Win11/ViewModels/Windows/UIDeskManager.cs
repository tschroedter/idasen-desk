﻿using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Windows.Input ;
using System.Windows.Threading ;
using Autofac ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Settings ;
using JetBrains.Annotations ;
using Microsoft.Toolkit.Uwp.Notifications ;
using NHotkey ;
using NHotkey.Wpf ;
using Constants = Idasen.BluetoothLE.Characteristics.Common.Constants ;
using MessageBox = System.Windows.MessageBox ;
using ILogger = Serilog.ILogger ;
using Hardcodet.Wpf.TaskbarNotification;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public class UiDeskManager : IUiDeskManager
{
    private static readonly KeyGesture IncrementGesture = new(Key.Up , ModifierKeys.Control   | ModifierKeys.Alt | ModifierKeys.Shift) ;
    private static readonly KeyGesture DecrementGesture = new(Key.Down , ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) ;

    private readonly ITaskbarIconProvider _iconProvider ;
    private readonly ISettingsManager ?   _manager ;

    private IDesk ?         _desk ;
    private IDeskProvider ? _deskProvider ;
    private IErrorManager ? _errorManager ;
    private IDisposable ?   _finished ;
    private ILogger ?       _logger ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    private Func < IDeskProvider > ?  _providerFactory ;
    private CancellationToken ?       _token ;
    private CancellationTokenSource ? _tokenSource ;
    private TaskbarIcon ?             _taskbarIcon ;

    public UiDeskManager ( ISettingsManager     manager ,
                           ITaskbarIconProvider iconProvider )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( iconProvider ,
                                nameof ( iconProvider ) ) ;

        _manager      = manager ;
        _iconProvider = iconProvider ;
    }

    public bool IsInitialize => _logger != null && _manager != null ; // todo  && _provider != null ;

    public void Dispose ( )
    {
        _logger?.Information ( "Disposing..." ) ;

        _tokenSource?.Cancel ( ) ;

        DisposeDesk ( ) ;

        _deskProvider?.Dispose ( ) ;
        _tokenSource?.Dispose ( ) ;
    }

    public UiDeskManager Initialize ( IContainer container , TaskbarIcon taskbarIcon )
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;
        Guard.ArgumentNotNull ( taskbarIcon ,
                                nameof ( taskbarIcon ) ) ;

        _taskbarIcon     = taskbarIcon ;
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

        // ReSharper disable once AsyncVoidLambda
        Task.Run ( new Action ( async ( ) => { await AutoConnect ( ) ; } ) ) ;

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


    private void OnGlobalHotKeyStanding ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger?.Information ( "Received global hot key for 'Standing' command..." ) ;

            if ( _desk == null )
                return ;

            Task.Run ( async ( ) => await DoStanding ( ) ) ;
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

            if ( _desk == null )
                return ;

            Task.Run ( async ( ) => await DoSeating ( ) ) ;
        }
        catch ( Exception exception )
        {
            _logger?.Error ( exception , "Failed to handle global hot key command for 'Seating'." ) ;
        }
    }

    private async Task DoStanding ( )
    {
        try
        {
            _logger?.Debug ( $"{nameof ( DoStanding )}" ) ;

            await Standing ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( DoStanding )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( DoStanding )}" ) ;
        }
    }

    public async Task Standing ( )
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
            _logger?.Debug ( $"{nameof ( DoSeating )}" ) ;

            await Seating ( ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             $"Failed to call {nameof ( DoSeating )}" ) ;

            _errorManager?.PublishForMessage ( $"Failed to call {nameof ( DoSeating )}" ) ;
        }
    }

    public async Task Seating ( )
    {
        if ( _manager != null )
        {
            await _manager.Load ( )
                          .ConfigureAwait ( false ) ;

            _desk?.MoveTo ( _manager.CurrentSettings.HeightSettings.SeatingHeightInCm *
                            100 ) ; // todo duplicate
        }
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

            // DisposeDesk ( ) ;

            _tokenSource?.Cancel ( false ) ;

            _tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            _token       = _tokenSource.Token ;

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
            _logger?.Error ( e ,
                             $"[{_desk?.DeviceName}] Failed to connect" ) ;

            ConnectFailed ( ) ;
        }
    }

    public async Task AutoConnect ( )
    {
        _logger?.Debug ( "Auto connecting..." ) ;

        try
        {
            CheckIfInitialized ( ) ;

            _logger?.Debug ( "Trying to load settings..." ) ;

            if ( _manager == null )
                throw new Exception ( "Manager is null" ) ;

            await _manager.Load ( ) ;

            _logger?.Debug ( "Trying to auto connect to Idasen Desk..." ) ;

            if ( _token == null )
                throw new Exception ( "Token is null" ) ;

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

    // todo this ui stuff should ne in MainWindowViewModel
    private void ShowFancyBalloon ( string     title ,
                                    string     text ,
                                    Visibility visibilityBulbGreen  = Visibility.Hidden ,
                                    Visibility visibilityBulbYellow = Visibility.Hidden ,
                                    Visibility visibilityBulbRed    = Visibility.Hidden )
    {
        if ( _manager is { CurrentSettings.NotificationsEnabled: false } )
        {
            _logger?.Information ( $"Notifications are disabled. " +
                                   $"Title = '{title}' "           +
                                   $"Text = '{text}'" ) ;

            return ;
        }

        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            _logger?.Debug ( "Dispatching call on UI thread" ) ;

            Dispatcher.CurrentDispatcher.BeginInvoke ( new Action ( ( ) => ShowFancyBalloon ( title ,
                                                                                              text ,
                                                                                              visibilityBulbGreen ,
                                                                                              visibilityBulbYellow ,
                                                                                              visibilityBulbRed ) ) ) ;

            return ;
        }

        _logger?.Debug ( $"Title = '{title}', "                              +
                         $"Text = '{text}', "                                +
                         $"visibilityBulbGreen = '{visibilityBulbGreen}', "  +
                         $"visibilityBulbYellow = '{visibilityBulbYellow}' " +
                         $"visibilityBulbRed = '{visibilityBulbRed}'" ) ;

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var builder = new ToastContentBuilder ( ) ;
        builder.AddText ( title ) ;
        builder.AddText ( text ) ; // todo image balloon

        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        // Try running this code and you should see the notification appear!
        builder.Show ( ) ;
    }

    public void Disconnect ( )
    {
        try
        {
            _logger?.Debug ( $"[{_desk?.DeviceName}] Trying to disconnect from Idasen Desk..." ) ;

            DisposeDesk ( ) ;

            _tokenSource?.Cancel ( false ) ;

            _logger?.Debug ( $"[{_desk?.DeviceName}] ...disconnected from Idasen Desk" ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             "Failed to disconnect" ) ;

            ConnectFailed ( ) ;
        }
    }


    private void ConnectFailed ( )
    {
        _logger?.Debug ( "Connection failed..." ) ;

        Disconnect ( ) ;

        ShowFancyBalloon ( "Failed to Connect" ,
                           Constants.CheckAndEnableBluetooth ,
                           visibilityBulbRed : Visibility.Visible ) ;
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

        ShowFancyBalloon ( "Success" ,
                           "Connected to desk: " +
                           Environment.NewLine   +
                           $"'{desk.Name}'" ,
                           Visibility.Visible ) ;

        _iconProvider.Initialize ( _logger! ,
                                   _desk ,
                                   _taskbarIcon ) ;

        _logger?.Debug ( $"[{_desk?.DeviceName}] Connected successful" ) ;

        if ( ! _manager?.CurrentSettings.DeviceSettings.DeviceLocked ?? true )
            return ;

        _logger?.Information ( "Locking desk movement" ) ;

        _desk?.MoveLock ( ) ;
    }

    private void OnFinishedChanged ( uint height )
    {
        _logger?.Debug ( $"Height = {height}" ) ;

        var heightInCm = Math.Round ( height / 100.0 ) ;

        ShowFancyBalloon ( "Finished" ,
                           $"Desk height is {heightInCm:F0} cm" ,
                           Visibility.Visible ) ;
    }
}