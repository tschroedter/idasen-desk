using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Core.Tests.DevicesDiscovery.ConstructorNullTesters ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns.ConstructorNullTester
{
    [ TestClass ]
    public class GenericAttributeConstructorNullTester
        : BaseConstructorNullTester < GenericAttribute >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}