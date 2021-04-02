using System.Text.RegularExpressions ;

namespace Idasen.BluetoothLE.Core
{
    public static class ULongExtensions
    {
        private const string Grouping = "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})" ;
        private const string Replace  = "$1:$2:$3:$4:$5:$6" ;

        /// <summary>
        ///     Converts a ulong value into a Mac Address, e.g. 254682828386071
        ///     will be 'E7:A1:F7:84:2F:17'.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     The Mac Address.
        /// </returns>
        public static string ToMacAddress ( this ulong value ) // todo testing
        {
            var tempMac = value.ToString ( "X" ) ;

            var macAddress = Regex.Replace ( tempMac ,
                                             Grouping ,
                                             Replace ) ;

            return macAddress ;
        }
    }
}