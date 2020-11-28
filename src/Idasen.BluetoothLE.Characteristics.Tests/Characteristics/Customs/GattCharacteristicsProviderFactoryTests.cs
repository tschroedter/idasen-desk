using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Customs ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics.Customs
{
    [ AutoDataTestClass ]
    public class GattCharacteristicsProviderFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvoked_Instance (
            GattCharacteristicsProviderFactory sut ,
            IGattCharacteristicsResultWrapper  wrapper )
        {
            sut.Create ( wrapper )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}