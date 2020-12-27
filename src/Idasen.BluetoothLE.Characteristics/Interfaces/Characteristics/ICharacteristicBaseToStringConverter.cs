using Idasen.BluetoothLE.Characteristics.Characteristics ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics
{
    /// <summary>
    ///     Converts a <see cref="CharacteristicBase" /> instance to a string
    ///     which can be used for logging.
    /// </summary>
    public interface ICharacteristicBaseToStringConverter
    {
        /// <summary>
        ///     Convert a <see cref="CharacteristicBase" /> instance to a string.
        /// </summary>
        [ NotNull ]
        string ToString ( CharacteristicBase characteristic ) ;
    }
}