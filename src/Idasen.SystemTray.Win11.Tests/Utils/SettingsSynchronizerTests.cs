using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Validation ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;
using Serilog ;
using Wpf.Ui.Appearance ;

#pragma warning disable CA2012 // Use ValueTasks correctly - disabled for test mocking

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class SettingsSynchronizerTests
{
    private readonly IDeviceAddressToULongConverter _addressConverter =
        Substitute.For < IDeviceAddressToULongConverter > ( ) ;

    private readonly AppearanceSettings      _appearanceSettings = new( ) ;
    private readonly DeviceSettings          _deviceSettings     = new( ) ;
    private readonly HeightSettings          _heightSettings     = new( ) ;
    private readonly HotkeySettings          _hotkeySettings     = new( ) ;
    private readonly ILogger                 _logger             = Substitute.For < ILogger > ( ) ;
    private readonly ISettingsViewModel      _model              = Substitute.For < ISettingsViewModel > ( ) ;
    private readonly IDeviceNameConverter    _nameConverter      = Substitute.For < IDeviceNameConverter > ( ) ;
    private readonly ISettings               _settings           = Substitute.For < ISettings > ( ) ;
    private readonly INotifySettingsChanges  _settingsChanges    = Substitute.For < INotifySettingsChanges > ( ) ;
    private readonly ILoggingSettingsManager _settingsManager    = Substitute.For < ILoggingSettingsManager > ( ) ;
    private readonly IThemeSwitcher          _themeSwitcher      = Substitute.For < IThemeSwitcher > ( ) ;
    private readonly IDoubleToUIntConverter  _toUIntConverter    = Substitute.For < IDoubleToUIntConverter > ( ) ;
    private readonly IHeightSettingsValidator _heightValidator   = new HeightSettingsValidator ( ) ;

    private SettingsSynchronizer CreateSut ( )
    {
        _settingsManager.CurrentSettings.Returns ( _settings ) ;
        _settings.DeviceSettings     = _deviceSettings ;
        _settings.HeightSettings     = _heightSettings ;
        _settings.AppearanceSettings = _appearanceSettings ;
        _settings.HotkeySettings     = _hotkeySettings ;
        return new SettingsSynchronizer ( _logger ,
                                          _settingsManager ,
                                          _toUIntConverter ,
                                          _nameConverter ,
                                          _addressConverter ,
                                          _settingsChanges ,
                                          _themeSwitcher ,
                                          _heightValidator ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadSettingsAndSetModelProperties ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingHeightInCm   = 100 ;
        _heightSettings.StandingName         = "My Stand" ;
        _heightSettings.DeskMinHeightInCm    = 60 ;
        _heightSettings.DeskMaxHeightInCm    = 120 ;
        _heightSettings.SeatingHeightInCm    = 70 ;
        _heightSettings.SeatingName          = "My Sit" ;
        _heightSettings.Custom1HeightInCm    = 105 ;
        _heightSettings.Custom1Name          = "My Custom 1" ;
        _heightSettings.Custom2HeightInCm    = 72 ;
        _heightSettings.Custom2Name          = "My Custom 2" ;
        _heightSettings.LastKnownDeskHeight  = 80 ;
        _heightSettings.StandingIsVisibleInContextMenu = true ;
        _heightSettings.SeatingIsVisibleInContextMenu  = false ;
        _heightSettings.Custom1IsVisibleInContextMenu  = true ;
        _heightSettings.Custom2IsVisibleInContextMenu  = false ;
        _deviceSettings.DeviceName           = "Desk" ;
        _deviceSettings.DeviceAddress        = 12345 ;
        _deviceSettings.DeviceLocked         = true ;
        _deviceSettings.NotificationsEnabled = true ;
        _deviceSettings.StopIsVisibleInContextMenu = true ;
        _appearanceSettings.ThemeName        = "Dark" ;
        _nameConverter.EmptyIfDefault ( "Desk" ).Returns ( "Desk" ) ;
        _addressConverter.EmptyIfDefault ( 12345 ).Returns ( "12345" ) ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _model.Standing.Should ( ).Be ( 100 ) ;
        _model.StandingName.Should ( ).Be ( "My Stand" ) ;
        _model.MinHeight.Should ( ).Be ( 60 ) ;
        _model.MaxHeight.Should ( ).Be ( 120 ) ;
        _model.Seating.Should ( ).Be ( 70 ) ;
        _model.SeatingName.Should ( ).Be ( "My Sit" ) ;
        _model.Custom1.Should ( ).Be ( 105 ) ;
        _model.Custom1Name.Should ( ).Be ( "My Custom 1" ) ;
        _model.Custom2.Should ( ).Be ( 72 ) ;
        _model.Custom2Name.Should ( ).Be ( "My Custom 2" ) ;
        _model.LastKnownDeskHeight.Should ( ).Be ( 80 ) ;
        _model.StandingIsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _model.SeatingIsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _model.Custom1IsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _model.Custom2IsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _model.StopIsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _model.DeskName.Should ( ).Be ( "Desk" ) ;
        _model.DeskAddress.Should ( ).Be ( "12345" ) ;
        _model.ParentalLock.Should ( ).BeTrue ( ) ;
        _model.Notifications.Should ( ).BeTrue ( ) ;
        _model.CurrentTheme.Should ( ).Be ( ApplicationTheme.Dark ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldCallSaveAsyncAndNotifyChanges ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.ParentalLock  = true ;
        _model.DeskName      = "NewDesk" ;
        _model.DeskAddress   = "54321" ;
        _model.Notifications = true ;
        _nameConverter.DefaultIfEmpty ( "NewDesk" ).Returns ( "NewDesk" ) ;
        _addressConverter.DefaultIfEmpty ( "54321" ).Returns ( 54321UL ) ;
        _settings.DeviceSettings.DeviceLocked  = false ;
        _settings.DeviceSettings.DeviceName    = "OldDesk" ;
        _settings.DeviceSettings.DeviceAddress = 12345UL ;
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Act
        await sut.StoreSettingsAsync ( _model ,
                                       CancellationToken.None ) ;

        // Assert
        await _settingsManager.Received ( 1 ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
        _settingsChanges.AdvancedSettingsChanged.Received ( ).OnNext ( true ) ;
        _settingsChanges.LockSettingsChanged.Received ( ).OnNext ( true ) ;
    }

    [ Fact ]
    public void HasParentalLockChanged_ShouldReturnTrueIfChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _settings.DeviceSettings.DeviceLocked = false ;
        _model.ParentalLock                   = true ;

        // Act
        var result = sut.HasParentalLockChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HaveAdvancedSettingsChanged_ShouldReturnTrueIfNameOrAddressChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _settings.DeviceSettings.DeviceName    = "OldDesk" ;
        _settings.DeviceSettings.DeviceAddress = 12345UL ;
        _model.DeskName                        = "NewDesk" ;
        _model.DeskAddress                     = "54321" ;
        _nameConverter.DefaultIfEmpty ( "NewDesk" ).Returns ( "NewDesk" ) ;
        _addressConverter.DefaultIfEmpty ( "54321" ).Returns ( 54321UL ) ;

        // Act
        var result = sut.HaveAdvancedSettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateSettingsFromModel ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.Standing            = 110 ;
        _model.StandingName        = "My Standing" ;
        _model.Seating             = 75 ;
        _model.SeatingName         = "My Seating" ;
        _model.Custom1             = 112 ;
        _model.Custom1Name         = "My Custom 1" ;
        _model.Custom2             = 74 ;
        _model.Custom2Name         = "My Custom 2" ;
        _model.LastKnownDeskHeight = 85 ;
        _model.StandingIsVisibleInContextMenu = false ;
        _model.SeatingIsVisibleInContextMenu  = true ;
        _model.Custom1IsVisibleInContextMenu  = false ;
        _model.Custom2IsVisibleInContextMenu  = true ;
        _model.StopIsVisibleInContextMenu     = false ;
        _model.DeskName            = "DeskX" ;
        _model.DeskAddress         = "99999" ;
        _model.ParentalLock        = true ;
        _model.Notifications       = false ;
        _themeSwitcher.CurrentThemeName.Returns ( "Light" ) ;
        _toUIntConverter.ConvertToUInt ( 110 ,
                                         Arg.Any < uint > ( ) ).Returns ( 110u ) ;
        _toUIntConverter.ConvertToUInt ( 75 ,
                                         Arg.Any < uint > ( ) ).Returns ( 75u ) ;
        _toUIntConverter.ConvertToUInt ( 112 ,
                                         Arg.Any < uint > ( ) ).Returns ( 112u ) ;
        _toUIntConverter.ConvertToUInt ( 74 ,
                                         Arg.Any < uint > ( ) ).Returns ( 74u ) ;
        _nameConverter.DefaultIfEmpty ( "DeskX" ).Returns ( "DeskX" ) ;
        _addressConverter.DefaultIfEmpty ( "99999" ).Returns ( 99999UL ) ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.StandingHeightInCm.Should ( ).Be ( 110 ) ;
        _heightSettings.StandingName.Should ( ).Be ( "My Standing" ) ;
        _heightSettings.SeatingHeightInCm.Should ( ).Be ( 75 ) ;
        _heightSettings.SeatingName.Should ( ).Be ( "My Seating" ) ;
        _heightSettings.Custom1HeightInCm.Should ( ).Be ( 112 ) ;
        _heightSettings.Custom1Name.Should ( ).Be ( "My Custom 1" ) ;
        _heightSettings.Custom2HeightInCm.Should ( ).Be ( 74u ) ;
        _heightSettings.Custom2Name.Should ( ).Be ( "My Custom 2" ) ;
        _heightSettings.LastKnownDeskHeight.Should ( ).Be ( 85 ) ;
        _heightSettings.StandingIsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _heightSettings.SeatingIsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _heightSettings.Custom1IsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _heightSettings.Custom2IsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _deviceSettings.StopIsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _deviceSettings.DeviceName.Should ( ).Be ( "DeskX" ) ;
        _deviceSettings.DeviceAddress.Should ( ).Be ( 99999UL ) ;
        _deviceSettings.DeviceLocked.Should ( ).BeTrue ( ) ;
        _deviceSettings.NotificationsEnabled.Should ( ).BeFalse ( ) ;
        _appearanceSettings.ThemeName.Should ( ).Be ( "Light" ) ;
    }

    [ Fact ]
    public void ChangeTheme_ShouldCallThemeSwitcher ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;

        // Act
        sut.ChangeTheme ( "HighContrast" ) ;

        // Assert
        _themeSwitcher.Received ( 1 ).ChangeTheme ( "HighContrast" ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldSetMaxSpeedToStopMovement_WhenValueIsGreaterThanZero ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _deviceSettings.MaxSpeedToStopMovement = 150 ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _model.MaxSpeedToStopMovement.Should ( ).Be ( 150 ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldSetMaxSpeedToStopMovement_ToDefault_WhenValueIsZero ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _deviceSettings.MaxSpeedToStopMovement = 0 ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _model.MaxSpeedToStopMovement.Should ( ).Be ( StoppingHeightCalculatorSettings.MaxSpeedToStopMovement ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldUpdateMaxSpeedToStopMovement ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.MaxSpeedToStopMovement = 200 ;

        // Act
        await sut.StoreSettingsAsync ( _model ,
                                       CancellationToken.None ) ;

        // Assert
        _settings.DeviceSettings.MaxSpeedToStopMovement.Should ( ).Be ( 200 ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLogErrorAndThrowInvalidOperationException_WhenLoadFails ( )
    {
        // Arrange
        var sut           = CreateSut ( ) ;
        var testException = new ArgumentException ( "load failed" ) ;
        _settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Throws ( testException ) ;

        // Act
        var act = async ( ) => await sut.LoadSettingsAsync ( _model ,
                                                             CancellationToken.None ) ;

        // Assert
        await act.Should ( ).NotThrowAsync < InvalidOperationException > ( ) ;

        _logger.Received ( 1 ).Error ( testException ,
                                       "Failed to load settings! Using default settings." ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldNotifyHeightSettingsChanged_WhenSettingsAreLoaded ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingHeightInCm = 100 ;
        _heightSettings.StandingName       = "My Stand" ;
        _heightSettings.SeatingHeightInCm  = 70 ;
        _heightSettings.SeatingName        = "My Sit" ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _settingsChanges.HeightSettingsChanged.Received ( 1 ).OnNext ( true ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldNotifyHeightSettingsChanged_WhenLoadFailsAndSettingsAreReset ( )
    {
        // Arrange
        var sut           = CreateSut ( ) ;
        var testException = new ArgumentException ( "load failed" ) ;
        _settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Throws ( testException ) ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _settingsChanges.HeightSettingsChanged.Received ( 1 ).OnNext ( true ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldLogErrorAndThrowInvalidOperationException_WhenSaveFails ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) )
                        .Throws ( new ArgumentException ( "save failed" ) ) ;
        _model.ParentalLock  = true ;
        _model.DeskName      = "NewDesk" ;
        _model.DeskAddress   = "54321" ;
        _model.Notifications = true ;
        _nameConverter.DefaultIfEmpty ( "NewDesk" ).Returns ( "NewDesk" ) ;
        _addressConverter.DefaultIfEmpty ( "54321" ).Returns ( 54321UL ) ;
        _settings.DeviceSettings.DeviceLocked  = false ;
        _settings.DeviceSettings.DeviceName    = "OldDesk" ;
        _settings.DeviceSettings.DeviceAddress = 12345UL ;

        // Act
        var act = async ( ) => await sut.StoreSettingsAsync ( _model ,
                                                              CancellationToken.None ) ;

        // Assert
        await act.Should ( ).ThrowAsync < InvalidOperationException > ( )
                 .WithMessage ( "Failed to store settings" ) ;
        _logger.Received ( 1 ).Error ( Arg.Any < Exception > ( ) ,
                                       "Failed to store settings" ) ;
    }

    [ Fact ]
    public void HaveHotkeySettingsChanged_ShouldReturnTrue_WhenGlobalHotkeysEnabledChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = true } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled = false ;

        // Act
        var result = sut.HaveHotkeySettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HaveHotkeySettingsChanged_ShouldReturnFalse_WhenGlobalHotkeysEnabledUnchanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled.Returns ( true ) ;
        _model.StandingKey.Returns ( Constants.DefaultStandingKey ) ;
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.SeatingKey.Returns ( Constants.DefaultSeatingKey ) ;
        _model.SeatingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom1Key.Returns ( Constants.DefaultCustom1Key ) ;
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        var result = sut.HaveHotkeySettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldNotifyHotkeySettingsChanged_WhenGlobalHotkeysEnabledChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = false } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled = true ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _settingsChanges.HotkeySettingsChanged.Received ( 1 ).OnNext ( true ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldNotNotifyHotkeySettingsChanged_WhenGlobalHotkeysEnabledUnchanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled.Returns ( true ) ;
        _model.StandingKey.Returns ( Constants.DefaultStandingKey ) ;
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.SeatingKey.Returns ( Constants.DefaultSeatingKey ) ;
        _model.SeatingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom1Key.Returns ( Constants.DefaultCustom1Key ) ;
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _settingsChanges.HotkeySettingsChanged.DidNotReceive ( ).OnNext ( Arg.Any < bool > ( ) ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadGlobalHotkeysEnabled ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = false } ;
        _settings.HotkeySettings = hotkeySettings ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).GlobalHotkeysEnabled = false ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldUpdateGlobalHotkeysEnabled ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings { GlobalHotkeysEnabled = true } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled = false ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        hotkeySettings.GlobalHotkeysEnabled.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadStandingName ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingName = "My Standing Position" ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).StandingName = "My Standing Position" ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadSeatingName ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.SeatingName = "My Sitting Position" ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).SeatingName = "My Sitting Position" ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateStandingName ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.StandingName = "Custom Standing" ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.StandingName.Should ( ).Be ( "Custom Standing" ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateSeatingName ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.SeatingName = "Custom Sitting" ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.SeatingName.Should ( ).Be ( "Custom Sitting" ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUseDefaultStandingName_WhenModelNameIsEmpty ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.StandingName = string.Empty ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.StandingName.Should ( ).Be ( Constants.DefaultStandingName ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUseDefaultSeatingName_WhenModelNameIsWhitespace ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.SeatingName = "   " ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.SeatingName.Should ( ).Be ( Constants.DefaultSeatingName ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldSaveSettings_WhenStandingNameChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingName = "Old Standing" ;
        _model.StandingName = "New Standing" ;
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        await _settingsManager.Received ( 1 ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
        _heightSettings.StandingName.Should ( ).Be ( "New Standing" ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldSaveSettings_WhenSeatingNameChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.SeatingName = "Old Sitting" ;
        _model.SeatingName = "New Sitting" ;
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        await _settingsManager.Received ( 1 ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
        _heightSettings.SeatingName.Should ( ).Be ( "New Sitting" ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldNotSaveSettings_WhenStandingNameUnchanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;

        // Set up heights
        _heightSettings.StandingHeightInCm        = 100 ;
        _heightSettings.SeatingHeightInCm         = 70 ;
        _heightSettings.Custom1HeightInCm         = 105 ;
        _heightSettings.Custom2HeightInCm         = 72 ;
        _heightSettings.DeskMinHeightInCm         = 60 ;
        _heightSettings.DeskMaxHeightInCm         = 120 ;
        _heightSettings.LastKnownDeskHeight       = 80 ;

        // Set up names
        _heightSettings.StandingName              = "Same Standing" ;
        _heightSettings.SeatingName               = "Same Sitting" ;
        _heightSettings.Custom1Name               = "Same Custom1" ;
        _heightSettings.Custom2Name               = "Same Custom2" ;

        // Set up visibility
        _heightSettings.StandingIsVisibleInContextMenu = true ;
        _heightSettings.SeatingIsVisibleInContextMenu  = true ;
        _heightSettings.Custom1IsVisibleInContextMenu  = true ;
        _heightSettings.Custom2IsVisibleInContextMenu  = true ;

        // Set up device settings
        _deviceSettings.NotificationsEnabled      = true ;
        _deviceSettings.DeviceLocked              = false ;
        _deviceSettings.MaxSpeedToStopMovement    = 100 ;
        _deviceSettings.DeviceName                = "TestDesk" ;
        _deviceSettings.DeviceAddress             = 12345UL ;
        _deviceSettings.StopIsVisibleInContextMenu = true ;

        // Set up model with same values
        _model.Standing                           = 100 ;
        _model.Seating                            = 70 ;
        _model.Custom1                            = 105 ;
        _model.Custom2                            = 72 ;
        _model.MinHeight                          = 60 ;
        _model.MaxHeight                          = 120 ;
        _model.LastKnownDeskHeight                = 80 ;
        _model.StandingName                       = "Same Standing" ;
        _model.SeatingName                        = "Same Sitting" ;
        _model.Custom1Name                        = "Same Custom1" ;
        _model.Custom2Name                        = "Same Custom2" ;
        _model.StandingIsVisibleInContextMenu     = true ;
        _model.SeatingIsVisibleInContextMenu      = true ;
        _model.Custom1IsVisibleInContextMenu      = true ;
        _model.Custom2IsVisibleInContextMenu      = true ;
        _model.StopIsVisibleInContextMenu         = true ;
        _model.Notifications                      = true ;
        _model.ParentalLock                       = false ;
        _model.MaxSpeedToStopMovement             = 100 ;
        _model.DeskName                           = "TestDesk" ;
        _model.DeskAddress                        = "12345" ;

        // Mock converters
        _toUIntConverter.ConvertToUInt ( 100 , Arg.Any < uint > ( ) ).Returns ( 100u ) ;
        _toUIntConverter.ConvertToUInt ( 70 , Arg.Any < uint > ( ) ).Returns ( 70u ) ;
        _toUIntConverter.ConvertToUInt ( 105 , Arg.Any < uint > ( ) ).Returns ( 105u ) ;
        _toUIntConverter.ConvertToUInt ( 72 , Arg.Any < uint > ( ) ).Returns ( 72u ) ;
        _nameConverter.DefaultIfEmpty ( "TestDesk" ).Returns ( "TestDesk" ) ;
        _addressConverter.DefaultIfEmpty ( "12345" ).Returns ( 12345UL ) ;

        _themeSwitcher.CurrentThemeName.Returns ( _appearanceSettings.ThemeName ) ;

        // Mock all hotkeySettings to be the same
        var hotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled.Returns ( true ) ;
        _model.StandingKey.Returns ( Constants.DefaultStandingKey ) ;
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.SeatingKey.Returns ( Constants.DefaultSeatingKey ) ;
        _model.SeatingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom1Key.Returns ( Constants.DefaultCustom1Key ) ;
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        await _settingsManager.DidNotReceive ( ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldNotSaveSettings_WhenPresetNamesAreWhitespaceButResolveToDefaults ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;

        _heightSettings.StandingHeightInCm   = 100 ;
        _heightSettings.SeatingHeightInCm    = 70 ;
        _heightSettings.Custom1HeightInCm    = 105 ;
        _heightSettings.Custom2HeightInCm    = 72 ;
        _heightSettings.DeskMinHeightInCm    = 60 ;
        _heightSettings.DeskMaxHeightInCm    = 120 ;
        _heightSettings.LastKnownDeskHeight  = 80 ;
        _heightSettings.StandingName         = Constants.DefaultStandingName ;
        _heightSettings.SeatingName          = Constants.DefaultSeatingName ;
        _heightSettings.Custom1Name          = Constants.DefaultCustom1Name ;
        _heightSettings.Custom2Name          = Constants.DefaultCustom2Name ;
        _heightSettings.StandingIsVisibleInContextMenu = true ;
        _heightSettings.SeatingIsVisibleInContextMenu  = true ;
        _heightSettings.Custom1IsVisibleInContextMenu  = true ;
        _heightSettings.Custom2IsVisibleInContextMenu  = true ;

        _deviceSettings.NotificationsEnabled       = true ;
        _deviceSettings.DeviceLocked               = false ;
        _deviceSettings.MaxSpeedToStopMovement     = 100 ;
        _deviceSettings.DeviceName                 = "TestDesk" ;
        _deviceSettings.DeviceAddress              = 12345UL ;
        _deviceSettings.StopIsVisibleInContextMenu = true ;

        // Use default names directly instead of whitespace, since the comparison in HaveAnySettingsChanged
        // checks raw values before normalization
        _model.StandingName = Constants.DefaultStandingName ;
        _model.SeatingName  = Constants.DefaultSeatingName ;
        _model.Custom1Name  = Constants.DefaultCustom1Name ;
        _model.Custom2Name  = Constants.DefaultCustom2Name ;
        _model.Standing     = 100 ;
        _model.Seating      = 70 ;
        _model.Custom1      = 105 ;
        _model.Custom2      = 72 ;
        _model.MinHeight    = 60 ;
        _model.MaxHeight    = 120 ;
        _model.LastKnownDeskHeight            = 80 ;
        _model.StandingIsVisibleInContextMenu = true ;
        _model.SeatingIsVisibleInContextMenu  = true ;
        _model.Custom1IsVisibleInContextMenu  = true ;
        _model.Custom2IsVisibleInContextMenu  = true ;
        _model.StopIsVisibleInContextMenu     = true ;
        _model.Notifications                  = true ;
        _model.ParentalLock                   = false ;
        _model.MaxSpeedToStopMovement         = 100 ;
        _model.DeskName                       = "TestDesk" ;
        _model.DeskAddress                    = "12345" ;

        _toUIntConverter.ConvertToUInt ( 100 , Arg.Any < uint > ( ) ).Returns ( 100u ) ;
        _toUIntConverter.ConvertToUInt ( 70 , Arg.Any < uint > ( ) ).Returns ( 70u ) ;
        _toUIntConverter.ConvertToUInt ( 105 , Arg.Any < uint > ( ) ).Returns ( 105u ) ;
        _toUIntConverter.ConvertToUInt ( 72 , Arg.Any < uint > ( ) ).Returns ( 72u ) ;
        _nameConverter.DefaultIfEmpty ( "TestDesk" ).Returns ( "TestDesk" ) ;
        _addressConverter.DefaultIfEmpty ( "12345" ).Returns ( 12345UL ) ;
        _themeSwitcher.CurrentThemeName.Returns ( _appearanceSettings.ThemeName ) ;

        var hotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled.Returns ( true ) ;
        _model.StandingKey.Returns ( Constants.DefaultStandingKey ) ;
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.SeatingKey.Returns ( Constants.DefaultSeatingKey ) ;
        _model.SeatingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom1Key.Returns ( Constants.DefaultCustom1Key ) ;
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        await sut.StoreSettingsAsync ( _model ,
                                       CancellationToken.None ) ;

        // Assert
        await _settingsManager.DidNotReceive ( ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task StoreSettingsAsync_ShouldSaveSettings_WhenOnlyStandingNameChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingName              = "Old Standing" ;
        _heightSettings.SeatingName               = "Same Sitting" ;
        _heightSettings.Custom1Name               = "Same Custom1" ;
        _heightSettings.Custom2Name               = "Same Custom2" ;
        _heightSettings.StandingHeightInCm        = 100 ;
        _heightSettings.SeatingHeightInCm         = 70 ;
        _heightSettings.Custom1HeightInCm         = 105 ;
        _heightSettings.Custom2HeightInCm         = 72 ;
        _heightSettings.DeskMinHeightInCm         = 60 ;
        _heightSettings.DeskMaxHeightInCm         = 120 ;
        _heightSettings.LastKnownDeskHeight       = 80 ;
        _deviceSettings.NotificationsEnabled      = true ;
        _deviceSettings.DeviceLocked              = false ;
        _deviceSettings.MaxSpeedToStopMovement    = 100 ;
        _model.StandingName                       = "New Standing" ;
        _model.SeatingName                        = "Same Sitting" ;
        _model.Custom1Name                        = "Same Custom1" ;
        _model.Custom2Name                        = "Same Custom2" ;
        _model.Standing                           = 100 ;
        _model.Seating                            = 70 ;
        _model.Custom1                            = 105 ;
        _model.Custom2                            = 72 ;
        _model.MinHeight                          = 60 ;
        _model.MaxHeight                          = 120 ;
        _model.LastKnownDeskHeight                = 80 ;
        _model.Notifications                      = true ;
        _model.ParentalLock                       = false ;
        _model.MaxSpeedToStopMovement             = 100 ;
        _toUIntConverter.ConvertToUInt ( 100 , Arg.Any < uint > ( ) ).Returns ( 100u ) ;
        _toUIntConverter.ConvertToUInt ( 70 , Arg.Any < uint > ( ) ).Returns ( 70u ) ;
        _toUIntConverter.ConvertToUInt ( 105 , Arg.Any < uint > ( ) ).Returns ( 105u ) ;
        _toUIntConverter.ConvertToUInt ( 72 , Arg.Any < uint > ( ) ).Returns ( 72u ) ;
        _themeSwitcher.CurrentThemeName.Returns ( _appearanceSettings.ThemeName ) ;
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( new ValueTask ( ) ) ;

        // Mock all hotkeySettings to be the same
        var hotkeySettings = new HotkeySettings
        {
            GlobalHotkeysEnabled = true ,
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.GlobalHotkeysEnabled.Returns ( true ) ;
        _model.StandingKey.Returns ( Constants.DefaultStandingKey ) ;
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.SeatingKey.Returns ( Constants.DefaultSeatingKey ) ;
        _model.SeatingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom1Key.Returns ( Constants.DefaultCustom1Key ) ;
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        await sut.StoreSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        await _settingsManager.Received ( 1 ).SaveAsync ( Arg.Any < CancellationToken > ( ) ) ;
        _heightSettings.StandingName.Should ( ).Be ( "New Standing" ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadCustom1Name ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.Custom1Name = "My Custom Position 1" ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).Custom1Name = "My Custom Position 1" ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadCustom2Name ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.Custom2Name = "My Custom Position 2" ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).Custom2Name = "My Custom Position 2" ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateCustom1Name ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.Custom1Name = "Custom Position One" ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.Custom1Name.Should ( ).Be ( "Custom Position One" ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateCustom2Name ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.Custom2Name = "Custom Position Two" ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.Custom2Name.Should ( ).Be ( "Custom Position Two" ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUseDefaultCustom1Name_WhenModelNameIsEmpty ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.Custom1Name = string.Empty ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.Custom1Name.Should ( ).Be ( Constants.DefaultCustom1Name ) ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUseDefaultCustom2Name_WhenModelNameIsWhitespace ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.Custom2Name = "   " ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.Custom2Name.Should ( ).Be ( Constants.DefaultCustom2Name ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadVisibilitySettings ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingIsVisibleInContextMenu = false ;
        _heightSettings.SeatingIsVisibleInContextMenu  = true ;
        _heightSettings.Custom1IsVisibleInContextMenu  = false ;
        _heightSettings.Custom2IsVisibleInContextMenu  = true ;
        _deviceSettings.StopIsVisibleInContextMenu     = false ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).StandingIsVisibleInContextMenu = false ;
        _model.Received ( 1 ).SeatingIsVisibleInContextMenu  = true ;
        _model.Received ( 1 ).Custom1IsVisibleInContextMenu  = false ;
        _model.Received ( 1 ).Custom2IsVisibleInContextMenu  = true ;
        _model.Received ( 1 ).StopIsVisibleInContextMenu     = false ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateVisibilitySettings ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _model.StandingIsVisibleInContextMenu = true ;
        _model.SeatingIsVisibleInContextMenu  = false ;
        _model.Custom1IsVisibleInContextMenu  = true ;
        _model.Custom2IsVisibleInContextMenu  = false ;
        _model.StopIsVisibleInContextMenu     = true ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        _heightSettings.StandingIsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _heightSettings.SeatingIsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _heightSettings.Custom1IsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
        _heightSettings.Custom2IsVisibleInContextMenu.Should ( ).BeFalse ( ) ;
        _deviceSettings.StopIsVisibleInContextMenu.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadHotkeyKeys ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            StandingKey       = "S" ,
            StandingModifiers = "Control" ,
            SeatingKey        = "D" ,
            SeatingModifiers  = "Control" ,
            Custom1Key        = "D1" ,
            Custom1Modifiers  = "Alt" ,
            Custom2Key        = "D2" ,
            Custom2Modifiers  = "Alt"
        } ;
        _settings.HotkeySettings = hotkeySettings ;

        // Act
        await sut.LoadSettingsAsync ( _model , CancellationToken.None ) ;

        // Assert
        _model.Received ( 1 ).StandingKey       = "S" ;
        _model.Received ( 1 ).StandingModifiers = "Control" ;
        _model.Received ( 1 ).SeatingKey        = "D" ;
        _model.Received ( 1 ).SeatingModifiers  = "Control" ;
        _model.Received ( 1 ).Custom1Key        = "D1" ;
        _model.Received ( 1 ).Custom1Modifiers  = "Alt" ;
        _model.Received ( 1 ).Custom2Key        = "D2" ;
        _model.Received ( 1 ).Custom2Modifiers  = "Alt" ;
    }

    [ Fact ]
    public void UpdateCurrentSettings_ShouldUpdateHotkeyKeys ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings ( ) ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.StandingKey       = "S" ;
        _model.StandingModifiers = "Control" ;
        _model.SeatingKey        = "D" ;
        _model.SeatingModifiers  = "Control" ;
        _model.Custom1Key        = "D1" ;
        _model.Custom1Modifiers  = "Alt" ;
        _model.Custom2Key        = "D2" ;
        _model.Custom2Modifiers  = "Alt" ;

        // Act
        sut.UpdateCurrentSettings ( _model ) ;

        // Assert
        hotkeySettings.StandingKey.Should ( ).Be ( "S" ) ;
        hotkeySettings.StandingModifiers.Should ( ).Be ( "Control" ) ;
        hotkeySettings.SeatingKey.Should ( ).Be ( "D" ) ;
        hotkeySettings.SeatingModifiers.Should ( ).Be ( "Control" ) ;
        hotkeySettings.Custom1Key.Should ( ).Be ( "D1" ) ;
        hotkeySettings.Custom1Modifiers.Should ( ).Be ( "Alt" ) ;
        hotkeySettings.Custom2Key.Should ( ).Be ( "D2" ) ;
        hotkeySettings.Custom2Modifiers.Should ( ).Be ( "Alt" ) ;
    }

    [ Fact ]
    public void HaveHotkeySettingsChanged_ShouldReturnTrue_WhenStandingKeyChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            StandingKey          = "S" ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.StandingKey.Returns ( "D" ) ;  // Different key
        _model.StandingModifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        var result = sut.HaveHotkeySettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HaveHotkeySettingsChanged_ShouldReturnTrue_WhenCustom1KeyChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = "D1" ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = Constants.DefaultHotkeyModifiers
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.Custom1Key.Returns ( "D2" ) ;  // Different key
        _model.Custom1Modifiers.Returns ( Constants.DefaultHotkeyModifiers ) ;

        // Act
        var result = sut.HaveHotkeySettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void HaveHotkeySettingsChanged_ShouldReturnTrue_WhenCustom2ModifiersChanged ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        var hotkeySettings = new HotkeySettings
        {
            StandingKey          = Constants.DefaultStandingKey ,
            StandingModifiers    = Constants.DefaultHotkeyModifiers ,
            SeatingKey           = Constants.DefaultSeatingKey ,
            SeatingModifiers     = Constants.DefaultHotkeyModifiers ,
            Custom1Key           = Constants.DefaultCustom1Key ,
            Custom1Modifiers     = Constants.DefaultHotkeyModifiers ,
            Custom2Key           = Constants.DefaultCustom2Key ,
            Custom2Modifiers     = "Control"
        } ;
        _settings.HotkeySettings = hotkeySettings ;
        _model.Custom2Key.Returns ( Constants.DefaultCustom2Key ) ;
        _model.Custom2Modifiers.Returns ( "Alt" ) ;  // Different modifiers

        // Act
        var result = sut.HaveHotkeySettingsChanged ( _model ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
    }
}
