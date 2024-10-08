namespace Idasen.SystemTray.Win11.Interfaces
{
    public interface ITaskbarIconProviderFactory
    {
        ITaskbarIconProvider Create ( Application application ) ;
    }
}