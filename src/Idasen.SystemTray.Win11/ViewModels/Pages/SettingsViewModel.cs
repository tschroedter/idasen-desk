using System.ComponentModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reflection ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Appearance ;
// Alias Wpf.Ui dialog types to avoid ambiguity with System.Windows.MessageBox
using UiMessageBox = Wpf.Ui.Controls.MessageBox ;
using UiMessageBoxResult = Wpf.Ui.Controls.MessageBoxResult ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsViewModel (
    ILogger                 logger ,
    ILoggingSettingsManager settingsManager ,
    IScheduler              scheduler ,
    ISettingsSynchronizer   synchronizer )
    : ObservableObject , INavigationAware , ISettingsViewModel
{
    [ ObservableProperty ] private string _appVersion = string.Empty ;

    private IDisposable ? _autoSaveSubscription ;

    [ ObservableProperty ] private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ] private uint _custom1 = 100 ;

    [ ObservableProperty ] private bool _custom1IsVisibleInContextMenu = true ;

    [ ObservableProperty ] private uint _custom2 = 90 ;

    [ ObservableProperty ] private bool _custom2IsVisibleInContextMenu = true ;

    [ ObservableProperty ] private string _deskAddress = string.Empty ;

    [ ObservableProperty ] private string _deskName = string.Empty ;

    private bool _disposed ;

    private bool _isInitialized ;
    private bool _isLoadingSettings ;

    [ ObservableProperty ] private uint _lastKnownDeskHeight = Constants.DefaultDeskMinHeightInCm ;

    [ ObservableProperty ] private string _logFolderPath = string.Empty ;

    [ ObservableProperty ] private uint _maxHeight = 90 ;

    [ ObservableProperty ]
    private uint _maxSpeedToStopMovement = StoppingHeightCalculatorSettings.MaxSpeedToStopMovement ;

    [ ObservableProperty ] private uint _minHeight = 90 ;

    [ ObservableProperty ] private bool _notifications = true ;

    [ ObservableProperty ] private bool _parentalLock ;

    [ ObservableProperty ] private uint _seating = 90 ;

    [ ObservableProperty ] private bool _seatingIsVisibleInContextMenu = true ;

    [ ObservableProperty ] private string _settingsFileFullPath = string.Empty ;

    private IDisposable ? _settingsSaved ;

    [ ObservableProperty ] private uint _standing = 100 ;

    [ ObservableProperty ] private bool _standingIsVisibleInContextMenu = true ;

    [ ObservableProperty ] private bool _stopIsVisibleInContextMenu = true ;

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
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing )
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
        }

        _disposed = true ;
    }

    public async Task InitializeAsync ( CancellationToken token )
    {
        _isLoadingSettings = true ;

        await synchronizer.LoadSettingsAsync ( this ,
                                               token ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ;

        _settingsSaved = settingsManager.SettingsSaved
                                        .ObserveOn ( scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;

        // Start auto-save after load
        SetupAutoSave ( ) ;
        _isLoadingSettings = false ;
    }

    private void SetupAutoSave ( )
    {
        _autoSaveSubscription?.Dispose ( ) ;

        // Observe property changes from INotifyPropertyChanged, debounce and persist settings
        _autoSaveSubscription = Observable
                               .FromEventPattern < PropertyChangedEventHandler ,
                                    PropertyChangedEventArgs > ( h => ( ( INotifyPropertyChanged )this )
                                                                     .PropertyChanged += h ,
                                                                 h => ( ( INotifyPropertyChanged )this )
                                                                     .PropertyChanged -= h )
                               .Where ( _ => ! _isLoadingSettings )
                               .Throttle ( TimeSpan.FromMilliseconds ( 300 ) ,
                                           scheduler )
                               .Select ( _ => Observable.FromAsync ( cancellationToken =>
                                                                         synchronizer.StoreSettingsAsync ( this ,
                                                                                                           cancellationToken ) ) )
                               .Switch ( )
                               .Subscribe ( _ => { } ,
                                            ex => logger.Error ( ex ,
                                                                 "Failed to auto-save settings" ) ) ;
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

    private static string GetAssemblyVersion ( )
    {
        return Assembly.GetExecutingAssembly ( ).GetName ( ).Version?.ToString ( ) ?? string.Empty ;
    }

    [ RelayCommand ]
    internal void OnChangeTheme ( string parameter )
    {
        synchronizer.ChangeTheme ( parameter ) ;
    }

    [ RelayCommand ]
    internal async Task OnResetSettings ( )
    {
        try
        {
            // Confirm with the user (only if we have a UI dispatcher; in tests we proceed without the dialog)
            var dispatcher = Application.Current?.Dispatcher ;

            if ( dispatcher != null )
            {
                var uiMessageBox = new UiMessageBox
                {
                    Title             = "Reset settings?" ,
                    Content           = "Do you want to reset all settings to their default values?" ,
                    PrimaryButtonText = "Reset" ,
                    CloseButtonText   = "Cancel"
                } ;

                UiMessageBoxResult result ;

                if ( dispatcher.CheckAccess ( ) )
                {
                    result = await uiMessageBox.ShowDialogAsync ( ) ;
                }
                else
                {
                    var showTask = dispatcher.Invoke ( ( ) => uiMessageBox.ShowDialogAsync ( ) ) ;

                    result = await showTask ;
                }

                if ( result != UiMessageBoxResult.Primary )
                    return ;
            }

            _isLoadingSettings = true ;

            // Reset settings to defaults at the source and persist once
            await settingsManager.ResetSettingsAsync ( CancellationToken.None ) ;

            // Reload the ViewModel from the freshly reset settings
            await synchronizer.LoadSettingsAsync ( this ,
                                                   CancellationToken.None ) ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "Failed to reset settings" ) ;
        }
        finally
        {
            _isLoadingSettings = false ;
        }

        // No extra save here; reset already persisted defaults
    }
}