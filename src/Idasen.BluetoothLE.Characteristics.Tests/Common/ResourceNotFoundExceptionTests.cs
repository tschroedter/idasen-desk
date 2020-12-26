using FluentAssertions ;
using Idasen.BluetoothLE.Core ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ TestClass ]
    public class ResourceNotFoundExceptionTests
    {
        private const string ResourceName = "ResourceName" ;
        private const string Message      = "Message" ;

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsMessage ( )
        {
            var sut = new ResourceNotFoundException ( ResourceName ,
                                                      Message ) ;

            sut.Message
               .Should ( )
               .Be ( Message ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsResourceName ( )
        {
            var sut = new ResourceNotFoundException ( ResourceName ,
                                                      Message ) ;

            sut.ResourceName
               .Should ( )
               .Be ( ResourceName ) ;
        }
    }
}