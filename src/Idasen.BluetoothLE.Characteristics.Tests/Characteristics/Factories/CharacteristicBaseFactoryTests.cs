using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Factories
{
    [ AutoDataTestClass ]
    public class CharacteristicBaseFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvoked_Instance (
            CharacteristicBaseFactory sut ,
            IDevice                   device )
        {
            // not easy to  test because of dependency on ILifetimeScope
            sut.Create < object > ( device )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}