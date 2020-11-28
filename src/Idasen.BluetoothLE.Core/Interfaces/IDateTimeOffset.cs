using System ;

namespace Idasen.BluetoothLE.Core.Interfaces
{
    /// <summary>
    ///     Represents a point in time, typically expressed as a date and time
    ///     of day, relative to Coordinated Universal Time (UTC).
    /// </summary>
    public interface IDateTimeOffset
    {
        /// <summary>
        ///     Gets a DateTimeOffset object that is set to the current date and
        ///     time on the current computer, with the offset set to the local
        ///     time's offset from Coordinated Universal Time (UTC).
        /// </summary>
        IDateTimeOffset Now { get ; }

        /// <summary>
        ///     Gets the number of ticks that represents the date and time of
        ///     the current DateTimeOffset object in clock time.
        /// </summary>
        long Ticks { get ; }

        /// <summary>
        ///     Converts the value of the current DateTimeOffset object to its
        ///     equivalent string representation using the specified format and
        ///     culture-specific format information.
        /// </summary>
        /// <param name="format">
        ///     A format string.
        /// </param>
        /// <param name="formatProvider">
        ///     An object that supplies culture-specific formatting information.
        /// </param>
        /// <returns>
        ///     A string representation of the value of the current DateTimeOffset
        ///     object, as specified by format and provider.
        /// </returns>
        string ToString ( string          format ,
                          IFormatProvider formatProvider ) ;
    }
}