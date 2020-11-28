using System.Diagnostics.CodeAnalysis ;

namespace Idasen.BluetoothLE.Characteristics.Common
{
    [ SuppressMessage ( "NDepend" ,
                        "ND1207:NonStaticClassesShouldBeInstantiatedOrTurnedToStatic" ,
                        Justification = "Class is used in AllGattCharacteristicsProvider" ) ]
    public sealed class CsvGattCharacteristic
    {
        public string Uuid { get ; set ; }

        public string Name { get ; set ; }
    }
}