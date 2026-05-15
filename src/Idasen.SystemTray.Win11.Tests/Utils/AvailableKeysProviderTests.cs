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
        keys.Should ( ).Contain ( "D1" ) ;
        keys.Should ( ).Contain ( "D2" ) ;
        keys.Should ( ).Contain ( "D3" ) ;
        keys.Should ( ).Contain ( "D4" ) ;
        keys.Should ( ).Contain ( "D5" ) ;
        keys.Should ( ).Contain ( "D6" ) ;
        keys.Should ( ).Contain ( "D7" ) ;
        keys.Should ( ).Contain ( "D8" ) ;
        keys.Should ( ).Contain ( "D9" ) ;
        keys.Should ( ).Contain ( "D0" ) ;
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
        // 4 arrow keys + 10 number keys + 12 function keys = 26 total
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
        keys [ 4 ].Should ( ).Be ( "D1" ) ;
        keys [ 5 ].Should ( ).Be ( "D2" ) ;
        keys [ 6 ].Should ( ).Be ( "D3" ) ;
        keys [ 7 ].Should ( ).Be ( "D4" ) ;
        keys [ 8 ].Should ( ).Be ( "D5" ) ;
        keys [ 9 ].Should ( ).Be ( "D6" ) ;
        keys [ 10 ].Should ( ).Be ( "D7" ) ;
        keys [ 11 ].Should ( ).Be ( "D8" ) ;
        keys [ 12 ].Should ( ).Be ( "D9" ) ;
        keys [ 13 ].Should ( ).Be ( "D0" ) ;
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
