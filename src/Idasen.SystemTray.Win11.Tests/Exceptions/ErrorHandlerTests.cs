using Idasen.SystemTray.Win11.Utils.Exceptions ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Exceptions ;

public class ErrorHandlerTests
{
    private readonly ILogger _logger = Substitute.For < ILogger > ( ) ;

    [ Theory ]
    [ InlineData ( - 2147023729 ) ] // Simulating a Bluetooth disabled exception
    [ InlineData ( - 2147020577 ) ] // Simulating a Bluetooth disabled exception
    public void Handle_ShouldCallHandler_WhenBluetoothException ( int hresult )
    {
        // Arrange  
        var sut       = CreateSut ( ) ;
        var exception = new Exception { HResult = hresult } ;

        // Act  
        sut.Handle ( exception ,
                     _logger ) ;

        // Assert
        _logger.Received ( 1 )
               .Information ( Arg.Any < string > ( ) ) ;
    }

    [ Fact ]
    public void Handle_ShouldCallHandler_WhenNotBluetoothException ( )
    {
        // Arrange  
        var sut       = CreateSut ( ) ;
        var exception = new Exception ( "Test" ) ;

        // Act  
        sut.Handle ( exception ,
                     _logger ) ;

        // Assert
        _logger.Received ( 1 )
               .Error ( exception ,
                        exception.Message ) ;
    }

    private ErrorHandler CreateSut ( )
    {
        return new ErrorHandler ( ) ;
    }
}