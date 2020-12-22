using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.ConstructorNullTester
{
    [ TestClass ]
    public class ControlNullTests
        : BaseConstructorNullTester < Control >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}