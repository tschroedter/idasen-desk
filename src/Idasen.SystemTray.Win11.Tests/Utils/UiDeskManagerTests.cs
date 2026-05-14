using System.Reactive.Concurrency ;
using System.Reflection ;
using System.Windows.Input ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class UiDeskManagerTests
{
    private static UiDeskManager CreateSut (
        out IDesk            desk ,
        out ISettingsManager settingsManager ,
        out IDeskProvider    deskProvider )
    {
        var logger = Substitute.For < ILogger > ( ) ;
        settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var iconProvider    = Substitute.For < ITaskbarIconProvider > ( ) ;
        var notifications   = Substitute.For < INotifications > ( ) ;
        var scheduler       = Scheduler.Immediate ;
        var dp              = Substitute.For < IDeskProvider > ( ) ;
        var providerFactory = new Func < IDeskProvider > ( ( ) => dp ) ;
        var errorManager    = Substitute.For < IErrorManager > ( ) ;
        var settingsChanges = Substitute.For < IObserveSettingsChanges > ( ) ;

        deskProvider = dp ;

        var settings = new Settings { HeightSettings = new HeightSettings ( ) } ;
        settingsManager.CurrentSettings.Returns ( settings ) ;

        var sut = new UiDeskManager (
                                     logger ,
                                     settingsManager ,
                                     iconProvider ,
                                     notifications ,
                                     scheduler ,
                                     providerFactory ,
                                     errorManager ,
                                     settingsChanges ) ;

        desk = Substitute.For < IDesk > ( ) ;
        typeof ( UiDeskManager )
           .GetField ( "_desk" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       desk ) ;

        return sut ;
    }

    [ Fact ]
    public async Task StandAsync_MovesDeskToConfiguredStandingHeight ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.StandingHeightInCm = 120 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        await sut.StandAsync ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 120u * 100u ) ;
    }

    [ Fact ]
    public async Task SitAsync_MovesDeskToConfiguredSeatingHeight ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.SeatingHeightInCm = 70 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        await sut.SitAsync ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 70u * 100u ) ;
    }

    [ Fact ]
    public async Task StopAsync_WhenConnected_CallsMoveStop ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ) ;

        await sut.StopAsync ( ) ;

        await desk.Received ( 1 ).MoveStopAsync ( ) ;
    }

    [ Fact ]
    public async Task StopAsync_WhenNotConnected_DoesNothing ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ) ;
        typeof ( UiDeskManager )
           .GetField ( "_desk" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       null ) ;

        await sut.StopAsync ( ) ;

        desk.DidNotReceive ( ).MoveTo ( Arg.Any < uint > ( ) ) ;

        await desk.DidNotReceive ( ).MoveStopAsync ( ) ;
        await desk.DidNotReceive ( ).MoveStopAsync ( ) ;
        await desk.DidNotReceive ( ).MoveUnlockAsync ( ) ;
    }

    [ Fact ]
    public async Task MoveLockAsync_WhenConnected_CallsMoveLock ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ) ;

        await sut.MoveLockAsync ( ) ;

        await desk.Received ( 1 ).MoveLockAsync ( ) ;
    }

    [ Fact ]
    public async Task MoveUnlockAsync_WhenConnected_CallsMoveUnlock ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out _ ) ;

        await sut.MoveUnlockAsync ( ) ;

        await desk.Received ( 1 ).MoveUnlockAsync ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenConnected_DisposesAndClearsDeskAndProvider ( )
    {
        var sut = CreateSut ( out var desk ,
                              out _ ,
                              out var provider ) ;
        // Assign provider field
        typeof ( UiDeskManager )
           .GetField ( "_deskProvider" ,
                       BindingFlags.Instance | BindingFlags.NonPublic )!
           .SetValue ( sut ,
                       provider ) ;

        await sut.DisconnectAsync ( ) ;

        // desk and provider should be disposed
        desk.Received ( 1 ).Dispose ( ) ;
        provider.Received ( 1 ).Dispose ( ) ;

        // and cleared from fields
        var deskField = typeof ( UiDeskManager ).GetField ( "_desk" ,
                                                            BindingFlags.Instance | BindingFlags.NonPublic )! ;
        var providerField = typeof ( UiDeskManager ).GetField ( "_deskProvider" ,
                                                                BindingFlags.Instance | BindingFlags.NonPublic )! ;
        deskField.GetValue ( sut ).Should ( ).BeNull ( ) ;
        providerField.GetValue ( sut ).Should ( ).BeNull ( ) ;
    }

    [ Fact ]
    public async Task Custom1Async_MovesDeskToConfiguredCustom1Height ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom1HeightInCm = 111 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        await sut.Custom1Async ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 111u * 100u ) ;
    }

    [ Fact ]
    public async Task Custom2Async_MovesDeskToConfiguredCustom2Height ( )
    {
        var sut = CreateSut ( out var desk ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HeightSettings.Custom2HeightInCm = 66 ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        await sut.Custom2Async ( ) ;

        await settingsManager.Received ( 1 ).LoadAsync ( Arg.Any < CancellationToken > ( ) ) ;
        desk.Received ( 1 ).MoveTo ( 66u * 100u ) ;
    }

    // Hotkey Configuration Tests

    [ Fact ]
    public void ParseKey_WithValidKeyString_ReturnsCorrectKey ( )
    {
        // Arrange
        var parseKeyMethod = typeof ( UiDeskManager ).GetMethod ( "ParseKey" ,
                                                                  BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( Key )parseKeyMethod !.Invoke ( null ,
                                                      [ "F1" ] ) !;

        // Assert
        result.Should ( ).Be ( Key.F1 ) ;
    }

    [ Theory ]
    [ InlineData ( "Up" , Key.Up ) ]
    [ InlineData ( "Down" , Key.Down ) ]
    [ InlineData ( "Left" , Key.Left ) ]
    [ InlineData ( "Right" , Key.Right ) ]
    [ InlineData ( "F5" , Key.F5 ) ]
    [ InlineData ( "A" , Key.A ) ]
    [ InlineData ( "Space" , Key.Space ) ]
    public void ParseKey_WithVariousKeys_ReturnsCorrectKey ( string keyString , Key expectedKey )
    {
        // Arrange
        var parseKeyMethod = typeof ( UiDeskManager ).GetMethod ( "ParseKey" ,
                                                                  BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( Key )parseKeyMethod !.Invoke ( null ,
                                                      [ keyString ] ) !;

        // Assert
        result.Should ( ).Be ( expectedKey ) ;
    }

    [ Fact ]
    public void ParseKey_WithInvalidKeyString_ThrowsArgumentException ( )
    {
        // Arrange
        var parseKeyMethod = typeof ( UiDeskManager ).GetMethod ( "ParseKey" ,
                                                                  BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var act = ( ) => parseKeyMethod !.Invoke ( null ,
                                                   [ "InvalidKey" ] ) ;

        // Assert
        act.Should ( ).Throw < TargetInvocationException > ( )
           .WithInnerException < ArgumentException > ( )
           .WithMessage ( "Invalid key string: 'InvalidKey'*" ) ;
    }

    [ Fact ]
    public void ParseModifierKeys_WithSingleModifier_ReturnsCorrectModifier ( )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ "Control" ] ) !;

        // Assert
        result.Should ( ).Be ( ModifierKeys.Control ) ;
    }

    [ Theory ]
    [ InlineData ( "Control" , ModifierKeys.Control ) ]
    [ InlineData ( "Alt" , ModifierKeys.Alt ) ]
    [ InlineData ( "Shift" , ModifierKeys.Shift ) ]
    [ InlineData ( "Windows" , ModifierKeys.Windows ) ]
    public void ParseModifierKeys_WithVariousModifiers_ReturnsCorrectModifier ( string modifierString ,
                                                                                ModifierKeys expectedModifier )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ modifierString ] ) !;

        // Assert
        result.Should ( ).Be ( expectedModifier ) ;
    }

    [ Fact ]
    public void ParseModifierKeys_WithMultipleModifiers_ReturnsCombinedModifiers ( )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ "Control, Alt, Shift" ] ) !;

        // Assert
        result.Should ( ).Be ( ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift ) ;
    }

    [ Theory ]
    [ InlineData ( "Control,Alt" , ModifierKeys.Control | ModifierKeys.Alt ) ]
    [ InlineData ( "Control, Shift" , ModifierKeys.Control | ModifierKeys.Shift ) ]
    [ InlineData ( "Alt, Shift, Windows" , ModifierKeys.Alt | ModifierKeys.Shift | ModifierKeys.Windows ) ]
    public void ParseModifierKeys_WithVariousCombinations_ReturnsCombinedModifiers ( string modifierString ,
                                                                                     ModifierKeys expectedModifiers )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ modifierString ] ) !;

        // Assert
        result.Should ( ).Be ( expectedModifiers ) ;
    }

    [ Fact ]
    public void ParseModifierKeys_WithEmptyString_ReturnsNone ( )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ string.Empty ] ) !;

        // Assert
        result.Should ( ).Be ( ModifierKeys.None ) ;
    }

    [ Fact ]
    public void ParseModifierKeys_WithWhitespace_ReturnsNone ( )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ "   " ] ) !;

        // Assert
        result.Should ( ).Be ( ModifierKeys.None ) ;
    }

    [ Fact ]
    public void ParseModifierKeys_WithInvalidModifier_IgnoresInvalidAndReturnsParsedOnes ( )
    {
        // Arrange
        var parseModifierKeysMethod = typeof ( UiDeskManager ).GetMethod ( "ParseModifierKeys" ,
                                                                           BindingFlags.NonPublic | BindingFlags.Static ) ;

        // Act
        var result = ( ModifierKeys )parseModifierKeysMethod !.Invoke ( null ,
                                                                        [ "Control, InvalidModifier, Shift" ] ) !;

        // Assert
        result.Should ( ).Be ( ModifierKeys.Control | ModifierKeys.Shift ) ;
    }

    [ Fact ]
    public void CreateKeyGesture_WithValidInputs_ReturnsKeyGesture ( )
    {
        // Arrange
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;

        var createKeyGestureMethod = typeof ( UiDeskManager ).GetMethod ( "CreateKeyGesture" ,
                                                                          BindingFlags.NonPublic | BindingFlags.Instance ) ;

        var defaultGesture = new KeyGesture ( Key.F12 ,
                                             ModifierKeys.Windows ) ;

        // Act
        var result = ( KeyGesture )createKeyGestureMethod !.Invoke ( sut ,
                                                                     [
                                                                         "F1" ,
                                                                         "Control, Shift" ,
                                                                         defaultGesture
                                                                     ] ) !;

        // Assert
        result.Key.Should ( ).Be ( Key.F1 ) ;
        result.Modifiers.Should ( ).Be ( ModifierKeys.Control | ModifierKeys.Shift ) ;
    }

    [ Fact ]
    public void CreateKeyGesture_WithInvalidKey_ReturnsDefaultGesture ( )
    {
        // Arrange
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;

        var createKeyGestureMethod = typeof ( UiDeskManager ).GetMethod ( "CreateKeyGesture" ,
                                                                          BindingFlags.NonPublic | BindingFlags.Instance ) ;

        var defaultGesture = new KeyGesture ( Key.F12 ,
                                             ModifierKeys.Windows ) ;

        // Act
        var result = ( KeyGesture )createKeyGestureMethod !.Invoke ( sut ,
                                                                     [
                                                                         "InvalidKey" ,
                                                                         "Control" ,
                                                                         defaultGesture
                                                                     ] ) !;

        // Assert
        result.Key.Should ( ).Be ( Key.F12 ) ;
        result.Modifiers.Should ( ).Be ( ModifierKeys.Windows ) ;
    }

    [ Fact ]
    public void HotkeySettings_DefaultConfiguration_EnablesHotkeys ( )
    {
        // Arrange & Act
        var settings = new Settings ( ) ;

        // Assert
        settings.HotkeySettings.GlobalHotkeysEnabled.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HotkeySettings_CanBeDisabled_ViaConfiguration ( )
    {
        // Arrange
        var settings = new Settings
        {
            HotkeySettings = new HotkeySettings
            {
                GlobalHotkeysEnabled = false
            }
        } ;

        // Act & Assert
        settings.HotkeySettings.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void HotkeySettings_CustomKeys_CanBeConfigured ( )
    {
        // Arrange
        var settings = new Settings
        {
            HotkeySettings = new HotkeySettings
            {
                StandingKey       = "F9" ,
                StandingModifiers = "Control, Alt" ,
                SeatingKey        = "F10" ,
                SeatingModifiers  = "Control, Alt"
            }
        } ;

        // Act & Assert
        using var scope = new FluentAssertions.Execution.AssertionScope ( ) ;
        settings.HotkeySettings.StandingKey.Should ( ).Be ( "F9" ) ;
        settings.HotkeySettings.StandingModifiers.Should ( ).Be ( "Control, Alt" ) ;
        settings.HotkeySettings.SeatingKey.Should ( ).Be ( "F10" ) ;
        settings.HotkeySettings.SeatingModifiers.Should ( ).Be ( "Control, Alt" ) ;
    }

    [ Fact ]
    public void SafeAddOrReplaceHotkey_ShouldHandleNonExistentHotkey ( )
    {
        // Arrange
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = true } ;

        // Get the private method via reflection
        var method = typeof ( UiDeskManager ).GetMethod ( "SafeAddOrReplaceHotkey" ,
                                                          BindingFlags.NonPublic | BindingFlags.Instance ) ;

        // Act - This should not throw even if the hotkey doesn't exist
        var gesture = new KeyGesture ( Key.F1 , ModifierKeys.Control ) ;
        var act = ( ) => method?.Invoke ( sut , [ "TestHotkey" , gesture , null ] ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void RegisterGlobalHotkeys_ShouldRegisterAllFourHotkeys ( )
    {
        // Arrange
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = "Up" ,
            StandingModifiers    = "Control, Alt, Shift" ,
            SeatingKey           = "Down" ,
            SeatingModifiers     = "Control, Alt, Shift" ,
            Custom1Key           = "Left" ,
            Custom1Modifiers     = "Control, Alt, Shift" ,
            Custom2Key           = "Right" ,
            Custom2Modifiers     = "Control, Alt, Shift"
        } ;

        // Get the private method via reflection
        var method = typeof ( UiDeskManager ).GetMethod ( "RegisterGlobalHotkeys" ,
                                                          BindingFlags.NonPublic | BindingFlags.Instance ) ;

        // Act - This should not throw
        var act = ( ) => method?.Invoke ( sut , null ) ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void UnregisterGlobalHotkeys_ShouldThrowTargetInvocationException_WhenHotkeysNotRegistered ( )
    {
        // Arrange
        var sut = CreateSut ( out _ ,
                              out var settingsManager ,
                              out _ ) ;
        var settings = ( Settings )settingsManager.CurrentSettings ;
        settings.HotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = false } ;

        // Get the private method via reflection
        var method = typeof ( UiDeskManager ).GetMethod ( "UnregisterGlobalHotkeys" ,
                                                          BindingFlags.NonPublic | BindingFlags.Instance ) ;

        // Act
        var act = ( ) => method?.Invoke ( sut , null ) ;

        // Assert
        act.Should ( ).Throw < TargetInvocationException > ( ) ;
    }

    [ Fact ]
    public void HotkeySettings_DefaultsToEnabled ( )
    {
        // Arrange & Act
        var settings = new HotkeySettings ( ) ;

        // Assert
        settings.GlobalHotkeysEnabled.Should ( ).BeTrue ( ) ;
    }
}
