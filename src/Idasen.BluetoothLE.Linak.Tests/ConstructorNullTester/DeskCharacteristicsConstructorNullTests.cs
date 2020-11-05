using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
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