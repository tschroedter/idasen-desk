using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Validation ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Validation ;

public class HeightSettingsValidatorTests
{
    private readonly HeightSettingsValidator _validator = new ( ) ;

    [ Fact ]
    public void ValidateHeight_WithValidHeight_ReturnsSuccess ( )
    {
        // Arrange
        const uint height    = 100 ;
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateHeight ( height , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeTrue ( ) ;
        result.Errors.Should ( ).BeEmpty ( ) ;
    }

    [ Fact ]
    public void ValidateHeight_BelowMinimum_ReturnsFailure ( )
    {
        // Arrange
        const uint height    = 50 ;
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateHeight ( height , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "below" ) ) ;
    }

    [ Fact ]
    public void ValidateHeight_AboveMaximum_ReturnsFailure ( )
    {
        // Arrange
        const uint height    = 135 ;
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateHeight ( height , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "exceeds" ) ) ;
    }

    [ Fact ]
    public void ValidateHeight_BelowAbsoluteMinimum_ReturnsFailure ( )
    {
        // Arrange
        const uint height    = 55 ; // Below absolute minimum of 58
        const uint minHeight = 55 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateHeight ( height , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "absolute minimum" ) ) ;
    }

    [ Fact ]
    public void ValidateHeight_AboveAbsoluteMaximum_ReturnsFailure ( )
    {
        // Arrange
        const uint height    = 135 ; // Above absolute maximum of 130
        const uint minHeight = 60 ;
        const uint maxHeight = 135 ;

        // Act
        var result = _validator.ValidateHeight ( height , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "absolute maximum" ) ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_WithValidRange_ReturnsSuccess ( )
    {
        // Arrange
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeTrue ( ) ;
        result.Errors.Should ( ).BeEmpty ( ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_MinGreaterThanMax_ReturnsFailure ( )
    {
        // Arrange
        const uint minHeight = 127 ;
        const uint maxHeight = 60 ;

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "must be less than" ) ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_MinEqualToMax_ReturnsFailure ( )
    {
        // Arrange
        const uint minHeight = 100 ;
        const uint maxHeight = 100 ;

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "must be less than" ) ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_RangeTooSmall_ReturnsFailure ( )
    {
        // Arrange
        const uint minHeight = 100 ;
        const uint maxHeight = 105 ; // Only 5 cm range, minimum is 10 cm

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "too small" ) ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_MinBelowAbsoluteMinimum_ReturnsFailure ( )
    {
        // Arrange
        const uint minHeight = 50 ; // Below absolute minimum of 58
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "absolute minimum" ) ) ;
    }

    [ Fact ]
    public void ValidateMinMaxConstraints_MaxAboveAbsoluteMaximum_ReturnsFailure ( )
    {
        // Arrange
        const uint minHeight = 60 ;
        const uint maxHeight = 135 ; // Above absolute maximum of 130

        // Act
        var result = _validator.ValidateMinMaxConstraints ( minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "absolute maximum" ) ) ;
    }

    [ Fact ]
    public void ValidateAllHeights_WithValidSettings_ReturnsSuccess ( )
    {
        // Arrange
        const uint standing  = 120 ;
        const uint seating   = 75 ;
        const uint custom1   = 100 ;
        const uint custom2   = 85 ;
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateAllHeights ( standing , seating , custom1 , custom2 , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeTrue ( ) ;
        result.Errors.Should ( ).BeEmpty ( ) ;
    }

    [ Fact ]
    public void ValidateAllHeights_StandingLowerThanSeating_ReturnsWarning ( )
    {
        // Arrange
        const uint standing  = 75 ;  // Lower than seating
        const uint seating   = 120 ;
        const uint custom1   = 100 ;
        const uint custom2   = 85 ;
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateAllHeights ( standing , seating , custom1 , custom2 , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "Warning" ) && e.Contains ( "typically be higher" ) ) ;
    }

    [ Fact ]
    public void ValidateAllHeights_InvalidConstraints_ReturnsMultipleErrors ( )
    {
        // Arrange
        const uint standing  = 120 ;
        const uint seating   = 75 ;
        const uint custom1   = 150 ; // Above max
        const uint custom2   = 50 ;  // Below min
        const uint minHeight = 60 ;
        const uint maxHeight = 127 ;

        // Act
        var result = _validator.ValidateAllHeights ( standing , seating , custom1 , custom2 , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).HaveCountGreaterThan ( 1 ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "Custom 1" ) ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "Custom 2" ) ) ;
    }

    [ Fact ]
    public void ValidateAllHeights_InvalidMinMaxConstraints_ReturnsFailureImmediately ( )
    {
        // Arrange
        const uint standing  = 100 ;
        const uint seating   = 75 ;
        const uint custom1   = 100 ;
        const uint custom2   = 85 ;
        const uint minHeight = 127 ; // Min greater than max
        const uint maxHeight = 60 ;

        // Act
        var result = _validator.ValidateAllHeights ( standing , seating , custom1 , custom2 , minHeight , maxHeight ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "must be less than" ) ) ;
    }

    [ Fact ]
    public void ValidatePresetName_WithValidName_ReturnsSuccess ( )
    {
        // Arrange
        const string name = "My Custom Preset" ;

        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeTrue ( ) ;
        result.Errors.Should ( ).BeEmpty ( ) ;
    }

    [ Fact ]
    public void ValidatePresetName_EmptyString_ReturnsFailure ( )
    {
        // Arrange
        const string name = "" ;

        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "cannot be empty" ) ) ;
    }

    [ Fact ]
    public void ValidatePresetName_WhitespaceOnly_ReturnsFailure ( )
    {
        // Arrange
        const string name = "   " ;

        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "cannot be empty" ) ) ;
    }

    [ Fact ]
    public void ValidatePresetName_TooLong_ReturnsFailure ( )
    {
        // Arrange
        var name = new string ( 'A' , 51 ) ; // 51 characters, max is 50

        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "cannot exceed" ) ) ;
    }

    [ Theory ]
    [ InlineData ( "<test>" ) ]
    [ InlineData ( "test>file" ) ]
    [ InlineData ( "test:name" ) ]
    [ InlineData ( "test\"quote" ) ]
    [ InlineData ( "test/slash" ) ]
    [ InlineData ( "test\\backslash" ) ]
    [ InlineData ( "test|pipe" ) ]
    [ InlineData ( "test?question" ) ]
    [ InlineData ( "test*star" ) ]
    public void ValidatePresetName_WithInvalidCharacters_ReturnsFailure ( string name )
    {
        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeFalse ( ) ;
        result.Errors.Should ( ).Contain ( e => e.Contains ( "invalid characters" ) ) ;
    }

    [ Theory ]
    [ InlineData ( "Standing" ) ]
    [ InlineData ( "My Desk" ) ]
    [ InlineData ( "Preset 123" ) ]
    [ InlineData ( "Work-From-Home" ) ]
    [ InlineData ( "Focus_Mode" ) ]
    public void ValidatePresetName_WithValidNames_ReturnsSuccess ( string name )
    {
        // Act
        var result = _validator.ValidatePresetName ( name ) ;

        // Assert
        result.IsValid.Should ( ).BeTrue ( ) ;
        result.Errors.Should ( ).BeEmpty ( ) ;
    }
}
