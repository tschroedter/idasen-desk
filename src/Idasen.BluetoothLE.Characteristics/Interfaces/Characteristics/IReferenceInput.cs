using System.Collections.Generic ;

// ReSharper disable UnusedMemberInSuper.Global

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    public interface IReferenceInput
        : ICharacteristicBase
    {
        IEnumerable < byte > Ctrl1 { get ; }
    }
}