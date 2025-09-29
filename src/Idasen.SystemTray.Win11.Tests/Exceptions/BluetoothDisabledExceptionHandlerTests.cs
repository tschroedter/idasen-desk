using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.SystemTray.Win11.Utils.Exceptions ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Exceptions ;

public class BluetoothDisabledExceptionHandlerTests
{
    private readonly ILogger _logger = Substitute.For < ILogger > ( ) ;


    [ Theory ]
    [ InlineData ( - 2147023729 ) ] // Simulating a Bluetooth disabled exception
    [ InlineData ( - 2147020577 ) ] // Simulating a Bluetooth disabled exception
    public void CanHandle_ShouldReturnTrue_WhenExceptionIsBluetoothDisabledException ( int hresult )
    {
        // Arrange  
        var exception = new InvalidOperationException( "Bluetooth Exception" )
        {
            HResult = hresult
        } ;

        // Act  
        var result = CreateSut ( ).CanHandle ( exception ) ;

        // Assert  
        result.Should ( )
              .BeTrue ( ) ;
    }

    [ Fact ]
    public void CanHandle_ShouldReturnFalse_WhenExceptionIsNotBluetoothDisabledException ( )
    {
        // Arrange  
        var exception = new InvalidOperationException( "Exception" ) ;

        // Act  
        var result = CreateSut ( ).CanHandle ( exception ) ;

        // Assert  
        result.Should ( )
              .BeFalse ( ) ;
    }

    [ Fact ]
    public void Handle_ShouldLogBluetoothStatusException ( )
    {
        // Arrange  
        var exception = Substitute.For < Exception > ( ) ;

        // Act  
        CreateSut ( ).Handle ( exception ,
                               _logger ) ;

        // Assert  
        exception.Received ( 1 )
                 .LogBluetoothStatusException ( _logger ,
                                                string.Empty ) ;
    }

    private static BluetoothDisabledExceptionHandler CreateSut ( )
    {
        return new BluetoothDisabledExceptionHandler ( ) ;
    }
}