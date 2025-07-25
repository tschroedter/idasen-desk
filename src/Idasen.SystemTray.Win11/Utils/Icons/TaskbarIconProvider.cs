using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

[ ExcludeFromCodeCoverage ]
public class TaskbarIconProvider : ITaskbarIconProvider
{
    private readonly IDynamicIconCreator _creator ;
    private readonly ISettingsManager    _manager ;
    private readonly IScheduler          _scheduler ;
    private readonly object              _syncRoot = new ( ) ;
    private          IDisposable ?       _disposable ;
    private          bool                _isDisposed ;
    private          bool                _isInitialized ;
    private          ILogger ?           _logger ;

    public TaskbarIconProvider (
        IScheduler          scheduler ,
        IDynamicIconCreator creator ,
        ISettingsManager    manager )
    {
        Guard.ArgumentNotNull ( scheduler ,
                                nameof ( scheduler ) ) ;
        Guard.ArgumentNotNull ( creator ,
                                nameof ( creator ) ) ;
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;

        _scheduler = scheduler ;
        _creator   = creator ;
        _manager   = manager ;
    }

    public NotifyIcon ? NotifyIcon { get ; private set ; }

    public void Dispose ( )
    {
        lock ( _syncRoot )
        {
            if ( _isDisposed )
                return ;

            _isDisposed = true ;

            _disposable?.Dispose ( ) ;
            _disposable = null ;

            NotifyIcon?.Dispose ( ) ;
            NotifyIcon = null ;

            _logger?.Information ( "TaskbarIconProvider disposed." ) ;
        }
    }

    public void Initialize (
        ILogger      logger ,
        IDesk        desk ,
        NotifyIcon ? notifyIcon )
    {
        Guard.ArgumentNotNull ( logger ,
                                nameof ( logger ) ) ;
        Guard.ArgumentNotNull ( desk ,
                                nameof ( desk ) ) ;

        lock ( _syncRoot )
        {
            if ( _isDisposed )
            {
                logger.Error ( "TaskbarIconProvider is already disposed." ) ;
                return ;
            }

            if ( _isInitialized )
            {
                logger.Error ( "TaskbarIconProvider is already initialized" ) ;
                return ;
            }

            _logger    = logger ;
            NotifyIcon = notifyIcon ;

            _disposable = desk.HeightAndSpeedChanged
                              .ObserveOn ( _scheduler )
                              .Subscribe ( OnHeightAndSpeedChanged ) ;

            var heightInCm = _manager.CurrentSettings.HeightSettings.LastKnownDeskHeight ;

            if ( heightInCm is >= Constants.DefaultDeskMinHeightInCm and <= Constants.DefaultDeskMaxHeightInCm )
            {
                // HeightSpeedDetails expects height in millimeters
                OnHeightAndSpeedChanged ( new HeightSpeedDetails (
                                                                  DateTimeOffset.Now ,
                                                                  heightInCm * 100 ,
                                                                  0 ) ) ;
            }

            _isInitialized = true ;
        }
    }

    private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
    {
        NotifyIcon ? notifyIcon ;
        ILogger ?    logger ;

        lock ( _syncRoot )
        {
            if ( _isDisposed )
                return ;

            notifyIcon = NotifyIcon ;
            logger     = _logger ;
        }

        if ( notifyIcon == null )
        {
            logger?.Error ( "NotifyIcon is null" ) ;
            return ;
        }

        var heightInCm = ( int ) Math.Round ( details.Height / 100.0 ) ;

        _creator.Update ( notifyIcon ,
                          heightInCm ) ;

        logger?.Debug ( "Height: {HeightInCm}" ,
                        heightInCm ) ;
    }
}