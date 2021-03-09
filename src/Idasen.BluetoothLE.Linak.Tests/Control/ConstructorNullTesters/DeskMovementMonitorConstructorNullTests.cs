using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Linak.Tests.Control.ConstructorNullTesters
{
    [ TestClass ]
    public class DeskMovementMonitorConstructorNullTests
        : BaseConstructorNullTester < Desk >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}