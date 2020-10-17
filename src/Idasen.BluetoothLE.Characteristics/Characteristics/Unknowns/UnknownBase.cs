using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class UnknownBase : ICharacteristicBase
    {
        protected static readonly IEnumerable<byte> RawArrayEmpty = Enumerable.Empty<byte>()
                                                                              .ToArray();

        public T Initialize<T>() where T : class
        {
            return this as T;
        }

        public Task Refresh()
        {
            return Task.FromResult(false);
        }
    }
}