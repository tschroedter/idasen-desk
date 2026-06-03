using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils.Validation ;

/// <summary>
///     Validates height settings to ensure they are within safe operating parameters.
/// </summary>
public class HeightSettingsValidator : IHeightSettingsValidator
{
    /// <summary>
    ///     Absolute minimum height for any Idasen desk (in cm).
    /// </summary>
    private const uint AbsoluteMinHeight = 58 ;

    /// <summary>
    ///     Absolute maximum height for any Idasen desk (in cm).
    /// </summary>
    private const uint AbsoluteMaxHeight = 130 ;

    /// <summary>
    ///     Minimum allowed difference between min and max height constraints (in cm).
    /// </summary>
    private const uint MinHeightRange = 10 ;

    /// <summary>
    ///     Maximum allowed length for preset names.
    /// </summary>
    private const int MaxPresetNameLength = 50 ;

    /// <summary>
    ///     Minimum allowed length for preset names.
    /// </summary>
    private const int MinPresetNameLength = 1 ;

    public ValidationResult ValidateHeight ( uint height , uint minHeight , uint maxHeight )
    {
        var errors = new List < string > ( ) ;

        // Check absolute bounds first
        if ( height < AbsoluteMinHeight )
        {
            errors.Add ( $"Height {height} cm is below the absolute minimum of {AbsoluteMinHeight} cm." ) ;
        }

        if ( height > AbsoluteMaxHeight )
        {
            errors.Add ( $"Height {height} cm exceeds the absolute maximum of {AbsoluteMaxHeight} cm." ) ;
        }

        // Check against desk-specific constraints
        if ( height < minHeight )
        {
            errors.Add ( $"Height {height} cm is below the desk minimum of {minHeight} cm." ) ;
        }

        if ( height > maxHeight )
        {
            errors.Add ( $"Height {height} cm exceeds the desk maximum of {maxHeight} cm." ) ;
        }

        return errors.Count == 0
                   ? ValidationResult.Success ( )
                   : ValidationResult.Failure ( errors ) ;
    }

    public ValidationResult ValidateMinMaxConstraints ( uint minHeight , uint maxHeight )
    {
        var errors = new List < string > ( ) ;

        // Check absolute bounds
        if ( minHeight < AbsoluteMinHeight )
        {
            errors.Add ( $"Minimum height {minHeight} cm is below the absolute minimum of {AbsoluteMinHeight} cm." ) ;
        }

        if ( maxHeight > AbsoluteMaxHeight )
        {
            errors.Add ( $"Maximum height {maxHeight} cm exceeds the absolute maximum of {AbsoluteMaxHeight} cm." ) ;
        }

        // Check that min is less than max
        if ( minHeight >= maxHeight )
        {
            errors.Add ( $"Minimum height ({minHeight} cm) must be less than maximum height ({maxHeight} cm)." ) ;
        }

        // Check that the range is reasonable
        if ( maxHeight > minHeight )
        {
            var heightRange = maxHeight - minHeight;

            if ( heightRange < MinHeightRange )
            {
                errors.Add ( $"Height range ({heightRange} cm) is too small. Minimum range is {MinHeightRange} cm." ) ;
            }
        }

        return errors.Count == 0
                   ? ValidationResult.Success ( )
                   : ValidationResult.Failure ( errors ) ;
    }

    public ValidationResult ValidateAllHeights ( uint standing ,
                                                 uint seating ,
                                                 uint custom1 ,
                                                 uint custom2 ,
                                                 uint minHeight ,
                                                 uint maxHeight )
    {
        var errors = new List < string > ( ) ;

        // First validate the constraints themselves
        var constraintsResult = ValidateMinMaxConstraints ( minHeight , maxHeight ) ;
        if ( ! constraintsResult.IsValid )
        {
            errors.AddRange ( constraintsResult.Errors ) ;
            // If constraints are invalid, no point checking individual heights
            return ValidationResult.Failure ( errors ) ;
        }

        // Validate each preset height
        var standingResult = ValidateHeight ( standing , minHeight , maxHeight ) ;
        if ( ! standingResult.IsValid )
        {
            errors.AddRange ( standingResult.Errors.Select ( e => $"Standing: {e}" ) ) ;
        }

        var seatingResult = ValidateHeight ( seating , minHeight , maxHeight ) ;
        if ( ! seatingResult.IsValid )
        {
            errors.AddRange ( seatingResult.Errors.Select ( e => $"Seating: {e}" ) ) ;
        }

        var custom1Result = ValidateHeight ( custom1 , minHeight , maxHeight ) ;
        if ( ! custom1Result.IsValid )
        {
            errors.AddRange ( custom1Result.Errors.Select ( e => $"Custom 1: {e}" ) ) ;
        }

        var custom2Result = ValidateHeight ( custom2 , minHeight , maxHeight ) ;
        if ( ! custom2Result.IsValid )
        {
            errors.AddRange ( custom2Result.Errors.Select ( e => $"Custom 2: {e}" ) ) ;
        }

        // Logical validation: standing should typically be higher than seating
        if ( standing <= seating )
        {
            errors.Add ( $"Warning: Standing height ({standing} cm) should typically be higher than seating height ({seating} cm)." ) ;
        }

        return errors.Count == 0
                   ? ValidationResult.Success ( )
                   : ValidationResult.Failure ( errors ) ;
    }

    public ValidationResult ValidatePresetName ( string name )
    {
        var errors = new List < string > ( ) ;

        if ( string.IsNullOrWhiteSpace ( name ) )
        {
            errors.Add ( "Preset name cannot be empty or whitespace." ) ;
            return ValidationResult.Failure ( errors ) ;
        }

        var trimmedName = name.Trim ( ) ;

        if ( trimmedName.Length < MinPresetNameLength )
        {
            errors.Add ( $"Preset name must be at least {MinPresetNameLength} character long." ) ;
        }

        if ( trimmedName.Length > MaxPresetNameLength )
        {
            errors.Add ( $"Preset name cannot exceed {MaxPresetNameLength} characters." ) ;
        }

        // Check for invalid characters (optional - you can customize this)
        var invalidChars = new [ ] { '<' , '>' , ':' , '"' , '/' , '\\' , '|' , '?' , '*' } ;
        if ( trimmedName.Any ( c => invalidChars.Contains ( c ) ) )
        {
            errors.Add ( $"Preset name contains invalid characters. Avoid: {string.Join ( " " , invalidChars )}" ) ;
        }

        return errors.Count == 0
                   ? ValidationResult.Success ( )
                   : ValidationResult.Failure ( errors ) ;
    }
}
