using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common.ConstructorNullTester
{
    [ TestClass ]
    public class AllGattCharacteristicsProviderConstructorNullTests
        : BaseConstructorNullTester < AllGattCharacteristicsProvider >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}