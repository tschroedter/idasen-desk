using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.ConstructorNullTester
{
    [ TestClass ]
    public class DeskCharacteristicsCreatorConstructorNullTests
        : BaseConstructorNullTester < DeskCharacteristicsCreator >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}