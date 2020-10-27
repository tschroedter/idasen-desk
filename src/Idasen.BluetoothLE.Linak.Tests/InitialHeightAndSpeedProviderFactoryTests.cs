using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ AutoDataTestClass ]
    public class InitialHeightAndSpeedProviderFactoryTests
    {
        [ AutoDataTestMethod ]
        public void Create_ForInvoked_Instance (
            InitialHeightAndSpeedProviderFactory sut ,
            IDeskCommandExecutor                 executor ,
            IDeskHeightAndSpeed                  heightAndSpeed )
        {
            sut.Create ( executor ,
                         heightAndSpeed )
               .Should ( )
               .NotBeNull ( ) ;
        }
    }
}