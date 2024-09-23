using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11
{
    public class TaskbarIconProvider : ITaskbarIconProvider
    {
        public TaskbarIconProvider ( ILogger                logger ,
                                     Application            application ,
                                     IScheduler             scheduler ,
                                     IDynamicIconCreator creator )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( application ,
                                    nameof ( application ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( creator ,
                                    nameof ( creator ) ) ;

            _logger    = logger ;
            _scheduler = scheduler ;
            _creator   = creator ;

            NotifyIcon = ( TaskbarIcon ) ( application.FindResource ( "NotifyIcon" ) ) ;
        }

        public TaskbarIcon ? NotifyIcon { get ; }


        public void Dispose ( )
        {
            NotifyIcon?.Dispose ( ) ;
            _disposable?.Dispose ( ) ;
        }

        public void Initialize ( IDesk desk )
        {
            Guard.ArgumentNotNull ( desk ,
                                    nameof ( desk ) ) ;

            _disposable = desk.HeightAndSpeedChanged
                              .ObserveOn ( _scheduler )
                              .Subscribe ( OnHeightAndSpeedChanged ) ;
        }

        public delegate ITaskbarIconProvider Factory ( Application application ) ;

        private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
        {
            if (NotifyIcon == null)
            {
                _logger.Error("NotifyIcon is null");

                return;
            }
            var heightInCm = Convert.ToInt32 ( Math.Round ( details.Height / 100.0 ) ) ;

            _creator.Update ( NotifyIcon, heightInCm ) ;

            _logger.Debug($"Height: {heightInCm}");
        }

        private readonly IDynamicIconCreator _creator ;
        private readonly ILogger             _logger ;
        private readonly IScheduler          _scheduler ;

        private IDisposable ? _disposable ;
    }
}