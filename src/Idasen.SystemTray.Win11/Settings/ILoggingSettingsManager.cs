using Autofac ;

namespace Idasen.SystemTray.Win11.Settings
{
    public interface ILoggingSettingsManager : ISettingsManager
    {
        void Initialize ( IContainer container ) ;
    }
}