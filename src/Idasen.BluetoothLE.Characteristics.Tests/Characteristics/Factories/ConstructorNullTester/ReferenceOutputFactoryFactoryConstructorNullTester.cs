using Idasen.BluetoothLE.Characteristics.Characteristics.Factories ;
using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Factories.ConstructorNullTester
{
    [ TestClass ]
    public class ReferenceOutputFactoryFactoryConstructorNullTester
        : BaseConstructorNullTester < ReferenceOutputFactory >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}