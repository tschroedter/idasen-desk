using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    public class GattServicesDictionaryConstructorNullTests
        : BaseConstructorNullTester < GattServicesDictionary >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}