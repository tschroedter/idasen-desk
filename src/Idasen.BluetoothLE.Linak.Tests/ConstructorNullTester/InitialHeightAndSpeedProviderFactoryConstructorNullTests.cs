using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Linak.Control ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.ConstructorNullTester
{
    [ TestClass ]
    public class InitialHeightAndSpeedProviderFactoryConstructorNullTests
        : BaseConstructorNullTester < InitialHeightAndSpeedProviderFactory >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}