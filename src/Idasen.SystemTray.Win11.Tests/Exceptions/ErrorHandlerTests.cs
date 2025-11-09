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
        var exception = new InvalidOperationException { HResult = hresult } ;

        // Act  
        sut.Handle ( exception ,
                     _logger ) ;

        // Assert
        _logger.Received ( 1 )
               .Warning ( "Bluetooth seems to be disabled or unavailable. Please enable Bluetooth in Windows settings and try again." ) ;
    }

    [ Fact ]
    public void Handle_ShouldCallHandler_WhenNotBluetoothException ( )
    {
        // Arrange  
        var sut       = CreateSut ( ) ;
        var exception = new InvalidOperationException ( "Test" ) ;

        // Act  
        sut.Handle ( exception ,
                     _logger ) ;

        // Assert
        _logger.Received ( 1 )
               .Error ( exception ,
                        "Test" ) ;
    }

    [ Fact ]
    public void Handle_ShouldDoNothing_WhenNoHandlerFound ( )
    {
        // Arrange  
        var sut = CreateSut ( ) ;
        sut.Handlers.Clear ( ) ; // No handlers added
        var exception = new InvalidOperationException ( "Test" ) ;

        // Act  
        sut.Handle ( exception ,
                     _logger ) ;

        // Assert
#pragma warning disable CA2254
        _logger.Received ( 0 )
               .Warning ( exception ,
                          Arg.Is<string>(s => s.Contains("No handler found for exception:"))) ;
#pragma warning restore CA2254
    }

    private static ErrorHandler CreateSut ( )
    {
        return new ErrorHandler ( ) ;
    }
}