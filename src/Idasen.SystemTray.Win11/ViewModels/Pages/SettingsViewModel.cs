using System.Reflection ;
using Idasen.SystemTray.Win11.Settings ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SettingsViewModel ( ISettingsManager settingsManager )
    : ObservableObject , INavigationAware
{
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

    [ ObservableProperty ]
    private bool _notifications = true ;

    [ ObservableProperty ]
    private bool _parentalLock ;

    [ ObservableProperty ]
    private uint _seating = 90 ;

    [ObservableProperty]
    private uint _minHeight = 90;

    [ObservableProperty]
    private uint _maxHeight = 90;

    // General Settings
    [ObservableProperty ]
    private uint _standing = 100 ;

    public void OnNavigatedTo ( )
    {
        if ( ! _isInitialized )
            InitializeViewModel ( ) ;

        LoadSettings();
    }

    public void OnNavigatedFrom ( )
    {
    }

    public SettingsViewModel Initialize ( )
    {
        LoadSettings ( ) ;

        return this ;
    }

    public void LoadSettings()
    {
        Task.Run ( async ( ) =>
                   {
                       await settingsManager.Load ( ) ;

                       Seating      = settingsManager.CurrentSettings.HeightSettings.SeatingHeightInCm ;
                       Standing     = settingsManager.CurrentSettings.HeightSettings.StandingHeightInCm ;
                       ParentalLock = settingsManager.CurrentSettings.NotificationsEnabled ;
                       MinHeight    = settingsManager.CurrentSettings.HeightSettings.DeskMinHeightInCm ;
                       MaxHeight    = settingsManager.CurrentSettings.HeightSettings.DeskMaxHeightInCm ;
                   } ).GetAwaiter ( )
            .GetResult ( ) ; // todo not nice but will do for know
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