using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common.ConstructorNullTester
{
    [ TestClass ]
    public class TestSimpleDictionaryBaseConstructorNullTests
        : BaseConstructorNullTester < RawValueWriter >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}