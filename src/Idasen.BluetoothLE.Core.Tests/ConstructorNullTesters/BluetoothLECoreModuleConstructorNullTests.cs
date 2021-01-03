using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ConstructorNullTesters
{
    [ TestClass ]
    // ReSharper disable once InconsistentNaming
    public class BluetoothLECoreModuleConstructorNullTests
        : BaseConstructorNullTester < BluetoothLECoreModule >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}