using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class DeskMoverFactory
        : IDeskMoverFactory
    {
        private readonly DeskMover.Factory _factory;

        public DeskMoverFactory([NotNull] DeskMover.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        public IDeskMover Create(IDeskCommandExecutor executor,
                                 IDeskHeightAndSpeed  heightAndSpeed)
        {
            Guard.ArgumentNotNull(executor,
                                  nameof(executor));
            Guard.ArgumentNotNull(heightAndSpeed,
                                  nameof(heightAndSpeed));

            return _factory(executor,
                            heightAndSpeed);
        }
    }
}