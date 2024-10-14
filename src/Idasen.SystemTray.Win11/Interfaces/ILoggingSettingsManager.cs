using Autofac;

namespace Idasen.SystemTray.Win11.Interfaces
{
    public interface ILoggingSettingsManager : ISettingsManager
    {
        void Initialize(IContainer container);
    }
}