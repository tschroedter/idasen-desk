using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Factories.ConstructorNullTester
{
    [ TestClass ]
    public class CharacteristicBaseFactoryConstructorNullTester
        : BaseConstructorNullTester < CharacteristicBaseFactory >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}