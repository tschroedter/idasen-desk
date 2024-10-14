using System.Reflection ;
using Autofac ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Converters ;
using Serilog ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SettingsViewModel (
    ILoggingSettingsManager settingsManager ,
    INotifySettingsChanges  settingsChanges )
    : ObservableObject , INavigationAware
{
    private readonly IDeviceAddressToULongConverter _addressConverter = new DeviceAddressToULongConverter ( ) ;

    private readonly IDoubleToUIntConverter _doubleConverter = new DoubleToUIntConverter ( ) ; // todo inject
    private readonly IDeviceNameConverter   _nameConverter   = new DeviceNameConverter ( ) ;

    [ ObservableProperty ]
    private string _appVersion = string.Empty ;

    [ ObservableProperty ]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ]
    private string _deskAddress = string.Empty ;

    // Advanced Settings
    [ ObservableProperty ]
    private string _deskName = string.Empty ;

    private bool _isInitialized ;

    private ILogger ? _logger ;

    [ ObservableProperty ]
    private uint _maxHeight = 90 ;

    [ ObservableProperty ]
    private uint _minHeight = 90 ;

    [ ObservableProperty ]
    private bool _notifications = true ;

    [ ObservableProperty ]
    private bool _parentalLock ;

    [ ObservableProperty ]
    private uint _seating = 90 ;

    // General Settings
    [ ObservableProperty ]
    private uint _standing = 100 ;

    private Task ? _storingSettingsTask ;

    public void OnNavigatedTo ( )
    {
        if ( ! _isInitialized )
            InitializeViewModel ( ) ;
    }

    public void OnNavigatedFrom ( )
    {
        StoreSettings ( ) ;
    }

    public SettingsViewModel Initialize ( IContainer container )
    {
        _logger = container.Resolve < ILogger > ( ) ;

        LoadSettings ( ) ;

        return this ;
    }

    public void LoadSettings ( )
    {
        Task.Run ( async ( ) =>
                   {
                       _logger?.Debug ( "Load settings" ) ;

                       await settingsManager.Load ( ) ;

                       var current = settingsManager.CurrentSettings ;

                       Standing      = current.HeightSettings.StandingHeightInCm ;
                       MinHeight     = current.HeightSettings.DeskMinHeightInCm ;
                       MaxHeight     = current.HeightSettings.DeskMaxHeightInCm ;
                       Seating       = current.HeightSettings.SeatingHeightInCm ;
                       DeskName      = _nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
                       DeskAddress   = _addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
                       ParentalLock  = current.DeviceSettings.DeviceLocked ;
                       Notifications = current.DeviceSettings.NotificationsEnabled ;
                       //DeskName      = _nameConverter.EmptyIfDefault(current.DeviceSettings.DeviceName);
                       //DeskAddress   = _addressConverter.EmptyIfDefault(current.DeviceSettings.DeviceAddress);
                   } ).GetAwaiter ( )
            .GetResult ( ) ; // todo not nice but will do for know
    }

    public void StoreSettings ( )
    {
        if ( _storingSettingsTask?.Status == TaskStatus.Running )
        {
            _logger?.Warning ( "Storing Settings already in progress" ) ;

            return ;
        }

        var settings = settingsManager.CurrentSettings ;

        var newDeviceName           = _nameConverter.DefaultIfEmpty ( DeskName ) ;
        var newDeviceAddress        = _addressConverter.DefaultIfEmpty ( DeskAddress ) ;
        var newDeviceLocked         = ParentalLock ;
        var newNotificationsEnabled = Notifications ;

        var lockChanged = settings.DeviceSettings.DeviceLocked != newDeviceLocked ;

        settings.HeightSettings.StandingHeightInCm = _doubleConverter.ConvertToUInt ( Standing ,
                                                                                      Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm = _doubleConverter.ConvertToUInt ( Seating ,
                                                                                     Constants.DefaultHeightSeatingInCm ) ;
        settings.DeviceSettings.DeviceName           = newDeviceName ;
        settings.DeviceSettings.DeviceAddress        = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked         = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled = newNotificationsEnabled ;

        var advancedChanged = settings.DeviceSettings.DeviceName           != newDeviceName    ||
                              settings.DeviceSettings.DeviceAddress        != newDeviceAddress ||
                              settings.DeviceSettings.NotificationsEnabled != newNotificationsEnabled ;

        _storingSettingsTask = Task.Run ( async ( ) =>
                                          {
                                              await DoStoreSettings ( settings ,
                                                                      advancedChanged ,
                                                                      lockChanged ) ;
                                          } ) ;
    }

    private async Task DoStoreSettings ( ISettings settings ,
                                         bool      advancedChanged ,
                                         bool      lockChanged )
    {
        try
        {
            _logger?.Debug ( $"Storing new settings: {settings}" ) ;

            await settingsManager.Save ( ) ;

            if ( advancedChanged )
            {
                _logger?.Information ( "Advanced settings have changed, reconnecting..." ) ;

                settingsChanges.AdvancedSettingsChanged.OnNext ( advancedChanged ) ;
            }

            if ( lockChanged )
            {
                _logger?.Information ( "Advanced Locked settings have changed..." ) ;

                settingsChanges.LockSettingsChanged.OnNext ( settings.DeviceSettings.DeviceLocked ) ;
            }
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             "Failed to store settings" ) ;
        }
    }

    private void InitializeViewModel ( )
    {
        CurrentTheme = ApplicationThemeManager.GetAppTheme ( ) ;
        AppVersion   = $"UiDesktopApp1 - {GetAssemblyVersion ( )}" ;

        _isInitialized = true ;
    }

    private string GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( ).GetName ( ).Version?.ToString ( ) ?? string.Empty ;
    }

    [ RelayCommand ]
    private void OnChangeTheme ( string parameter )
    {
        switch ( parameter )
        {
            case "theme_light" :
                if ( CurrentTheme == ApplicationTheme.Light )
                    break ;

                ApplicationThemeManager.Apply ( ApplicationTheme.Light ) ;
                CurrentTheme = ApplicationTheme.Light ;

                break ;

            case "theme_dark" :
                if ( CurrentTheme == ApplicationTheme.Dark )
                    break ;

                ApplicationThemeManager.Apply ( ApplicationTheme.Dark ) ;
                CurrentTheme = ApplicationTheme.Dark ;

                break ;

            case "theme_high_contrast" :
                if ( CurrentTheme == ApplicationTheme.HighContrast )
                    break ;

                ApplicationThemeManager.Apply ( ApplicationTheme.HighContrast ) ;
                CurrentTheme = ApplicationTheme.HighContrast ;

                break ;

            default :
                if ( CurrentTheme == ApplicationTheme.Unknown )
                    break ;

                ApplicationThemeManager.Apply ( ApplicationTheme.Unknown ) ;
                CurrentTheme = ApplicationTheme.Unknown ;

                break ;
        }
    }
}