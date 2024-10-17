using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils.Icons ;

public class TaskbarIconProvider : ITaskbarIconProvider
{
    private readonly IDynamicIconCreator _creator ;
    private readonly IScheduler          _scheduler ;
    private          IDisposable ?       _disposable ;
    private          ILogger ?           _logger ;

    public TaskbarIconProvider ( IScheduler          scheduler ,
                                 IDynamicIconCreator creator )
    {
        Guard.ArgumentNotNull ( scheduler ,
                                nameof ( scheduler ) ) ;
        Guard.ArgumentNotNull ( creator ,
                                nameof ( creator ) ) ;

        _scheduler = scheduler ;
        _creator   = creator ;
    }

    public NotifyIcon ? NotifyIcon { get ; private set ; }


    public void Dispose ( )
    {
        NotifyIcon?.Dispose ( ) ;
        _disposable?.Dispose ( ) ;
    }

    public void Initialize ( ILogger      logger ,
                             IDesk        desk ,
                             NotifyIcon ? notifyIcon )
    {
        Guard.ArgumentNotNull ( logger ,
                                nameof ( logger ) ) ;
        Guard.ArgumentNotNull ( desk ,
                                nameof ( desk ) ) ;
        _logger    = logger ;
        NotifyIcon = notifyIcon ;

        _disposable = desk.HeightAndSpeedChanged
                          .ObserveOn ( _scheduler )
                          .Subscribe ( OnHeightAndSpeedChanged ) ;
    }

    private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
    {
        if ( NotifyIcon == null )
        {
            _logger?.Error ( "NotifyIcon is null" ) ;

            return ;
        }

        var heightInCm = Convert.ToInt32 ( Math.Round ( details.Height / 100.0 ) ) ;

        _creator.Update ( NotifyIcon , heightInCm ) ;

        _logger?.Debug ( $"Height: {heightInCm}" ) ;
    }
}