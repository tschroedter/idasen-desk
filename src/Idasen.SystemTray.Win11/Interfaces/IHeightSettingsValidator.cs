namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Validates height settings to ensure they are within safe operating parameters.
/// </summary>
public interface IHeightSettingsValidator
{
    /// <summary>
    ///     Validates a height value against desk min/max constraints.
    /// </summary>
    /// <param name="height">The height to validate in centimeters.</param>
    /// <param name="minHeight">The minimum allowed height in centimeters.</param>
    /// <param name="maxHeight">The maximum allowed height in centimeters.</param>
    /// <returns>A validation result containing success status and error messages.</returns>
    ValidationResult ValidateHeight ( uint height , uint minHeight , uint maxHeight ) ;

    /// <summary>
    ///     Validates that min and max height constraints are valid.
    /// </summary>
    /// <param name="minHeight">The minimum height in centimeters.</param>
    /// <param name="maxHeight">The maximum height in centimeters.</param>
    /// <returns>A validation result containing success status and error messages.</returns>
    ValidationResult ValidateMinMaxConstraints ( uint minHeight , uint maxHeight ) ;

    /// <summary>
    ///     Validates all height settings together for consistency.
    /// </summary>
    /// <param name="standing">Standing height in centimeters.</param>
    /// <param name="seating">Seating height in centimeters.</param>
    /// <param name="custom1">Custom 1 height in centimeters.</param>
    /// <param name="custom2">Custom 2 height in centimeters.</param>
    /// <param name="minHeight">Minimum desk height in centimeters.</param>
    /// <param name="maxHeight">Maximum desk height in centimeters.</param>
    /// <returns>A validation result containing success status and error messages.</returns>
    ValidationResult ValidateAllHeights ( uint standing ,
                                          uint seating ,
                                          uint custom1 ,
                                          uint custom2 ,
                                          uint minHeight ,
                                          uint maxHeight ) ;

    /// <summary>
    ///     Validates a preset name to ensure it meets requirements.
    /// </summary>
    /// <param name="name">The preset name to validate.</param>
    /// <returns>A validation result containing success status and error messages.</returns>
    ValidationResult ValidatePresetName ( string name ) ;
}

/// <summary>
///     Represents the result of a validation operation.
/// </summary>
public class ValidationResult
{
    /// <summary>
    ///     Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get ; init ; }

    /// <summary>
    ///     Gets the list of error messages if validation failed.
    /// </summary>
    public IReadOnlyList < string > Errors { get ; init ; } = [] ;

    /// <summary>
    ///     Creates a successful validation result.
    /// </summary>
    public static ValidationResult Success ( )
    {
        return new ValidationResult { IsValid = true } ;
    }

    /// <summary>
    ///     Creates a failed validation result with error messages.
    /// </summary>
    public static ValidationResult Failure ( params string [ ] errors )
    {
        return new ValidationResult { IsValid = false , Errors = errors } ;
    }

    /// <summary>
    ///     Creates a failed validation result with a list of error messages.
    /// </summary>
    public static ValidationResult Failure ( IReadOnlyList < string > errors )
    {
        return new ValidationResult { IsValid = false , Errors = errors } ;
    }
}
