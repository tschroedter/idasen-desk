using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reflection ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsViewModel ( ILogger                                                          logger ,
                                         ILoggingSettingsManager                                          settingsManager ,
                                         IScheduler                                                       scheduler ,
                                         ISettingsSynchronizer                                            synchronizer ,
                                         IUiDeskManager                                                   uiDeskManager ,
                                         Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > timerFactory )
    : ObservableObject , INavigationAware , ISettingsViewModel
{
    [ ObservableProperty ]
    private string _appVersion = string.Empty ;

    [ ObservableProperty ]
    private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ]
    private uint _custom1 = 100 ;

    [ ObservableProperty ]
    private bool _custom1IsVisibleInContextMenu = true ;

    [ ObservableProperty ]
    private uint _custom2 = 90 ;

    [ ObservableProperty ]
    private bool _custom2IsVisibleInContextMenu = true ;

    [ ObservableProperty ]
    private string _deskAddress = string.Empty ;

    [ ObservableProperty ]
    private string _deskName = string.Empty ;

    [ ObservableProperty ]
    private uint _height ;

    private bool _isInitialized ;

    [ ObservableProperty ]
    private uint _lastKnownDeskHeight = Constants.DefaultDeskMinHeightInCm ;

    [ ObservableProperty ]
    private string _logFolderPath = string.Empty ;

    [ ObservableProperty ]
    private uint _maxHeight = 90 ;

    [ ObservableProperty ]
    private string _message = "Unknown" ;

    [ ObservableProperty ]
    private uint _minHeight = 90 ;

    [ ObservableProperty ]
    private bool _notifications = true ;

    [ ObservableProperty ]
    private bool _parentalLock ;

    [ ObservableProperty ]
    private uint _seating = 90 ;

    [ ObservableProperty ]
    private bool _seatingIsVisibleInContextMenu = true ;

    [ ObservableProperty ]
    private string _settingsFileFullPath = string.Empty ;

    private IDisposable ? _settingsSaved ;

    [ ObservableProperty ]
    private InfoBarSeverity _severity = InfoBarSeverity.Informational ;

    [ ObservableProperty ]
    private uint _standing = 100 ;

    [ ObservableProperty ]
    private bool _standingIsVisibleInContextMenu = true ;

    private IDisposable ? _statusBarInfoChanged ;

    [ ObservableProperty ]
    private bool _stopIsVisibleInContextMenu = true ;

    private ITimer ? _timer ;

    [ ObservableProperty ]
    private string _title = "Desk Status" ;

    public async Task OnNavigatedToAsync ( )
    {
        if ( _isInitialized )
            return ;

        await InitializeViewModelAsync ( ) ;
    }

    public async Task OnNavigatedFromAsync ( )
    {
        await synchronizer.StoreSettingsAsync ( this ,
                                                CancellationToken.None ).ConfigureAwait ( false ) ;
    }

    public void Dispose ( )
    {
        if ( _settingsSaved is not null )
        {
            _settingsSaved.Dispose ( ) ;
            _settingsSaved = null ;
        }

        if ( _statusBarInfoChanged is not null )
        {
            _statusBarInfoChanged.Dispose ( ) ;
            _statusBarInfoChanged = null ;
        }

        _timer?.Dispose();
        _timer = null ;
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

        // Initialize InfoBar with the latest known status and subscribe to changes
        Message  = uiDeskManager.LastStatusBarInfo.Message ;
        Severity = uiDeskManager.LastStatusBarInfo.Severity ;

        _statusBarInfoChanged = uiDeskManager.StatusBarInfoChanged
                                             .ObserveOn ( scheduler )
                                             .Subscribe ( OnStatusBarInfoChanged ) ;

        _timer = timerFactory ( OnElapsed ,
                                null ,
                                TimeSpan.FromSeconds ( 10 ) ,
                                Timeout.InfiniteTimeSpan ) ;
    }


    private void OnSettingsSaved ( ISettings settings )
    {
        // Always marshal back to the main application dispatcher to update bindable properties
        var appDispatcher = Application.Current?.Dispatcher ;

        if ( appDispatcher != null &&
             ! appDispatcher.CheckAccess ( ) )
        {
            logger.Debug ( "Dispatching call on UI thread" ) ;

            appDispatcher.BeginInvoke ( new Action ( ( ) => OnSettingsSaved ( settings ) ) ) ;

            return ;
        }

        LastKnownDeskHeight = settings.HeightSettings.LastKnownDeskHeight ;
    }

    private async Task InitializeViewModelAsync ( )
    {
        // Keep continuation on UI thread to safely update bindable properties
        CurrentTheme = await Task.Run ( ApplicationThemeManager.GetAppTheme ) ;
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
        synchronizer.ChangeTheme ( parameter ) ;
    }

    private void OnStatusBarInfoChanged ( StatusBarInfo info )
    {
        Message  = info.Message ;
        Severity = info.Severity ;
        Height   = info.Height ;

        _timer?.Change ( TimeSpan.FromSeconds ( 10 ) ,
                         Timeout.InfiniteTimeSpan ) ;
    }

    [ ExcludeFromCodeCoverage ]
    internal void OnElapsed ( object ? state )
    {
        Application.Current.Dispatcher.BeginInvoke ( DefaultInfoBar ) ;
    }

    internal void DefaultInfoBar ( )
    {
        if ( uiDeskManager is not { IsConnected: true } )
            return ;

        Message = Height == 0
                      ? "Can't determine desk height."
                      : $"Current desk height {Height} cm" ;

        Severity = InfoBarSeverity.Informational ;
    }
}