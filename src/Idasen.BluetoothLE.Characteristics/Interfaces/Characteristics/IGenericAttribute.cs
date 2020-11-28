using System.Collections.Generic ;

// ReSharper disable UnusedMemberInSuper.Global

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IGenericAttribute
        : ICharacteristicBase
    {
        IEnumerable < byte > RawServiceChanged { get ; }
    }
}