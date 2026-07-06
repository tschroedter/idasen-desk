using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Exceptions ;
using Idasen.TestLogger ;

namespace Idasen.SystemTray.Win11.Tests.Exceptions ;

public sealed class DefaultExceptionHandlerTests : IDisposable
{
    private readonly InMemoryLogger _logger = new( ) ;

    public void Dispose ( )
    {
        _logger.Dispose ( ) ;
    }

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
        _logger.Contains ( exception.Message )
               .Should ( )
               .BeTrue ( ) ;
    }

    private static DefaultExceptionHandler CreateSut ( )
    {
        return new DefaultExceptionHandler ( ) ;
    }
}
