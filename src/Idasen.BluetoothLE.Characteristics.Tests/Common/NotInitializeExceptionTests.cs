using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ AutoDataTestClass ]
    public class NotInitializeExceptionTests
    {
        [ AutoDataTestMethod ]
        public void Constructor_ForInvoked_SetsMessage (
            string message )
        {
            var sut = new NotInitializeException ( message ) ;

            sut.Message
               .Should ( )
               .Be ( message ) ;
        }
    }
}