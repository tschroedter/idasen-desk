using Idasen.BluetoothLE.Characteristics.Characteristics ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    /// <summary>
    ///     todo
    /// </summary>
    public interface ICharacteristicBaseToStringConverter
    {
        /// <summary>
        ///     todo
        /// </summary>
        [ NotNull ]
        string ToString ( CharacteristicBase characteristic ) ;
    }
}