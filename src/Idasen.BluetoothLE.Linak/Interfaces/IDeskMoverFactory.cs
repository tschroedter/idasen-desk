using Idasen.BluetoothLE.Linak.Control;
using JetBrains.Annotations;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskMoverFactory
    {
        IDeskMover Create([NotNull] IDeskCommandExecutor executor,
                          [NotNull] IDeskHeightAndSpeed  heightAndSpeed);
    }
}