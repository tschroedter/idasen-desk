using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reflection ;
using System.Windows.Threading ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ExcludeFromCodeCoverage]
public partial class SettingsViewModel ( ILogger                        logger ,
                                         ILoggingSettingsManager        settingsManager ,
                                         INotifySettingsChanges         settingsChanges ,
                                         IDeviceAddressToULongConverter addressConverter ,
                                         IDoubleToUIntConverter         toUIntConverter ,
                                         IDeviceNameConverter           nameConverter ,
                                         IScheduler                     scheduler )
    : ObservableObject , INavigationAware
{
    [ ObservableProperty ]
    private string _appVersion = string.Empty ;

    [ ObservableProperty ]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ]
    private string _deskAddress = string.Empty ;

    [ ObservableProperty ]
    private string _deskName = string.Empty ;

    private bool _isInitialized ;

    [ ObservableProperty ]
    private uint _lastKnownDeskHeight = Constants.DefaultDeskMinHeightInCm ;

    [ ObservableProperty ]
    private string _logFolderPath = string.Empty ;

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
    private string _settingsFileFullPath = string.Empty ;

    private IDisposable ? _settingsSaved ; // todo dispose

    [ ObservableProperty ]
    private uint _standing = 100 ;

    private Task ? _storingSettingsTask ;

    public SettingsViewModel Initialize ( )
    {
        LoadSettingsAsync ( ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ; // Todo: Maybe this could be ILoggingFile?

        _settingsSaved = settingsManager.SettingsSaved
                                        .ObserveOn ( scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;

        return this ;
    }

    private void OnSettingsSaved ( ISettings settings )
    {
        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            logger.Debug ( "Dispatching call on UI thread" ) ;

            Dispatcher.CurrentDispatcher.BeginInvoke ( new Action ( ( ) => OnSettingsSaved ( settings ) ) ) ;

            return ;
        }

        LastKnownDeskHeight = settings.HeightSettings.LastKnownDeskHeight ;
    }

    public void LoadSettingsAsync ( )
    {
        Task.Run ( async ( ) =>
                   {
                       logger.Debug ( "LoadAsync settings" ) ;

                       await settingsManager.LoadAsync ( ) ;

                       var current = settingsManager.CurrentSettings ;

                       Standing            = current.HeightSettings.StandingHeightInCm ;
                       MinHeight           = current.HeightSettings.DeskMinHeightInCm ;
                       MaxHeight           = current.HeightSettings.DeskMaxHeightInCm ;
                       Seating             = current.HeightSettings.SeatingHeightInCm ;
                       LastKnownDeskHeight = current.HeightSettings.LastKnownDeskHeight ;
                       DeskName            = nameConverter.EmptyIfDefault ( current.DeviceSettings.DeviceName ) ;
                       DeskAddress         = addressConverter.EmptyIfDefault ( current.DeviceSettings.DeviceAddress ) ;
                       ParentalLock        = current.DeviceSettings.DeviceLocked ;
                       Notifications       = current.DeviceSettings.NotificationsEnabled ;
                   } ).GetAwaiter ( )
            .GetResult ( ) ; // todo not nice but will do for now
    }

    public void StoreSettings ( )
    {
        if ( _storingSettingsTask?.Status == TaskStatus.Running )
        {
            logger.Warning ( "Storing Settings already in progress" ) ;

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
        var settings = settingsManager.CurrentSettings ;

        return settings.DeviceSettings.DeviceLocked != ParentalLock ;
    }

    private bool HaveAdvancedSettingsChanged ( )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName    = nameConverter.DefaultIfEmpty ( DeskName ) ;
        var newDeviceAddress = addressConverter.DefaultIfEmpty ( DeskAddress ) ;

        return settings.DeviceSettings.DeviceName    != newDeviceName ||
               settings.DeviceSettings.DeviceAddress != newDeviceAddress ;
    }

    private void UpdateCurrentSettings ( )
    {
        var settings = settingsManager.CurrentSettings ;

        var newDeviceName           = nameConverter.DefaultIfEmpty ( DeskName ) ;
        var newDeviceAddress        = addressConverter.DefaultIfEmpty ( DeskAddress ) ;
        var newDeviceLocked         = ParentalLock ;
        var newNotificationsEnabled = Notifications ;

        settings.HeightSettings.StandingHeightInCm = toUIntConverter.ConvertToUInt ( Standing ,
                                                                                     Constants.DefaultHeightStandingInCm ) ;
        settings.HeightSettings.SeatingHeightInCm = toUIntConverter.ConvertToUInt ( Seating ,
                                                                                    Constants.DefaultHeightSeatingInCm ) ;
        settings.HeightSettings.LastKnownDeskHeight   = LastKnownDeskHeight ;
        settings.DeviceSettings.DeviceName           = newDeviceName ;
        settings.DeviceSettings.DeviceAddress        = newDeviceAddress ;
        settings.DeviceSettings.DeviceLocked         = newDeviceLocked ;
        settings.DeviceSettings.NotificationsEnabled = newNotificationsEnabled ;
    }

    private async Task DoStoreSettingsAsync ( bool advancedChanged ,
                                              bool lockChanged )
    {
        try
        {
            logger.Debug ( $"Storing new settings: {settingsManager.CurrentSettings}" ) ;

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
            logger.Error ( e ,
                            "Failed to store settings" ) ;
        }
    }

    private void LockChanged ( ISettings settings )
    {
        logger.Information ( "Advanced Locked settings have changed..." ) ;

        settingsChanges.LockSettingsChanged.OnNext ( settings.DeviceSettings.DeviceLocked ) ;
    }

    private void AdvancedSettingsChanged ( bool advancedChanged )
    {
        logger.Information ( "Advanced settings have changed, reconnecting..." ) ;

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
    internal void OnChangeTheme ( string parameter )
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

    public Task OnNavigatedToAsync ( )
    {
        if (!_isInitialized)
            InitializeViewModel(); // todo make async

        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync ( )
    {
        StoreSettings(); // todo make async

        return Task.CompletedTask;
    }
}