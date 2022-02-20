using System.Windows ;

namespace Idasen.SystemTray.Interfaces
{
    public interface ITaskbarIconProviderFactory
    {
        ITaskbarIconProvider Create ( Application application ) ;
    }
}