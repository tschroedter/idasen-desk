using System.Reflection ;
using Autofac ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SettingsViewModel ( ILoggingSettingsManager        settingsManager ,
                                         INotifySettingsChanges         settingsChanges ,
                                         IDeviceAddressToULongConverter addressConverter ,
                                         IDoubleToUIntConverter         toUIntConverter ,
                                         IDeviceNameConverter           nameConverter )
    : ObservableObject , INavigationAware
{
    private bool      _isInitialized;
    private ILogger ? _logger ;

    [ObservableProperty ]
    private string _appVersion = string.Empty ;

    [ ObservableProperty ]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ]
    private string _deskAddress = string.Empty ;

    [ ObservableProperty ]
    private string _deskName = string.Empty ;

    [ ObservableProperty ]
    private string _settingsFileFullPath = string.Empty ;

    [ObservableProperty]
    private string _logFolderPath = string.Empty;

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

        LoadSettingsAsync ( ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName;
        LogFolderPath = Launcher.LoggingFile.Path;

        return this ;
    }

    public void LoadSettingsAsync ( )
    {
        Task.Run ( async ( ) =>
                   {
                       _logger?.Debug ( "LoadAsync settings" ) ;

                       await settingsManager.LoadAsync ( ) ;

                       var current = settingsManager.CurrentSettings ;

                       Standing      = current.HeightSettings.StandingHeightInCm ;
                       MinHeight     = current.HeightSettings.DeskMinHeightInCm ;
                       MaxHeight     = current.HeightSettings.DeskMaxHeightInCm ;
                       Seating       = current.HeightSettings.SeatingHeightInCm ;
                       DeskName      = nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
                       DeskAddress   = addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
                       ParentalLock  = current.DeviceSettings.DeviceLocked ;
                       Notifications = current.DeviceSettings.NotificationsEnabled ;
                   } ).GetAwaiter ( )
            .GetResult ( ) ; // todo not nice but will do for now
    }

    public void StoreSettings ( )
    {
        if ( _storingSettingsTask?.Status == TaskStatus.Running )
        {
            _logger?.Warning ( "Storing Settings already in progress" ) ;

            return ;
        }

        var lockChanged     = HasParentalLockChanged ( ) ;
        var advancedChanged = HaveAdvancedSettingsChanged ( ) ;

        UpdateCurrentSettings ( ) ;

        _storingSettingsTask = Task.Run ( async ( ) =>
                                          {
                                              await DoStoreSettingsAsync ( advancedChanged ,
                                                                           lockChanged ) ;
                                          } ) ;
    }

    private bool HasParentalLockChanged ( )
    {
        var settings = settingsManager.CurrentSettings;

        return settings.DeviceSettings.DeviceLocked != ParentalLock ;
    }

    private bool HaveAdvancedSettingsChanged ( )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName           = nameConverter.DefaultIfEmpty ( DeskName ) ;
        var newDeviceAddress        = addressConverter.DefaultIfEmpty ( DeskAddress ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    private void UpdateCurrentSettings (  )
    {
        var settings = settingsManager.CurrentSettings;

        var newDeviceName           = nameConverter.DefaultIfEmpty(DeskName);
        var newDeviceAddress        = addressConverter.DefaultIfEmpty(DeskAddress);
        var newDeviceLocked         = ParentalLock;
        var newNotificationsEnabled = Notifications;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( Standing ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm = toUIntConverter.ConvertToUInt ( Seating ,
                                                                                    Constants.DefaultHeightSeatingInCm ) ;
        settings.DeviceSettings.DeviceName           = newDeviceName ;
        settings.DeviceSettings.DeviceAddress        = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked         = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled = newNotificationsEnabled ;
    }

    private async Task DoStoreSettingsAsync ( bool      advancedChanged ,
                                              bool      lockChanged )
    {
        try
        {
            _logger?.Debug ( $"Storing new settings: {settingsManager.CurrentSettings}" ) ;

            await settingsManager.SaveAsync ( ) ;

            if ( advancedChanged )
            {
                AdvancedSettingsChanged ( advancedChanged ) ;
            }

            if ( lockChanged )
            {
                LockChanged ( settingsManager.CurrentSettings ) ;
            }
        }
        catch ( Exception e )
        {
            _logger?.Error ( e ,
                             "Failed to store settings" ) ;
        }
    }

    private void LockChanged ( ISettings settings )
    {
        _logger?.Information ( "Advanced Locked settings have changed..." ) ;

        settingsChanges.LockSettingsChanged.OnNext ( settings.DeviceSettings.DeviceLocked ) ;
    }

    private void AdvancedSettingsChanged ( bool advancedChanged )
    {
        _logger?.Information ( "Advanced settings have changed, reconnecting..." ) ;

        settingsChanges.AdvancedSettingsChanged.OnNext ( advancedChanged ) ;
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