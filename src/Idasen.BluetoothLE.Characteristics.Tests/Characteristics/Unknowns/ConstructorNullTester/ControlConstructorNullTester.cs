using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns.ConstructorNullTester
{
    [ TestClass ]
    public class ControlConstructorNullTester
        : BaseConstructorNullTester < Control >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}