using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using Serilog ;

#pragma warning disable CA2012 // Use ValueTasks correctly - disabled for test mocking

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class DeskMovementManagerTests
{
    [ Fact ]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException ( )
    {
        // Arrange
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;

        // Act
        var act = ( ) => new DeskMovementManager ( null! ,
                                                   settingsManager ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_WithNullSettingsManager_ThrowsArgumentNullException ( )
    {
        // Arrange
        var logger = Substitute.For < ILogger > ( ) ;

        // Act
        var act = ( ) => new DeskMovementManager ( logger ,
                                                   null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "settingsManager" ) ;
    }

    [ Fact ]
    public void SetDeskAccessor_WithNullAccessor_ThrowsArgumentNullException ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        // Act
        var act = ( ) => sut.SetDeskAccessor ( null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "deskAccessor" ) ;
    }

    [ Fact ]
    public void IsDeskAvailable_WhenDeskAccessorNotSet_ReturnsFalse ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        // Act
        var result = sut.IsDeskAvailable ( ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void IsDeskAvailable_WhenDeskAccessorReturnsNull_ReturnsFalse ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        sut.SetDeskAccessor ( ( ) => null ) ;

        // Act
        var result = sut.IsDeskAvailable ( ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void IsDeskAvailable_WhenDeskAccessorReturnsDesk_ReturnsTrue ( )
    {
        // Arrange
        var logger          = Substitute.For < ILogger > ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var desk            = Substitute.For < IDesk > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        sut.SetDeskAccessor ( ( ) => desk ) ;

        // Act
        var result = sut.IsDeskAvailable ( ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task MoveToHeightAsync_WhenDeskNotAvailable_DoesNotMoveDesk ( )
    {
        // Arrange
        var logger          = new InMemoryLogger ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        sut.SetDeskAccessor ( ( ) => null ) ;

        // Act
        await sut.MoveToHeightAsync ( 120 ,
                                      "TestOperation" ) ;

        // Assert
        await settingsManager.DidNotReceive ( ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task MoveToHeightAsync_WhenDeskAvailable_MovesDeskToCorrectHeight ( )
    {
        // Arrange
        var logger          = new InMemoryLogger ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var desk            = Substitute.For < IDesk > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        sut.SetDeskAccessor ( ( ) => desk ) ;

        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Act
        await sut.MoveToHeightAsync ( 120 ,
                                      "Stand" ) ;

        // Assert
        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 12000u ) ; // 120 cm * 100
    }

    [ Fact ]
    public async Task MoveToHeightAsync_ConvertsHeightCorrectly ( )
    {
        // Arrange
        var logger          = new InMemoryLogger ( ) ;
        var settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var desk            = Substitute.For < IDesk > ( ) ;
        var sut = new DeskMovementManager ( logger ,
                                            settingsManager ) ;

        sut.SetDeskAccessor ( ( ) => desk ) ;

        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Act
        await sut.MoveToHeightAsync ( 75 ,
                                      "Sit" ) ;

        // Assert
        desk.Received ( 1 ).MoveTo ( 7500u ) ; // 75 cm * 100
    }
}
