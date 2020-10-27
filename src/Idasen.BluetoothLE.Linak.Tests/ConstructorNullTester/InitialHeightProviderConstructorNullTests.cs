using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Idasen.BluetoothLE.Linak.Control ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.ConstructorNullTester
{
    [ TestClass ]
    public class InitialHeightProviderConstructorNullTests
        : BaseConstructorNullTester < InitialHeightProvider > // todo BaseConstructorNullTester should be in a shared assembly
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}