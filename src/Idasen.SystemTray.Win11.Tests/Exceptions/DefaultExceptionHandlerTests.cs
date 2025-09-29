using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Exceptions ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Exceptions ;

public class DefaultExceptionHandlerTests
{
    private readonly ILogger _logger = Substitute.For < ILogger > ( ) ;

    [ Fact ]
    public void CanHandle_ShouldAlwaysReturnTrue ( )
    {
        // Arrange  
        var exception = new InvalidOperationException ( "Test exception" ) ;

        // Act  
        var result = CreateSut ( ).CanHandle ( exception ) ;

        // Assert  
        result.Should ( )
              .BeTrue ( ) ;
    }

    [ Fact ]
    public void Handle_ShouldLogErrorWithExceptionMessage ( )
    {
        // Arrange  
        var exception = new InvalidOperationException ( "Test exception" ) ;

        // Act  
        CreateSut ( ).Handle ( exception ,
                               _logger ) ;

        // Assert  
        _logger.Received ( 1 )
               .Error ( exception ,
                        exception.Message ) ;
    }

    private static DefaultExceptionHandler CreateSut ( )
    {
        return new DefaultExceptionHandler ( ) ;
    }
}