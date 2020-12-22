using Idasen.BluetoothLE.Common.Tests ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.ConstructorNullTesters
{
    [ TestClass ]
    public class ResourceNotFoundExceptionConstructorNullTests
        : BaseConstructorNullTester < ResourceNotFoundException >
    {
        public override int NumberOfConstructorsPassed { get ; } = 1 ;
    }
}