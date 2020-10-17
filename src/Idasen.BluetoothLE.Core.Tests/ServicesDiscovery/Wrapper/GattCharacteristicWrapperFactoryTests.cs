using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.Wrapper
{
    [AutoDataTestClass]
    public class GattCharacteristicWrapperFactoryTests
    {
        [AutoDataTestMethod]
        public void Create_ForUnknownAddress_Throws(
            GattCharacteristicWrapperFactory        sut)
        {
            sut.Should (  )
               .NotBeNull (  ); // todo move successful instance creation into null tester
        }
    }
}