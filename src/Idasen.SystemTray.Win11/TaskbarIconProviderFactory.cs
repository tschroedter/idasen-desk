using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11
{
    public class TaskbarIconProviderFactory : ITaskbarIconProviderFactory
    {
        public TaskbarIconProviderFactory(TaskbarIconProvider.Factory factory)
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory;
        }

        public ITaskbarIconProvider Create(Application application)
        {
            if (_factory(application) is not ITaskbarIconProvider taskbarIconProvider)
            {
                throw new Exception($"Failed to create {nameof(ITaskbarIconProvider)}");
            }

            return taskbarIconProvider;
        }

        private readonly TaskbarIconProvider.Factory _factory;
    }
}