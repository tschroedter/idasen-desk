using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class VersionProviderTests
{
    [ Fact ]
    public void GetVersion_ShouldLogError_WhenAssemblyVersionIsNull ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
        versionProvider.GetAssemblyVersion ( ).Returns ( ( Version ? )null ) ;
        var sut = new VersionProvider ( logger ,
                                        versionProvider ) ;

        // Act
        var result = sut.GetVersion ( ) ;

        // Assert
        result.Should ( ).Be ( "v-.-.-" ) ;
        logger.Received ( 1 ).Error ( "Failed to get version" ) ;
    }

    [ Fact ]
    public void GetVersion_ShouldReturnFormattedVersion_AndCache ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
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
        logger.Received ( 1 ).Information ( "Version fetched successfully: {Version}" ,
                                            "v1.2.3" ) ;
    }

    [ Fact ]
    public void GetVersion_ShouldLogException_WhenVersionProviderThrows ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var versionProvider = Substitute.For < IAssemblyVersionProvider > ( ) ;
        var ex              = new InvalidOperationException ( "boom" ) ;
        versionProvider.GetAssemblyVersion ( ).Returns ( _ => throw ex ) ;
        var sut = new VersionProvider ( logger ,
                                        versionProvider ) ;

        // Act
        var result = sut.GetVersion ( ) ;

        // Assert
        result.Should ( ).Be ( "v-.-.-" ) ;
        logger.Received ( 1 ).Error ( ex ,
                                      "Failed to get version" ) ;
    }
}