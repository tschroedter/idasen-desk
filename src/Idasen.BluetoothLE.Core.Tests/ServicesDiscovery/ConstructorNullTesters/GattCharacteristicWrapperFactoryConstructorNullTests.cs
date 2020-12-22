using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    public class GattCharacteristicWrapperFactoryConstructorNullTests
        : BaseConstructorNullTester < GattCharacteristicWrapperFactory >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}