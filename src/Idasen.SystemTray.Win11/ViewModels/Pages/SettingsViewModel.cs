using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reflection ;
using System.ComponentModel ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsViewModel ( ILogger                                                          logger ,
                                         ILoggingSettingsManager                                          settingsManager ,
                                         IScheduler                                                       scheduler ,
                                         ISettingsSynchronizer                                            synchronizer ,
                                         IUiDeskManager                                                   uiDeskManager ,
                                         Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > timerFactory )
    : StatusBarInfoViewModelBase ( uiDeskManager , scheduler , timerFactory ) , INavigationAware , ISettingsViewModel
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

    private bool _isInitialized ;
    private bool _isLoadingSettings ;

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
    private bool _seatingIsVisibleInContextMenu = true ;

    [ ObservableProperty ]
    private string _settingsFileFullPath = string.Empty ;

    private IDisposable ? _settingsSaved ;
    private IDisposable ? _autoSaveSubscription ;

    [ ObservableProperty ]
    private uint _standing = 100 ;

    [ ObservableProperty ]
    private bool _standingIsVisibleInContextMenu = true ;

    [ ObservableProperty ]
    private bool _stopIsVisibleInContextMenu = true ;

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

    public override void Dispose ( )
    {
        if ( _settingsSaved is not null )
        {
            _settingsSaved.Dispose ( ) ;
            _settingsSaved = null ;
        }

        if ( _autoSaveSubscription is not null )
        {
            _autoSaveSubscription.Dispose ( ) ;
            _autoSaveSubscription = null ;
        }

        base.Dispose ( ) ;
    }

    public async Task InitializeAsync ( CancellationToken token )
    {
        _isLoadingSettings = true ;

        await synchronizer.LoadSettingsAsync ( this ,
                                               token ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ; // Todo: Maybe this could be ILoggingFile?

        _settingsSaved = settingsManager.SettingsSaved
                                        .ObserveOn ( Scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;

        // Start auto-save after load
        SetupAutoSave ( ) ;
        _isLoadingSettings = false ;

        // Initialize InfoBar from last known status now that the VM is initialized
        InitializeFromLastStatusBarInfo ( ) ;
    }

    private void SetupAutoSave ( )
    {
        _autoSaveSubscription?.Dispose ( ) ;

        // Observe property changes from INotifyPropertyChanged, debounce and persist settings
        _autoSaveSubscription = Observable
           .FromEventPattern<System.ComponentModel.PropertyChangedEventHandler, System.ComponentModel.PropertyChangedEventArgs>(
               h => ((System.ComponentModel.INotifyPropertyChanged)this).PropertyChanged += h ,
               h => ((System.ComponentModel.INotifyPropertyChanged)this).PropertyChanged -= h )
           .Where ( _ => ! _isLoadingSettings )
           .Throttle ( TimeSpan.FromMilliseconds ( 300 ) , Scheduler )
           .Select ( _ => Observable.FromAsync ( ( CancellationToken ct ) => synchronizer.StoreSettingsAsync ( this , ct ) ) )
           .Switch ( )
           .Subscribe ( _ => { } ,
                        ex => logger.Error ( ex , "Failed to auto-save settings" ) ) ;
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
}