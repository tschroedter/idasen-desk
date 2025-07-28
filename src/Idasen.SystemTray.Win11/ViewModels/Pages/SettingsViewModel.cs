using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reflection ;
using System.Windows.Threading ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsViewModel ( ILogger                 logger ,
                                         ILoggingSettingsManager settingsManager ,
                                         IScheduler              scheduler ,
                                         ISettingsSynchronizer   synchronizer )
    : ObservableObject , INavigationAware , ISettingsViewModel
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

    private IDisposable ? _settingsSaved ;

    [ ObservableProperty ]
    private uint _standing = 100 ;

    public void Dispose ( )
    {
        if ( _settingsSaved is null )
            return ;

        _settingsSaved.Dispose ( ) ;
        _settingsSaved = null ;
    }

    public async Task OnNavigatedToAsync ( )
    {
        if ( _isInitialized )
            return ;

        await InitializeViewModelAsync ( ) ;
    }

    public async Task OnNavigatedFromAsync ( )
    {
        await synchronizer.StoreSettingsAsync ( this ,
                                                CancellationToken.None ) ;
    }

    public async Task InitializeAsync ( CancellationToken token )
    {
        await synchronizer.LoadSettingsAsync ( this ,
                                               token ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ; // Todo: Maybe this could be ILoggingFile?

        _settingsSaved = settingsManager.SettingsSaved
                                        .ObserveOn ( scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;
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

    private async Task InitializeViewModelAsync ( )
    {
        CurrentTheme = await Task.Run ( ApplicationThemeManager.GetAppTheme ).ConfigureAwait ( false ) ;
        AppVersion   = $"UiDesktopApp1 - {GetAssemblyVersion ( )}" ;

        _isInitialized = true ;
    }

    private string GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( ).GetName ( ).Version?.ToString ( ) ?? string.Empty ;
    }

    [ RelayCommand ]
    internal void OnChangeTheme ( string parameter ) => 
        synchronizer.ChangeTheme ( parameter ) ;
}