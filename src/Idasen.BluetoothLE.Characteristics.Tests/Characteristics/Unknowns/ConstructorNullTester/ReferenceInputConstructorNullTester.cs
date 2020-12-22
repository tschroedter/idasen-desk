using Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns ;
using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Unknowns.ConstructorNullTester
{
    [ TestClass ]
    public class ReferenceInputConstructorNullTester
        : BaseConstructorNullTester < ReferenceInput >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}