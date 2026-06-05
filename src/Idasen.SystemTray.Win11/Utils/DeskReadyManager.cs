using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class DeskReadyManager : IDeskReadyManager
{
    private readonly ILogger                       _logger ;
    private readonly ISettingsManager              _settingsManager ;
    private readonly ITaskbarIconProvider          _iconProvider ;
    private readonly IDeskNotificationManager      _notificationManager ;
    private readonly NotifyIcon ?                  _notifyIcon ;

    private bool            _disposed ;
    private IDisposable ?   _finished ;
    private IDisposable ?   _heightChanged ;

    public DeskReadyManager (
        ILogger                   logger ,
        ISettingsManager          settingsManager ,
        ITaskbarIconProvider      iconProvider ,
        IDeskNotificationManager  notificationManager ,
        NotifyIcon ?              notifyIcon )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;
        ArgumentNullException.ThrowIfNull ( settingsManager ) ;
        ArgumentNullException.ThrowIfNull ( iconProvider ) ;
        ArgumentNullException.ThrowIfNull ( notificationManager ) ;

        _logger               = logger ;
        _settingsManager      = settingsManager ;
        _iconProvider         = iconProvider ;
        _notificationManager  = notificationManager ;
        _notifyIcon           = notifyIcon ;
    }

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( DeskReadyManager ) ) ;

        DisposeResource ( _finished , nameof ( _finished ) ) ;
        DisposeResource ( _heightChanged , nameof ( _heightChanged ) ) ;

        _finished      = null ;
        _heightChanged = null ;
    }

    private void DisposeResource ( IDisposable ? resource , string resourceName )
    {
        if ( resource == null )
            return ;

        try
        {
            resource.Dispose ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to dispose {ResourceName}" ,
                             resourceName ) ;
        }
    }

    public void OnDeskReady ( IDesk desk )
    {
        _logger.Debug ( "Desk ready event received, setting up subscriptions..." ) ;

        // Dispose existing subscriptions before creating new ones to prevent leaks on reconnect
        _finished?.Dispose ( ) ;
        _heightChanged?.Dispose ( ) ;

        _finished = desk.FinishedChanged
                        .ObserveOn ( Scheduler.Default )
                        .Subscribe ( height =>
                                     {
                                         _ = HandleFinishedChangedAsync ( height ) ;
                                     } ) ;

        _heightChanged = desk.HeightChanged
                             .ObserveOn ( Scheduler.Default )
                             .Throttle ( TimeSpan.FromSeconds ( AppConfiguration.Timeouts
                                                                                .HeightChangeThrottleSeconds ) )
                             .Subscribe ( height =>
                                          {
                                              _ = HandleHeightChangedAsync ( height ) ;
                                          } ) ;

        _iconProvider.Initialize ( _logger ,
                                   desk ,
                                   _notifyIcon ) ;

        var message = $"Connected successfully to '{desk.DeviceName}'." ;

        _notificationManager.ShowStatusUpdate ( 0 ,
                                                "Connected" ,
                                                message ,
                                                InfoBarSeverity.Success ) ;
    }

    private async Task HandleFinishedChangedAsync ( uint height )
    {
        try
        {
            await OnFinishedChanged ( height ).ConfigureAwait ( false ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Error in HandleFinishedChangedAsync") ;
        }
    }

    private async Task HandleHeightChangedAsync(uint height)
    {
        try
        {
            await OnHeightChanged(height).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.Error(ex,
                          "Error in HandleHeightChangedAsync");
        }
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

    private async Task NotifyAndPersistHeightAsync ( uint heightInCm , string title , InfoBarSeverity severity )
    {
        _notificationManager.ShowStatusUpdate ( heightInCm ,
                                                title ,
                                                HeightMessage ( heightInCm ) ,
                                                severity ) ;

        await _settingsManager.SetLastKnownDeskHeight ( heightInCm ,
                                                        CancellationToken.None ).ConfigureAwait ( false ) ;
    }

    private static string HeightMessage ( uint heightInCm )
    {
        return $"Current Height: {heightInCm} cm" ;
    }

    private static uint MmToCm ( uint height )
    {
        // Use double division and specify midpoint rounding to avoid ambiguous overloads and banker's rounding
        return ( uint )Math.Round ( height / ( double )AppConfiguration.UI.DeskHeightFactor ,
                                    MidpointRounding.AwayFromZero ) ;
    }
}
