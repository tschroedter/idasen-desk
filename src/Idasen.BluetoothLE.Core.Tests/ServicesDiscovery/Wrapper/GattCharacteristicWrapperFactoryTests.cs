using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.Wrapper
{
    [ AutoDataTestClass ]
    public class GattCharacteristicWrapperFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForCharacteristicNull_Throws (
            GattCharacteristicWrapperFactory sut )
        {
            Action action = ( ) => sut.Create ( null! ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( ) ;
        }
    }
}