using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.ConstructorNullTester
{
    [ TestClass ]
    public class DeskCharacteristicsConstructorNullTests
        : BaseConstructorNullTester < DeskCharacteristics >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}