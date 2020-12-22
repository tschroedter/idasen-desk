using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    public class ObservableTimerFactoryConstructorNullTests
        : BaseConstructorNullTester < ObservableTimerFactory >
    {
        public override int NumberOfConstructorsPassed { get ; } = 0 ;
    }
}