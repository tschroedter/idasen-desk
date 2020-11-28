using System ;
using System.Collections.Generic ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;

namespace Idasen.BluetoothLE.Linak
{
    public static class DeskCharacteristicDictionaryExtensions
    {
        public static readonly Dictionary < DeskCharacteristicKey , ICharacteristicBase > UnknownBases =
            new Dictionary < DeskCharacteristicKey , ICharacteristicBase >
            {
                { DeskCharacteristicKey.GenericAccess , new GenericAccess ( ) } ,
                { DeskCharacteristicKey.GenericAttribute , new GenericAttribute ( ) } ,
                { DeskCharacteristicKey.ReferenceInput , new ReferenceInput ( ) } ,
                { DeskCharacteristicKey.ReferenceOutput , new ReferenceOutput ( ) } ,
                { DeskCharacteristicKey.Dpg , new Dpg ( ) } ,
                { DeskCharacteristicKey.Control , new Characteristics.Characteristics.Unknowns.Control ( ) }
            } ;

        public static T As < T > (
            this Dictionary < DeskCharacteristicKey , ICharacteristicBase > dictionary ,
            DeskCharacteristicKey                                           key )
        {
            if ( dictionary.TryGetValue ( key ,
                                          out var characteristicBase ) )
                return ( T ) characteristicBase ;

            if ( UnknownBases.TryGetValue ( key ,
                                            out characteristicBase ) )
                return ( T ) characteristicBase ;

            throw new ArgumentException ( "" ,
                                          nameof ( key ) ) ;
        }
    }
}