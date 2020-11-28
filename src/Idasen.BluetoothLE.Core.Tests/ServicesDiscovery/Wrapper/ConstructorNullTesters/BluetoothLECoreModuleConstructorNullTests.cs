using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    // ReSharper disable once InconsistentNaming
    public class BluetoothLECoreModuleConstructorNullTests
        : BaseConstructorNullTester < BluetoothLECoreModule >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}