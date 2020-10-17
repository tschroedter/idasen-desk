using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class DeskCommandExecutorFactory
        : IDeskCommandExecutorFactory
    {
        private readonly DeskCommandExecutor.Factory _factory;

        public DeskCommandExecutorFactory([NotNull] DeskCommandExecutor.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        public IDeskCommandExecutor Create(IControl control)
        {
            Guard.ArgumentNotNull(control,
                                  nameof(control));

            return _factory(control);
        }
    }
}