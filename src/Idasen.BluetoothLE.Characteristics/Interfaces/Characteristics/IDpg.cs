using System.Collections.Generic;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IDpg
        : ICharacteristicBase
    {
        delegate IDpg Factory(IDevice device);

        IEnumerable<byte> RawDpg { get; }
    }
}