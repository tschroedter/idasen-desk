using System ;
using System.Globalization ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using NSubstitute ;
using Serilog ;
using Wpf.Ui.Appearance ;
using Xunit ;

namespace Idasen.SystemTray.Win11.IntegrationTests ;

public class SettingsSynchronizerV2Tests
{
    [ Fact ]
    public async Task LoadSettingsAsync_WhenThemeSwitcherThrows_ShouldThrowInvalidOperationException ( )
    {
        // arrange
        var themeSwitcher = Substitute.For < IThemeSwitcher > ( ) ;
        themeSwitcher.CurrentThemeName.Returns ( "Light" ) ;
        themeSwitcher
           .When ( t => t.ChangeTheme ( Arg.Any < string > ( ) ) )
           .Do ( _ => throw new InvalidOperationException ( "UI-thread error" ) ) ;

        var settingsManager = Substitute.For < ILoggingSettingsManager > ( ) ;
        settingsManager.LoadAsync ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.CompletedTask ) ;

        var currentSettings = Substitute.For < ISettings > ( ) ;
        currentSettings.HeightSettings.Returns ( new HeightSettings
        {
            DeskMinHeightInCm              = 0 ,
            DeskMaxHeightInCm              = 0 ,
            StandingHeightInCm             = 0 ,
            SeatingHeightInCm              = 0 ,
            Custom1HeightInCm              = 0 ,
            Custom2HeightInCm              = 0 ,
            LastKnownDeskHeight            = 0 ,
            StandingIsVisibleInContextMenu = true ,
            SeatingIsVisibleInContextMenu  = true ,
            Custom1IsVisibleInContextMenu  = true ,
            Custom2IsVisibleInContextMenu  = true
        } ) ;
        currentSettings.DeviceSettings.Returns ( new DeviceSettings
        {
            DeviceName                 = "desk" ,
            DeviceAddress              = 123UL ,
            DeviceLocked               = false ,
            NotificationsEnabled       = true ,
            StopIsVisibleInContextMenu = true ,
            MaxSpeedToStopMovement     = 1
        } ) ;
        currentSettings.AppearanceSettings.Returns ( new AppearanceSettings
        {
            ThemeName = "Dark" // different from themeSwitcher.CurrentThemeName to trigger ChangeTheme
        } ) ;

        settingsManager.CurrentSettings.Returns ( currentSettings ) ;

        var logger = Substitute.For < ILogger > ( ) ;
        var toUInt = Substitute.For < IDoubleToUIntConverter > ( ) ;
        toUInt.ConvertToUInt ( Arg.Any < double > ( ) ,
                               Arg.Any < uint > ( ) ).Returns ( ci => Convert.ToUInt32 ( ci [ 0 ] ,
                                                                                         CultureInfo
                                                                                            .InvariantCulture ) ) ;
        var nameConverter = Substitute.For < IDeviceNameConverter > ( ) ;
        nameConverter.EmptyIfDefault ( Arg.Any < string > ( ) ).Returns ( ci => ci [ 0 ] as string ?? string.Empty ) ;
        var addressConverter = Substitute.For < IDeviceAddressToULongConverter > ( ) ;
        addressConverter.EmptyIfDefault ( Arg.Any < ulong > ( ) ).Returns ( ci => ( ulong )ci [ 0 ] == default
                                                                                      ? string.Empty
                                                                                      : ci [ 0 ].ToString ( ) ) ;
        var settingsChanges = Substitute.For < INotifySettingsChanges > ( ) ;

        var model = new TestSettingsViewModel ( ) ;

        var sut = new SettingsSynchronizer ( logger ,
                                             settingsManager ,
                                             toUInt ,
                                             nameConverter ,
                                             addressConverter ,
                                             settingsChanges ,
                                             themeSwitcher ) ;

        // act & assert
        var ex = await Record.ExceptionAsync ( ( ) => sut.LoadSettingsAsync ( model ,
                                                                          CancellationToken.None ) ) ;
        Assert.Null ( ex ) ;
    }

    private sealed class TestSettingsViewModel : ISettingsViewModel
    {
        public uint             Standing                       { get ; set ; }
        public uint             MinHeight                      { get ; set ; }
        public uint             MaxHeight                      { get ; set ; }
        public uint             Seating                        { get ; set ; }
        public uint             Custom1                        { get ; set ; }
        public uint             Custom2                        { get ; set ; }
        public uint             LastKnownDeskHeight            { get ; set ; }
        public string           DeskName                       { get ; set ; } = string.Empty ;
        public string           DeskAddress                    { get ; set ; } = string.Empty ;
        public bool             ParentalLock                   { get ; set ; }
        public bool             Notifications                  { get ; set ; }
        public ApplicationTheme CurrentTheme                   { get ; set ; }
        public bool             StandingIsVisibleInContextMenu { get ; set ; }
        public bool             SeatingIsVisibleInContextMenu  { get ; set ; }
        public bool             Custom1IsVisibleInContextMenu  { get ; set ; }
        public bool             Custom2IsVisibleInContextMenu  { get ; set ; }
        public bool             StopIsVisibleInContextMenu     { get ; set ; }
        public uint             MaxSpeedToStopMovement         { get ; set ; }

        public void Dispose ( )
        {
        }
    }
}