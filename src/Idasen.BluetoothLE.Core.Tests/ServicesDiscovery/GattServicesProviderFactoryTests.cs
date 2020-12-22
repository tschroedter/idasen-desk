using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Common.Tests ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery
{
    [ AutoDataTestClass ]
    public class GattServicesProviderFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForWrapperNull_Throws (
            GattServicesProviderFactory sut )
        {
            Action action = ( ) => sut.Create ( null! ) ;

            action.Should ( )
                  .Throw < ArgumentNullException > ( )
                  .WithParameter ( "wrapper" ) ;
        }

        [ AutoDataTestMethod ]
        public void Create_ForWrapper_Instance (
            GattServicesProviderFactory sut ,
            IBluetoothLeDeviceWrapper   wrapper )
        {
            sut.Create ( wrapper )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}