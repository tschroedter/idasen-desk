using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskHeightAndSpeedFactory
    {
        IDeskHeightAndSpeed Create ( [ NotNull ] IReferenceOutput referenceOutput ) ;
    }
}