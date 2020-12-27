using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    public class DeviceConstructorTests
        : BaseConstructorNullTester < AssertionScope >
    {
        public override int NumberOfConstructorsPassed { get ; } = 2 ;
    }
}