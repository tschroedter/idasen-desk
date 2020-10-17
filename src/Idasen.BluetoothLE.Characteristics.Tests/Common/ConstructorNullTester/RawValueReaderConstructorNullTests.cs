using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common.ConstructorNullTester
{
    [ TestClass ]
    public class RawValueReaderConstructorNullTests
        : BaseConstructorNullTester < RawValueReader >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}