using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.ConstructorNullTester
{
    [ TestClass ]
    public class CharacteristicBaseNullTests
        : BaseConstructorNullTester < TestCharacteristicBase >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}