using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using Serilog.Events ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class VersionProviderTests
{
    [ Fact ]
    public void GetVersion_ShouldLogError_WhenAssemblyVersionIsNull ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
        versionProvider.GetAssemblyVersion ( ).Returns ( ( Version ? )null ) ;
        var sut = new VersionProvider ( logger ,
                                        versionProvider ) ;

        // Act
        var result = sut.GetVersion ( ) ;

        // Assert
        result.Should ( ).Be ( "v-.-.-" ) ;
        logger.Contains ( "Failed to get version" ,
                          LogEventLevel.Error )
              .Should ( )
              .BeTrue ( ) ;
    }

    [ Fact ]
    public void GetVersion_ShouldReturnFormattedVersion_AndCache ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
        versionProvider.GetAssemblyVersion ( ).Returns ( new Version ( 1 ,
                                                                       2 ,
                                                                       3 ) ) ;
        var sut = new VersionProvider ( logger ,
                                        versionProvider ) ;

        // Act
        var first  = sut.GetVersion ( ) ;
        var second = sut.GetVersion ( ) ;

        // Assert
        first.Should ( ).Be ( "v1.2.3" ) ;
        second.Should ( ).Be ( "v1.2.3" ) ;
        versionProvider.Received ( 1 ).GetAssemblyVersion ( ) ;
        logger.Contains ( "Version fetched successfully: \"v1.2.3\"" ,
                          LogEventLevel.Information )
              .Should ( )
              .BeTrue ( ) ;
    }

    [ Fact ]
    public void GetVersion_ShouldLogException_WhenVersionProviderThrows ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
        var       ex              = new InvalidOperationException ( "boom" ) ;
        versionProvider.GetAssemblyVersion ( ).Returns ( _ => throw ex ) ;
        var sut = new VersionProvider ( logger ,
                                        versionProvider ) ;

        // Act
        var result = sut.GetVersion ( ) ;

        // Assert
        result.Should ( ).Be ( "v-.-.-" ) ;
        logger.Contains ( "Failed to get version" ,
                          LogEventLevel.Error )
              .Should ( )
              .BeTrue ( ) ;
    }
}
