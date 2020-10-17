using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [TestClass]
    public class GattServiceCsvHelperMapConstructorNullTests
        : BaseConstructorNullTester<GattServiceCsvHelperMap>
    {
        public override int NumberOfConstructorsPassed { get; } = 0;
    }
}