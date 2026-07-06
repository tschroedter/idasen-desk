using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Exceptions ;
using Idasen.TestLogger ;

namespace Idasen.SystemTray.Win11.Tests.Exceptions ;

public sealed class ErrorHandlerTests : IDisposable
{
    private readonly InMemoryLogger _logger = new( ) ;

    public void Dispose ( )
    {
        _logger.Dispose ( ) ;
    }

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
        _logger
           .Contains ( "Bluetooth seems to be disabled or unavailable. Please enable Bluetooth in Windows settings and try again." )
           .Should ( )
           .BeTrue ( ) ;
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
        _logger
           .Contains ( "Test" )
           .Should ( )
           .BeTrue ( ) ;
    }

    [ Fact ]
    public void Handle_ShouldDoNothing_WhenNoHandlerFound ( )
    {
        // Arrange  
        var sut = CreateSut ( ) ;
        sut.Handlers.Clear ( ) ; // No handlers added
        var exception = new InvalidOperationException ( "Test" ) ;

        // Act
        // Assert
        Assert.Throws < InvalidOperationException > ( ( ) => sut.Handle ( exception ,
                                                                          _logger ) ) ;
    }

    private static ErrorHandler CreateSut ( )
    {
        return new ErrorHandler ( ) ;
    }
}
