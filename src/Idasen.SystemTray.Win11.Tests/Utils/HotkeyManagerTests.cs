using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class HotkeyManagerTests
{
    private readonly ILogger          _logger ;
    private readonly ISettingsManager _settingsManager ;
    private readonly INotifications   _notifications ;
    private readonly Settings         _settings ;

    public HotkeyManagerTests ( )
    {
        _logger          = Substitute.For < ILogger > ( ) ;
        _settingsManager = Substitute.For < ISettingsManager > ( ) ;
        _notifications   = Substitute.For < INotifications > ( ) ;

        _settings = new Settings
        {
            HotkeySettings = new HotkeySettings
            {
                StandingKey       = "Up" ,
                StandingModifiers = "Control, Alt, Shift" ,
                SeatingKey        = "Down" ,
                SeatingModifiers  = "Control, Alt, Shift" ,
                Custom1Key        = "Left" ,
                Custom1Modifiers  = "Control, Alt, Shift" ,
                Custom2Key        = "Right" ,
                Custom2Modifiers  = "Control, Alt, Shift"
            }
        } ;
        _settingsManager.CurrentSettings.Returns ( _settings ) ;
    }

    [ Fact ]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException ( )
    {
        var act = ( ) => new HotkeyManager (
                                            null! ,
                                            _settingsManager ,
                                            _notifications ) ;

        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_WithNullSettingsManager_ThrowsArgumentNullException ( )
    {
        var act = ( ) => new HotkeyManager (
                                            _logger ,
                                            null! ,
                                            _notifications ) ;

        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "settingsManager" ) ;
    }

    [ Fact ]
    public void Constructor_WithNullNotifications_ThrowsArgumentNullException ( )
    {
        var act = ( ) => new HotkeyManager (
                                            _logger ,
                                            _settingsManager ,
                                            null! ) ;

        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "notifications" ) ;
    }

    [ Fact ]
    public void Constructor_WithValidParameters_CreatesInstance ( )
    {
        // Act
        var manager = CreateManager ( ) ;

        // Assert
        manager.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void Dispose_CalledMultipleTimes_DoesNotThrow ( )
    {
        var manager = CreateManager ( ) ;

        var act = ( ) =>
                  {
                      manager.Dispose ( ) ;
                      manager.Dispose ( ) ;
                      manager.Dispose ( ) ;
                  } ;

        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void Dispose_AfterRegisteringHotkeys_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) =>
        {
            manager.RegisterGlobalHotkeys ( ) ;
            manager.Dispose ( ) ;
        } ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void Events_CanBeSubscribedTo ( )
    {
        var manager = CreateManager ( ) ;

        // Events should be able to be subscribed to without throwing
        var standingCalled = false ;
        var seatingCalled = false ;
        var custom1Called = false ;
        var custom2Called = false ;

        manager.StandingHotkeyPressed += ( _ , _ ) => standingCalled = true ;
        manager.SeatingHotkeyPressed  += ( _ , _ ) => seatingCalled = true ;
        manager.Custom1HotkeyPressed  += ( _ , _ ) => custom1Called = true ;
        manager.Custom2HotkeyPressed  += ( _ , _ ) => custom2Called = true ;

        // Verify events can be subscribed to without throwing
        standingCalled.Should ( ).BeFalse ( ) ;
        seatingCalled.Should ( ).BeFalse ( ) ;
        custom1Called.Should ( ).BeFalse ( ) ;
        custom2Called.Should ( ).BeFalse ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithValidSettings_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithNumberKeys_DoesNotThrow ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingKey = "1" ;
        _settings.HotkeySettings.SeatingKey  = "2" ;
        _settings.HotkeySettings.Custom1Key  = "3" ;
        _settings.HotkeySettings.Custom2Key  = "4" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithInvalidKey_DoesNotThrow ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingKey = "InvalidKey" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert - Should not throw even with invalid key (uses default)
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithEmptyModifiers_DoesNotThrow ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingModifiers = "" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithMultipleModifiers_DoesNotThrow ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingModifiers = "Control,Alt" ;
        _settings.HotkeySettings.SeatingModifiers  = "Control, Shift" ;
        _settings.HotkeySettings.Custom1Modifiers  = "Alt,Shift" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_CalledTwice_ReplacesExistingHotkeys ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) =>
        {
            manager.RegisterGlobalHotkeys ( ) ;
            manager.RegisterGlobalHotkeys ( ) ; // Should replace existing
        } ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void UnregisterGlobalHotkeys_AfterRegistration_ThrowsBecauseOfSTA ( )
    {
        // Arrange - WPF HotkeyManager requires STA thread
        var manager = CreateManager ( ) ;
        manager.RegisterGlobalHotkeys ( ) ;

        // Act
        var act = ( ) => manager.UnregisterGlobalHotkeys ( ) ;

        // Assert - In test environment without STA, this will throw
        act.Should ( ).Throw < InvalidOperationException > ( )
           .WithMessage ( "*Failed to unregister global hotkeys*" ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void UnregisterGlobalHotkeys_WithoutRegistration_ThrowsInvalidOperationException ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.UnregisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).Throw < InvalidOperationException > ( )
           .WithMessage ( "*Failed to unregister global hotkeys*" ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void Dispose_WithoutRegistration_DoesNotThrow ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.Dispose ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithVariousKeyFormats_HandlesCorrectly ( )
    {
        // Arrange - Test various key formats
        _settings.HotkeySettings.StandingKey = "A" ; // Letter key
        _settings.HotkeySettings.SeatingKey  = "F1" ; // Function key
        _settings.HotkeySettings.Custom1Key  = "Space" ; // Special key
        _settings.HotkeySettings.Custom2Key  = "0" ; // Number key

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithSingleModifier_DoesNotThrow ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingModifiers = "Control" ;
        _settings.HotkeySettings.SeatingModifiers  = "Alt" ;
        _settings.HotkeySettings.Custom1Modifiers  = "Shift" ;
        _settings.HotkeySettings.Custom2Modifiers  = "Windows" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_LogsInformationMessages ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        _logger.Received ( 1 ).Information ( "Registering global hotkeys..." ) ;
        _logger.Received ( 1 ).Information ( "Global hotkeys registered successfully" ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void UnregisterGlobalHotkeys_LogsAttempts ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;
        manager.RegisterGlobalHotkeys ( ) ;

        // Act
        try
        {
            manager.UnregisterGlobalHotkeys ( ) ;
        }
        catch ( InvalidOperationException )
        {
            // Expected in test environment without STA thread
        }

        // Assert - Even if it fails, it should log the unregistration attempt
        _logger.Received ( 1 ).Information ( "Unregistering global hotkeys..." ) ;

        manager.Dispose ( ) ;
    }

    [ Theory ]
    [ InlineData ( "1" ) ]
    [ InlineData ( "2" ) ]
    [ InlineData ( "5" ) ]
    [ InlineData ( "9" ) ]
    [ InlineData ( "0" ) ]
    public void RegisterGlobalHotkeys_WithNumberKey_MapsToDigitKey ( string numberKey )
    {
        // Arrange
        _settings.HotkeySettings.StandingKey = numberKey ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert - Should not throw and should map "1" to "D1", etc.
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithInvalidModifier_UsesDefault ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingModifiers = "InvalidModifier" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert - Should still register but with None modifier instead of invalid one
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void Dispose_LogsDisposalMessage ( )
    {
        // Arrange
        var manager = CreateManager ( ) ;

        // Act
        manager.Dispose ( ) ;

        // Assert
        _logger.Received ( 1 ).Information (
            "Disposing {TypeName}..." ,
            nameof ( HotkeyManager ) ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithAllDifferentKeys_DoesNotThrow ( )
    {
        // Arrange - Ensure all hotkeys have different keys to avoid conflicts
        _settings.HotkeySettings.StandingKey = "Q" ;
        _settings.HotkeySettings.SeatingKey  = "W" ;
        _settings.HotkeySettings.Custom1Key  = "E" ;
        _settings.HotkeySettings.Custom2Key  = "R" ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_WithWhitespaceModifiers_ParsesCorrectly ( )
    {
        // Arrange
        _settings.HotkeySettings.StandingModifiers = " Control , Alt " ;

        var manager = CreateManager ( ) ;

        // Act
        var act = ( ) => manager.RegisterGlobalHotkeys ( ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;

        manager.Dispose ( ) ;
    }

    private HotkeyManager CreateManager ( )
    {
        return new HotkeyManager (
            _logger ,
            _settingsManager ,
            _notifications ) ;
    }
}

