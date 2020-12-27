using System ;
using System.Collections.Generic ;
using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core
{
    [ ExcludeFromCodeCoverage ]
    public static class GattCharacteristicPropertiesExtensions
    {
        /// <summary>
        ///     Converts the <see cref="GattCharacteristicProperties" /> into a
        ///     string containing the property names separated by comma.
        /// </summary>
        /// <param name="properties">
        ///     This <see cref="GattCharacteristicProperties" /> to convert into a string.
        /// </param>
        /// <returns>
        ///     A string containing the property names separated by a comma.
        /// </returns>
        public static string ToCsv ( this GattCharacteristicProperties properties )
        {
            var list = new List < GattCharacteristicProperties > ( ) ;

            foreach ( GattCharacteristicProperties property in
                Enum.GetValues ( typeof ( GattCharacteristicProperties ) ) )
            {
                if ( ( properties & property ) == property ) list.Add ( property ) ;
            }

            return string.Join ( ", " ,
                                 list ) ;
        }
    }
}