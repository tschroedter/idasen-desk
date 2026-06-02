using System.Reactive.Subjects ;
using System.Reflection ;
using System.Windows ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Validation ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Microsoft.Reactive.Testing ;
using NSubstitute ;
using Serilog ;
using Wpf.Ui.Appearance ;

#pragma warning disable CA2012 // Use ValueTasks correctly - disabled for test mocking

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Pages ;

public sealed class SettingsViewModelTests
    : IDisposable
{
    private readonly ILogger                   _logger               = Substitute.For < ILogger > ( ) ;
    private readonly IMainWindow               _mainWindow           = Substitute.For < IMainWindow > ( ) ;
    private readonly TestScheduler             _scheduler            = new( ) ;
    private readonly ILoggingSettingsManager   _settingsManager      = Substitute.For < ILoggingSettingsManager > ( ) ;
    private readonly IHeightSettingsValidator  _heightValidator      = new HeightSettingsValidator ( ) ;

    private readonly Subject < ISettings >     _settingsSaved        = new( ) ;
    private readonly ISettingsSynchronizer     _synchronizer         = Substitute.For < ISettingsSynchronizer > ( ) ;
    private readonly IApplicationThemeManager  _themeManager         = Substitute.For < IApplicationThemeManager > ( ) ;
    private readonly IAvailableKeysProvider    _availableKeysProvider = Substitute.For < IAvailableKeysProvider > ( ) ;
    private readonly Subject < Visibility >    _visibilityChanges    = new( ) ;

    private bool _disposed ;

    public SettingsViewModelTests ( )
    {
        _settingsManager.SettingsFileName.Returns ( "TestSettings.json" ) ;
        _settingsManager.SettingsSaved.Returns ( _settingsSaved ) ;

        // Visibility stream for the main window
        _mainWindow.VisibilityChanged.Returns ( _visibilityChanges ) ;

        // Mock available keys
        _availableKeysProvider.AvailableKeys.Returns ( new List < string >
        {
            "Up" , "Down" , "Left" , "Right" ,
            "F1" , "F2" , "F3" , "F4" , "F5" , "F6" ,
            "F7" , "F8" , "F9" , "F10" , "F11" , "F12"
        } ) ;

        // Default synchronizer behaviors
        _synchronizer.LoadSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                          Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;
        _synchronizer.StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                           Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.CompletedTask ) ;

        // Default theme manager behavior
        _themeManager.ApplyAsync ( Arg.Any < ApplicationTheme > ( ) ).Returns ( Task.CompletedTask ) ;
    }

    public void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    ~SettingsViewModelTests ( )
    {
        Dispose ( false ) ;
    }

    private void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing )
        {
            _settingsSaved.Dispose ( ) ;
            _visibilityChanges.Dispose ( ) ;
        }

        _disposed = true ;
    }

    [ Fact ]
    public async Task InitializeAsync_ShouldLoadSettings_And_SubscribeToSettingsSaved ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .LoadSettingsAsync ( vm ,
                                                CancellationToken.None ) ;

        // The ViewModel applies theme on the UI thread by invoking the theme manager
        await _themeManager.Received ( 1 ).ApplyAsync ( ApplicationTheme.Unknown ) ;

        // simulate settings saved event and verify ViewModel updates
        var s = new Settings { HeightSettings = { LastKnownDeskHeight = 150 } } ;
        _settingsSaved.OnNext ( s ) ;

        // Pump scheduled work (ObserveOn(TestScheduler)) so the subscription runs
        _scheduler.Start ( ) ;

        vm.LastKnownDeskHeight.Should ( ).Be ( 150 ) ;
    }

    [ Fact ]
    public async Task OnNavigatedFromAsync_ShouldStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        await vm.OnNavigatedFromAsync ( ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 CancellationToken.None ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldStore_OnSinglePropertyChange_AfterDebounce ( )
    {
        using var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Notifications = ! vm.Notifications ;

        // Advance virtual time beyond debounce (300ms)
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldCoalesce_MultipleRapidChanges ( )
    {
        using var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Notifications = ! vm.Notifications ;
        // within debounce window, change again
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.Notifications = ! vm.Notifications ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.ParentalLock = ! vm.ParentalLock ;

        // Now advance past debounce window from last change
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task AutoSave_ShouldNotRun_AfterDispose ( )
    {
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;
        vm.Dispose ( ) ;

        vm.Notifications = ! vm.Notifications ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ResetSettings_ShouldCallManagerAndReload ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        _settingsManager.ResetSettingsAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;
        _synchronizer.LoadSettingsAsync ( vm ,
                                          Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        // Act: invoke the internal async method via reflection
        var method = typeof ( SettingsViewModel ).GetMethod ( "OnResetSettings" ,
                                                              BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;
        var task = ( Task ? )method.Invoke ( vm ,
                                             null ) ;
        if ( task != null ) await task ;

        // Assert
        await _settingsManager.Received ( 1 ).ResetSettingsAsync ( Arg.Any < CancellationToken > ( ) ) ;
        await _synchronizer.Received ( 1 ).LoadSettingsAsync ( vm ,
                                                               Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_Hidden_ShouldStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Hidden ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_Visible_ShouldNotStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Visible ) ;

        // Assert
        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task SubscribeToMainWindowVisibility_ShouldNotDuplicate_Subscription ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Try to invoke subscription again via reflection
        var method = typeof ( SettingsViewModel ).GetMethod ( "SubscribeToMainWindowVisibility" ,
                                                              BindingFlags.Instance | BindingFlags.NonPublic ) ;
        method.Should ( ).NotBeNull ( ) ;
        method.Invoke ( vm ,
                        null ) ;

        // Act: push a single non-visible event
        _visibilityChanges.OnNext ( Visibility.Collapsed ) ;

        // Assert: should only store once (not twice)
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Visibility_AfterDispose_ShouldNotStoreSettings ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        vm.Dispose ( ) ;

        // Act
        _visibilityChanges.OnNext ( Visibility.Hidden ) ;

        // Assert
        await _synchronizer.DidNotReceive ( )
                           .StoreSettingsAsync ( Arg.Any < ISettingsViewModel > ( ) ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task InitializeAsync_AppliesCurrentTheme_WithThemeManager ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        // Set a non-default theme before initialization
        vm.CurrentTheme = ApplicationTheme.Dark ;

        // Act
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Assert - theme manager should be asked to apply the current theme
        await _themeManager.Received ( 1 ).ApplyAsync ( ApplicationTheme.Dark ) ;
    }

    [ Fact ]
    public void GlobalHotkeysEnabled_DefaultsToTrue ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.GlobalHotkeysEnabled.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void GlobalHotkeysEnabled_CanBeSetToFalse ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.GlobalHotkeysEnabled = false ;

        // Assert
        vm.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task GlobalHotkeysEnabled_ChangeTriggersAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act - change the property
        vm.GlobalHotkeysEnabled = false ;

        // Advance scheduler by debounce time (300ms + buffer)
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public void GlobalHotkeysEnabled_ImplementsISettingsViewModel ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act & Assert - Verify the property works through the interface
        ( ( ISettingsViewModel )vm ).GlobalHotkeysEnabled = false ;

        vm.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldReturnProviderKeys ( )
    {
        // Arrange
        var expectedKeys = new List < string > { "F1" , "F2" , "Up" , "Down" } ;
        _availableKeysProvider.AvailableKeys.Returns ( expectedKeys ) ;
        var vm = CreateSut ( ) ;

        // Act
        var actualKeys = vm.AvailableKeys ;

        // Assert
        actualKeys.Should ( ).BeSameAs ( expectedKeys ) ;
    }

    [ Fact ]
    public void AvailableKeys_ShouldNotBeNull ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        var keys = vm.AvailableKeys ;

        // Assert
        keys.Should ( ).NotBeNull ( ) ;
    }

    [ Fact ]
    public void HotkeyKeys_DefaultValues_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.StandingKey.Should ( ).Be ( Constants.DefaultStandingKey ) ;
        vm.SeatingKey.Should ( ).Be ( Constants.DefaultSeatingKey ) ;
        vm.Custom1Key.Should ( ).Be ( Constants.DefaultCustom1Key ) ;
        vm.Custom2Key.Should ( ).Be ( Constants.DefaultCustom2Key ) ;
    }

    [ Fact ]
    public void HotkeyModifiers_DefaultValues_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.StandingModifiers.Should ( ).Be ( Constants.DefaultHotkeyModifiers ) ;
        vm.SeatingModifiers.Should ( ).Be ( Constants.DefaultHotkeyModifiers ) ;
        vm.Custom1Modifiers.Should ( ).Be ( Constants.DefaultHotkeyModifiers ) ;
        vm.Custom2Modifiers.Should ( ).Be ( Constants.DefaultHotkeyModifiers ) ;
    }

    [ Fact ]
    public void StandingKey_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.StandingKey = "F5" ;

        // Assert
        vm.StandingKey.Should ( ).Be ( "F5" ) ;
    }

    [ Fact ]
    public void StandingModifiers_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.StandingModifiers = "Control, Shift" ;

        // Assert
        vm.StandingModifiers.Should ( ).Be ( "Control, Shift" ) ;
    }

    [ Fact ]
    public void StandingControl_ShouldReturnTrue_WhenModifiersContainControl ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control, Alt" ;

        // Act
        var hasControl = vm.StandingControl ;

        // Assert
        hasControl.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StandingControl_ShouldReturnFalse_WhenModifiersDoNotContainControl ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Alt, Shift" ;

        // Act
        var hasControl = vm.StandingControl ;

        // Assert
        hasControl.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void StandingControl_Set_ShouldAddControl_WhenNotPresent ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Alt" ;

        // Act
        vm.StandingControl = true ;

        // Assert
        vm.StandingModifiers.Should ( ).Contain ( "Control" ) ;
    }

    [ Fact ]
    public void StandingControl_Set_ShouldRemoveControl_WhenPresent ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control, Alt" ;

        // Act
        vm.StandingControl = false ;

        // Assert
        vm.StandingModifiers.Should ( ).NotContain ( "Control" ) ;
    }

    [ Fact ]
    public void StandingAlt_ShouldReturnTrue_WhenModifiersContainAlt ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control, Alt" ;

        // Act
        var hasAlt = vm.StandingAlt ;

        // Assert
        hasAlt.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StandingAlt_Set_ShouldAddAlt_WhenNotPresent ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control" ;

        // Act
        vm.StandingAlt = true ;

        // Assert
        vm.StandingModifiers.Should ( ).Contain ( "Alt" ) ;
    }

    [ Fact ]
    public void StandingShift_ShouldReturnTrue_WhenModifiersContainShift ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Shift" ;

        // Act
        var hasShift = vm.StandingShift ;

        // Assert
        hasShift.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StandingShift_Set_ShouldAddShift_WhenNotPresent ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = string.Empty ;

        // Act
        vm.StandingShift = true ;

        // Assert
        vm.StandingModifiers.Should ( ).Be ( "Shift" ) ;
    }

    [ Fact ]
    public void SeatingControl_ShouldReturnTrue_WhenModifiersContainControl ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.SeatingModifiers = "Control" ;

        // Act
        var hasControl = vm.SeatingControl ;

        // Assert
        hasControl.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void SeatingControl_Set_ShouldUpdateSeatingModifiers ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.SeatingModifiers = string.Empty ;

        // Act
        vm.SeatingControl = true ;

        // Assert
        vm.SeatingModifiers.Should ( ).Contain ( "Control" ) ;
    }

    [ Fact ]
    public void Custom1Control_Set_ShouldUpdateCustom1Modifiers ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.Custom1Modifiers = string.Empty ;

        // Act
        vm.Custom1Control = true ;

        // Assert
        vm.Custom1Modifiers.Should ( ).Contain ( "Control" ) ;
    }

    [ Fact ]
    public void Custom1Alt_Set_ShouldUpdateCustom1Modifiers ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.Custom1Modifiers = string.Empty ;

        // Act
        vm.Custom1Alt = true ;

        // Assert
        vm.Custom1Modifiers.Should ( ).Contain ( "Alt" ) ;
    }

    [ Fact ]
    public void Custom2Shift_Set_ShouldUpdateCustom2Modifiers ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.Custom2Modifiers = string.Empty ;

        // Act
        vm.Custom2Shift = true ;

        // Assert
        vm.Custom2Modifiers.Should ( ).Contain ( "Shift" ) ;
    }

    [ Fact ]
    public void ModifierProperties_CaseInsensitive_ShouldWorkCorrectly ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "control, ALT, sHiFt" ;

        // Act & Assert
        vm.StandingControl.Should ( ).BeTrue ( ) ;
        vm.StandingAlt.Should ( ).BeTrue ( ) ;
        vm.StandingShift.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void ModifierProperties_WithSpaces_ShouldBeParsedCorrectly ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "  Control  ,  Alt  " ;

        // Act & Assert
        vm.StandingControl.Should ( ).BeTrue ( ) ;
        vm.StandingAlt.Should ( ).BeTrue ( ) ;
        vm.StandingShift.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void ModifierProperties_EmptyString_ShouldReturnFalse ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = string.Empty ;

        // Act & Assert
        vm.StandingControl.Should ( ).BeFalse ( ) ;
        vm.StandingAlt.Should ( ).BeFalse ( ) ;
        vm.StandingShift.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void UpdateModifier_ShouldNotCreateDuplicates ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control" ;

        // Act
        vm.StandingControl = true ; // Try to add Control again

        // Assert
        vm.StandingModifiers.Split ( ',' ).Where ( s => s.Trim ( ) == "Control" ).Should ( ).HaveCount ( 1 ) ;
    }

    [ Fact ]
    public void UpdateModifier_MultipleChanges_ShouldWorkCorrectly ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = string.Empty ;

        // Act
        vm.StandingControl = true ;
        vm.StandingAlt = true ;
        vm.StandingShift = true ;

        // Assert
        vm.StandingModifiers.Should ( ).Contain ( "Control" ) ;
        vm.StandingModifiers.Should ( ).Contain ( "Alt" ) ;
        vm.StandingModifiers.Should ( ).Contain ( "Shift" ) ;
    }

    [ Fact ]
    public void UpdateModifier_RemoveAll_ShouldResultInEmptyString ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        vm.StandingModifiers = "Control, Alt, Shift" ;

        // Act
        vm.StandingControl = false ;
        vm.StandingAlt = false ;
        vm.StandingShift = false ;

        // Assert
        vm.StandingModifiers.Should ( ).BeEmpty ( ) ;
    }

    [ Fact ]
    public async Task HotkeyKey_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.StandingKey = "F10" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task HotkeyModifier_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.StandingModifiers = "Control, Shift" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task HotkeyModifierCheckbox_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Ensure we start without Control
        vm.StandingModifiers = "Alt" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;
        _synchronizer.ClearReceivedCalls ( ) ; // Clear any prior calls

        // Act
        vm.StandingControl = true ; // This should update StandingModifiers and trigger auto-save
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public void StandingName_DefaultValue_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.StandingName.Should ( ).Be ( Constants.DefaultStandingName ) ;
    }

    [ Fact ]
    public void SeatingName_DefaultValue_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.SeatingName.Should ( ).Be ( Constants.DefaultSeatingName ) ;
    }

    [ Fact ]
    public void StandingName_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.StandingName = "Standing Position" ;

        // Assert
        vm.StandingName.Should ( ).Be ( "Standing Position" ) ;
    }

    [ Fact ]
    public void SeatingName_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.SeatingName = "Sitting Position" ;

        // Assert
        vm.SeatingName.Should ( ).Be ( "Sitting Position" ) ;
    }

    [ Fact ]
    public async Task StandingName_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.StandingName = "My Standing Desk" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task SeatingName_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.SeatingName = "My Sitting Desk" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public void StandingName_ImplementsISettingsViewModel ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act & Assert - Verify the property works through the interface
        ( ( ISettingsViewModel )vm ).StandingName = "Custom Standing" ;

        vm.StandingName.Should ( ).Be ( "Custom Standing" ) ;
    }

    [ Fact ]
    public void SeatingName_ImplementsISettingsViewModel ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act & Assert - Verify the property works through the interface
        ( ( ISettingsViewModel )vm ).SeatingName = "Custom Sitting" ;

        vm.SeatingName.Should ( ).Be ( "Custom Sitting" ) ;
    }

    [ Fact ]
    public void Custom1Name_DefaultValue_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.Custom1Name.Should ( ).Be ( Constants.DefaultCustom1Name ) ;
    }

    [ Fact ]
    public void Custom2Name_DefaultValue_ShouldBeSet ( )
    {
        // Arrange & Act
        var vm = CreateSut ( ) ;

        // Assert
        vm.Custom2Name.Should ( ).Be ( Constants.DefaultCustom2Name ) ;
    }

    [ Fact ]
    public void Custom1Name_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.Custom1Name = "Custom Position 1" ;

        // Assert
        vm.Custom1Name.Should ( ).Be ( "Custom Position 1" ) ;
    }

    [ Fact ]
    public void Custom2Name_CanBeChanged ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.Custom2Name = "Custom Position 2" ;

        // Assert
        vm.Custom2Name.Should ( ).Be ( "Custom Position 2" ) ;
    }

    [ Fact ]
    public async Task Custom1Name_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.Custom1Name = "My Custom 1" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Custom2Name_Change_ShouldTriggerAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act
        vm.Custom2Name = "My Custom 2" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 400 ).Ticks ) ;

        // Assert
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task PresetNames_MultipleChanges_ShouldCoalesceAutoSave ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;
        await vm.InitializeAsync ( CancellationToken.None ) ;

        // Act - Change multiple preset names rapidly
        vm.StandingName = "New Standing" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.SeatingName = "New Sitting" ;
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 100 ).Ticks ) ;
        vm.Custom1Name = "New Custom 1" ;

        // Advance past debounce window from last change
        _scheduler.AdvanceBy ( TimeSpan.FromMilliseconds ( 350 ).Ticks ) ;

        // Assert - Should only save once due to debouncing
        await _synchronizer.Received ( 1 )
                           .StoreSettingsAsync ( vm ,
                                                 Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public void PresetNames_EmptyOrWhiteSpace_ShouldBeAllowed ( )
    {
        // Arrange
        var vm = CreateSut ( ) ;

        // Act
        vm.StandingName = string.Empty ;
        vm.SeatingName = "   " ;

        // Assert - Empty/whitespace should be allowed, synchronizer will handle defaults
        vm.StandingName.Should ( ).BeEmpty ( ) ;
        vm.SeatingName.Should ( ).Be ( "   " ) ;
    }

    private SettingsViewModel CreateSut ( )
    {
        return new SettingsViewModel ( _logger ,
                                       _settingsManager ,
                                       _scheduler ,
                                       _synchronizer ,
                                       _themeManager ,
                                       _mainWindow ,
                                       _availableKeysProvider ,
                                       _heightValidator ) ;
    }
}
