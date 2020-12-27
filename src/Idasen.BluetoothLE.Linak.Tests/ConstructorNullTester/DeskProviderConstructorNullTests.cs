using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.ConstructorNullTester
{
    [ TestClass ]
    public class DeskProviderConstructorNullTests
        : BaseConstructorNullTester < DeskProvider >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}