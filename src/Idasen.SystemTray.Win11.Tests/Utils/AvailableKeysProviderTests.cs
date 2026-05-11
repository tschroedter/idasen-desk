using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class AvailableKeysProviderTests
{
    [ Fact ]
    public void AvailableKeys_ShouldNotBeNull ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldNotBeEmpty ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).NotBeEmpty ( ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldContainArrowKeys ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).Contain ( "Up" ) ;
        keys.Should ( ).Contain ( "Down" ) ;
        keys.Should ( ).Contain ( "Left" ) ;
        keys.Should ( ).Contain ( "Right" ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldContainFunctionKeys ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).Contain ( "F1" ) ;
        keys.Should ( ).Contain ( "F2" ) ;
        keys.Should ( ).Contain ( "F3" ) ;
        keys.Should ( ).Contain ( "F4" ) ;
        keys.Should ( ).Contain ( "F5" ) ;
        keys.Should ( ).Contain ( "F6" ) ;
        keys.Should ( ).Contain ( "F7" ) ;
        keys.Should ( ).Contain ( "F8" ) ;
        keys.Should ( ).Contain ( "F9" ) ;
        keys.Should ( ).Contain ( "F10" ) ;
        keys.Should ( ).Contain ( "F11" ) ;
        keys.Should ( ).Contain ( "F12" ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldHaveExpectedCount ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        // 4 arrow keys + 12 function keys = 16 total
        keys.Should ( ).HaveCount ( 16 ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldReturnReadOnlyList ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).BeAssignableTo < IReadOnlyList < string > > ( ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldReturnSameInstanceOnMultipleCalls ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys1 = provider.AvailableKeys ;
        var keys2 = provider.AvailableKeys ;

        // Assert
        keys1.Should ( ).BeSameAs ( keys2 ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldBeInExpectedOrder ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys [ 0 ].Should ( ).Be ( "Up" ) ;
        keys [ 1 ].Should ( ).Be ( "Down" ) ;
        keys [ 2 ].Should ( ).Be ( "Left" ) ;
        keys [ 3 ].Should ( ).Be ( "Right" ) ;
        keys [ 4 ].Should ( ).Be ( "F1" ) ;
        keys [ 15 ].Should ( ).Be ( "F12" ) ;
    }

    private static AvailableKeysProvider CreateSut ( )
    {
        return new AvailableKeysProvider ( ) ;
    }
}
