using System.Windows ;
using Idasen.SystemTray.Interfaces ;

namespace Idasen.SystemTray
{
    public class TaskbarIconProviderFactory : ITaskbarIconProviderFactory
    {
        public TaskbarIconProviderFactory ( TaskbarIconProvider.Factory factory )
        {
            _factory = factory ;
        }

        public ITaskbarIconProvider Create ( Application application )
        {
            return _factory ( application ) ;
        }

        private readonly TaskbarIconProvider.Factory _factory ;
    }
}