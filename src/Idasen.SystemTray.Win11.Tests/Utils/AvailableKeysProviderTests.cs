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
    public void AvailableKeys_ShouldContainNumberKeys ( )
    {
        // Arrange
        var provider = CreateSut ( ) ;

        // Act
        var keys = provider.AvailableKeys ;

        // Assert
        keys.Should ( ).Contain ( "1" ) ;
        keys.Should ( ).Contain ( "2" ) ;
        keys.Should ( ).Contain ( "3" ) ;
        keys.Should ( ).Contain ( "4" ) ;
        keys.Should ( ).Contain ( "5" ) ;
        keys.Should ( ).Contain ( "6" ) ;
        keys.Should ( ).Contain ( "7" ) ;
        keys.Should ( ).Contain ( "8" ) ;
        keys.Should ( ).Contain ( "9" ) ;
        keys.Should ( ).Contain ( "0" ) ;
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
        keys.Should ( ).HaveCount ( 26 ) ;
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
        keys [ 4 ].Should ( ).Be ( "1" ) ;
        keys [ 5 ].Should ( ).Be ( "2" ) ;
        keys [ 6 ].Should ( ).Be ( "3" ) ;
        keys [ 7 ].Should ( ).Be ( "4" ) ;
        keys [ 8 ].Should ( ).Be ( "5" ) ;
        keys [ 9 ].Should ( ).Be ( "6" ) ;
        keys [ 10 ].Should ( ).Be ( "7" ) ;
        keys [ 11 ].Should ( ).Be ( "8" ) ;
        keys [ 12 ].Should ( ).Be ( "9" ) ;
        keys [ 13 ].Should ( ).Be ( "0" ) ;
        keys [ 14 ].Should ( ).Be ( "F1" ) ;
        keys [ 15 ].Should ( ).Be ( "F2" ) ;
        keys [ 16 ].Should ( ).Be ( "F3" ) ;
        keys [ 17 ].Should ( ).Be ( "F4" ) ;
        keys [ 18 ].Should ( ).Be ( "F5" ) ;
        keys [ 19 ].Should ( ).Be ( "F6" ) ;
        keys [ 20 ].Should ( ).Be ( "F7" ) ;
        keys [ 21 ].Should ( ).Be ( "F8" ) ;
        keys [ 22 ].Should ( ).Be ( "F9" ) ;
        keys [ 23 ].Should ( ).Be ( "F10" ) ;
        keys [ 24 ].Should ( ).Be ( "F11" ) ;
        keys [ 25 ].Should ( ).Be ( "F12" ) ;
    }

    private static AvailableKeysProvider CreateSut ( )
    {
        return new AvailableKeysProvider ( ) ;
    }
}
