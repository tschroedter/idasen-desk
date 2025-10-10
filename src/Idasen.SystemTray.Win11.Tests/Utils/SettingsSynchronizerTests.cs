using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;
using Serilog ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class SettingsSynchronizerTests
{
    private readonly IDeviceAddressToULongConverter _addressConverter =
        Substitute.For < IDeviceAddressToULongConverter > ( ) ;

    private readonly AppearanceSettings      _appearanceSettings = new( ) ;
    private readonly DeviceSettings          _deviceSettings     = new( ) ;
    private readonly HeightSettings          _heightSettings     = new( ) ;
    private readonly ILogger                 _logger             = Substitute.For < ILogger > ( ) ;
    private readonly ISettingsViewModel      _model              = Substitute.For < ISettingsViewModel > ( ) ;
    private readonly IDeviceNameConverter    _nameConverter      = Substitute.For < IDeviceNameConverter > ( ) ;
    private readonly ISettings               _settings           = Substitute.For < ISettings > ( ) ;
    private readonly INotifySettingsChanges  _settingsChanges    = Substitute.For < INotifySettingsChanges > ( ) ;
    private readonly ILoggingSettingsManager _settingsManager    = Substitute.For < ILoggingSettingsManager > ( ) ;
    private readonly IThemeSwitcher          _themeSwitcher      = Substitute.For < IThemeSwitcher > ( ) ;
    private readonly IDoubleToUIntConverter  _toUIntConverter    = Substitute.For < IDoubleToUIntConverter > ( ) ;

    private SettingsSynchronizer CreateSut ( )
    {
        _settingsManager.CurrentSettings.Returns ( _settings ) ;
        _settings.DeviceSettings     = _deviceSettings ;
        _settings.HeightSettings     = _heightSettings ;
        _settings.AppearanceSettings = _appearanceSettings ;
        return new SettingsSynchronizer ( _logger ,
                                          _settingsManager ,
                                          _toUIntConverter ,
                                          _nameConverter ,
                                          _addressConverter ,
                                          _settingsChanges ,
                                          _themeSwitcher ) ;
    }

    [ Fact ]
    public async Task LoadSettingsAsync_ShouldLoadSettingsAndSetModelProperties ( )
    {
        // Arrange
        var sut = CreateSut ( ) ;
        _heightSettings.StandingHeightInCm   = 100 ;
        _heightSettings.DeskMinHeightInCm    = 60 ;
        _heightSettings.DeskMaxHeightInCm    = 120 ;
        _heightSettings.SeatingHeightInCm    = 70 ;
        _heightSettings.Custom1HeightInCm    = 105 ;
        _heightSettings.Custom2HeightInCm    = 72 ;
        _heightSettings.LastKnownDeskHeight  = 80 ;
        _deviceSettings.DeviceName           = "Desk" ;
        _deviceSettings.DeviceAddress        = 12345 ;
        _deviceSettings.DeviceLocked         = true ;
        _deviceSettings.NotificationsEnabled = true ;
        _appearanceSettings.ThemeName        = "Dark" ;
        _nameConverter.EmptyIfDefault ( "Desk" ).Returns ( "Desk" ) ;
        _addressConverter.EmptyIfDefault ( 12345 ).Returns ( "12345" ) ;

        // Act
        await sut.LoadSettingsAsync ( _model ,
                                      CancellationToken.None ) ;

        // Assert
        _model.Standing.Should ( ).Be ( 100 ) ;
        _model.MinHeight.Should ( ).Be ( 60 ) ;
        _model.MaxHeight.Should ( ).Be ( 120 ) ;
        _model.Seating.Should ( ).Be ( 70 ) ;
        _model.Custom1.Should ( ).Be ( 105 ) ;
        _model.Custom2.Should ( ).Be ( 72 ) ;
        _model.LastKnownDeskHeight.Should ( ).Be ( 80 ) ;
        _model.DeskName.Should ( ).Be ( "Desk" ) ;
        _model.DeskAddress.Should ( ).Be ( "12345" ) ;
        _model.ParentalLock.Should ( ).BeTrue ( ) ;
        _model.Notifications.Should ( ).BeTrue ( ) ;
        _model.CurrentTheme.Should ( ).Be ( ApplicationTheme.Dark ) ;
        _themeSwitcher.Received ( 1 ).ChangeTheme ( "Dark" ) ;
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
        _settingsManager.SaveAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

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
        _model.Seating             = 75 ;
        _model.Custom1             = 112 ;
        _model.Custom2             = 74 ;
        _model.LastKnownDeskHeight = 85 ;
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
        _heightSettings.SeatingHeightInCm.Should ( ).Be ( 75 ) ;
        _heightSettings.Custom1HeightInCm.Should ( ).Be ( 112 ) ;
        _heightSettings.Custom2HeightInCm.Should ( ).Be ( 74u ) ;
        _heightSettings.LastKnownDeskHeight.Should ( ).Be ( 85 ) ;
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
        await act.Should ( ).ThrowAsync < InvalidOperationException > ( )
                 .WithMessage ( "Failed to load settings" ) ;
        _logger.Received ( 1 ).Error ( testException ,
                                       "Failed to load settings" ) ;
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
}