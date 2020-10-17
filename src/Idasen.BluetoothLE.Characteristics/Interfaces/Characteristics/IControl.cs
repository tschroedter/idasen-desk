using System.Collections.Generic;
using System.Threading.Tasks;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IControl
        : ICharacteristicBase
    {
        IEnumerable<byte> RawControl2 { get; }

        IEnumerable<byte> RawControl3 { get; }

        Task<bool> TryWriteRawControl2(IEnumerable<byte> bytes);
    }
}