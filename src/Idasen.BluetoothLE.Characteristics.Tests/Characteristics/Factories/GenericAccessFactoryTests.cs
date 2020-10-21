using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Factories ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Factories
{
    [ AutoDataTestClass ]
    public class GenericAccessFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvoked_Instance (
            GenericAccessFactory sut ,
            IDevice              device )
        {
            sut.Create ( device )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}