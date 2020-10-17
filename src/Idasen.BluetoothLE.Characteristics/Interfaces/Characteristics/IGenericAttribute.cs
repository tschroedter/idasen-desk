using System.Collections.Generic;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IGenericAttribute
        : ICharacteristicBase
    {
        IEnumerable<byte> RawServiceChanged { get; }
    }
}