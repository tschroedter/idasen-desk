using Idasen.BluetoothLE.Characteristics.Characteristics.Customs ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Customs.ConstructorNullTester
{
    [ TestClass ]
    public class GattCharacteristicProviderConstructorNullTester
        : BaseConstructorNullTester < GattCharacteristicProvider >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}