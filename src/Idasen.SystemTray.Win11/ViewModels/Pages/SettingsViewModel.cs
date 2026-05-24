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
    ILogger                  logger ,
    ILoggingSettingsManager  settingsManager ,
    IScheduler               scheduler ,
    ISettingsSynchronizer    synchronizer ,
    IApplicationThemeManager themeManager ,
    IMainWindow              mainWindow ,
    IAvailableKeysProvider   availableKeysProvider )
    : ObservableObject , INavigationAware , ISettingsViewModel
{
    [ ObservableProperty ] private string _appVersion = string.Empty ;

    private IDisposable ? _autoSaveSubscription ;

    [ ObservableProperty ] private ApplicationTheme _currentTheme = ApplicationTheme.Unknown ;

    [ ObservableProperty ] private uint _custom1 = 100 ;

    [ ObservableProperty ] private bool _custom1IsVisibleInContextMenu = true ;

    [ ObservableProperty ] private string _custom1Name = Constants.DefaultCustom1Name ;

    [ ObservableProperty ] private uint _custom2 = 90 ;

    [ ObservableProperty ] private bool _custom2IsVisibleInContextMenu = true ;

    [ ObservableProperty ] private string _custom2Name = Constants.DefaultCustom2Name ;

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
    [ ObservableProperty ] private bool _globalHotkeysEnabled       = true ;

    [ ObservableProperty ] private string _standingName      = Constants.DefaultStandingName ;
    [ ObservableProperty ] private string _standingKey       = Constants.DefaultStandingKey ;
    [ ObservableProperty ] private string _standingModifiers = Constants.DefaultHotkeyModifiers ;
    [ ObservableProperty ] private string _seatingName       = Constants.DefaultSeatingName ;
    [ ObservableProperty ] private string _seatingKey        = Constants.DefaultSeatingKey ;
    [ ObservableProperty ] private string _seatingModifiers  = Constants.DefaultHotkeyModifiers ;
    [ ObservableProperty ] private string _custom1Key        = Constants.DefaultCustom1Key ;
    [ ObservableProperty ] private string _custom1Modifiers  = Constants.DefaultHotkeyModifiers ;
    [ ObservableProperty ] private string _custom2Key        = Constants.DefaultCustom2Key ;
    [ ObservableProperty ] private string _custom2Modifiers  = Constants.DefaultHotkeyModifiers ;

    private bool _isUpdatingModifiers ;
    private static readonly char [ ] ModifierSeparators = [ ',' , ' ' ] ;

    /// <summary>
    ///     Gets the available keys for hotkey configuration.
    /// </summary>
    public IReadOnlyList < string > AvailableKeys => availableKeysProvider.AvailableKeys ;

    public bool StandingControl
    {
        get => ContainsModifier ( StandingModifiers , "Control" ) ;
        set => UpdateModifier ( "Standing" , "Control" , value ) ;
    }

    public bool StandingAlt
    {
        get => ContainsModifier ( StandingModifiers , "Alt" ) ;
        set => UpdateModifier ( "Standing" , "Alt" , value ) ;
    }

    public bool StandingShift
    {
        get => ContainsModifier ( StandingModifiers , "Shift" ) ;
        set => UpdateModifier ( "Standing" , "Shift" , value ) ;
    }

    public bool SeatingControl
    {
        get => ContainsModifier ( SeatingModifiers , "Control" ) ;
        set => UpdateModifier ( "Seating" , "Control" , value ) ;
    }

    public bool SeatingAlt
    {
        get => ContainsModifier ( SeatingModifiers , "Alt" ) ;
        set => UpdateModifier ( "Seating" , "Alt" , value ) ;
    }

    public bool SeatingShift
    {
        get => ContainsModifier ( SeatingModifiers , "Shift" ) ;
        set => UpdateModifier ( "Seating" , "Shift" , value ) ;
    }

    public bool Custom1Control
    {
        get => ContainsModifier ( Custom1Modifiers , "Control" ) ;
        set => UpdateModifier ( "Custom1" , "Control" , value ) ;
    }

    public bool Custom1Alt
    {
        get => ContainsModifier ( Custom1Modifiers , "Alt" ) ;
        set => UpdateModifier ( "Custom1" , "Alt" , value ) ;
    }

    public bool Custom1Shift
    {
        get => ContainsModifier ( Custom1Modifiers , "Shift" ) ;
        set => UpdateModifier ( "Custom1" , "Shift" , value ) ;
    }

    public bool Custom2Control
    {
        get => ContainsModifier ( Custom2Modifiers , "Control" ) ;
        set => UpdateModifier ( "Custom2" , "Control" , value ) ;
    }

    public bool Custom2Alt
    {
        get => ContainsModifier ( Custom2Modifiers , "Alt" ) ;
        set => UpdateModifier ( "Custom2" , "Alt" , value ) ;
    }

    public bool Custom2Shift
    {
        get => ContainsModifier ( Custom2Modifiers , "Shift" ) ;
        set => UpdateModifier ( "Custom2" , "Shift" , value ) ;
    }

    private void UpdateModifier ( string hotkeyName , string modifier , bool add )
    {
        if ( _isUpdatingModifiers )
            return ;

        _isUpdatingModifiers = true ;

        try
        {
            var currentModifiers = hotkeyName switch
            {
                "Standing" => StandingModifiers ,
                "Seating"  => SeatingModifiers ,
                "Custom1"  => Custom1Modifiers ,
                "Custom2"  => Custom2Modifiers ,
                _          => string.Empty
            } ;

            var modifiers = ParseModifiers ( currentModifiers ) ;

            if ( add )
            {
                if ( ! modifiers.Contains ( modifier ) )
                    modifiers.Add ( modifier ) ;
            }
            else
            {
                modifiers.Remove ( modifier ) ;
            }

            var newValue = string.Join ( ", " , modifiers ) ;

            switch ( hotkeyName )
            {
                case "Standing":
                    StandingModifiers = newValue ;
                    break ;
                case "Seating":
                    SeatingModifiers = newValue ;
                    break ;
                case "Custom1":
                    Custom1Modifiers = newValue ;
                    break ;
                case "Custom2":
                    Custom2Modifiers = newValue ;
                    break ;
            }
        }
        finally
        {
            _isUpdatingModifiers = false ;
        }
    }

    private static bool ContainsModifier ( string modifierString , string modifier )
    {
        if ( string.IsNullOrWhiteSpace ( modifierString ) )
            return false ;

        return ParseModifiers ( modifierString )
              .Any ( parsedModifier => string.Equals ( parsedModifier , modifier , StringComparison.OrdinalIgnoreCase ) ) ;
    }

    private static List < string > ParseModifiers ( string modifierString )
    {
        if ( string.IsNullOrWhiteSpace ( modifierString ) )
            return new List < string > ( ) ;

        return modifierString
              .Split ( ModifierSeparators , StringSplitOptions.RemoveEmptyEntries )
              .Select ( s => s.Trim ( ) )
              .Where ( s => ! string.IsNullOrEmpty ( s ) )
              .ToList ( ) ;
    }

    partial void OnStandingModifiersChanged ( string value )
    {
        if ( _isUpdatingModifiers )
        {
            return ;
        }

        logger.Debug ( "value = {Value}", value );

        OnPropertyChanged ( nameof ( StandingControl ) ) ;
        OnPropertyChanged ( nameof ( StandingAlt ) ) ;
        OnPropertyChanged ( nameof ( StandingShift ) ) ;
    }

    partial void OnSeatingModifiersChanged ( string value )
    {
        if (_isUpdatingModifiers)
        {
            return;
        }

        logger.Debug("value = {Value}", value);

        OnPropertyChanged(nameof(SeatingControl));
        OnPropertyChanged(nameof(SeatingAlt));
        OnPropertyChanged(nameof(SeatingShift));
    }

    partial void OnCustom1ModifiersChanged ( string value )
    {
        if (_isUpdatingModifiers)
        {
            return;
        }

        logger.Debug("value = {Value}", value);

        OnPropertyChanged(nameof(Custom1Control));
        OnPropertyChanged(nameof(Custom1Alt));
        OnPropertyChanged(nameof(Custom1Shift));
    }

    partial void OnCustom2ModifiersChanged ( string value )
    {
        if (_isUpdatingModifiers)
        {
            return;
        }

        logger.Debug("value = {Value}", value);

        OnPropertyChanged(nameof(Custom2Control));
        OnPropertyChanged(nameof(Custom2Alt));
        OnPropertyChanged(nameof(Custom2Shift));
    }

    private IDisposable ? _visibilitySubscription ;


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

            _visibilitySubscription?.Dispose ( ) ;
            _visibilitySubscription = null ;
        }

        _disposed = true ;
    }

    public async Task InitializeAsync ( CancellationToken token )
    {
        _isLoadingSettings = true ;

        await LoadAndApplySettings ( token ) ;

        SettingsFileFullPath = settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ;

        _settingsSaved = settingsManager.SettingsSaved
                                        .ObserveOn ( scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;

        // Start auto-save after load
        SetupAutoSave ( ) ;

        // Subscribe to main window visibility changes
        SubscribeToMainWindowVisibility ( ) ;

        _isLoadingSettings = false ;
    }

    private async Task LoadAndApplySettings ( CancellationToken token )
    {
        await synchronizer.LoadSettingsAsync ( this ,
                                               token ) ;

        await themeManager.ApplyAsync(CurrentTheme);
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
        AppVersion   = $"{GetAssemblyVersion ( )}" ;

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

            await settingsManager.ResetSettingsAsync ( CancellationToken.None ) ;

            await LoadAndApplySettings ( CancellationToken.None ) ;
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

    private void SubscribeToMainWindowVisibility ( )
    {
        if ( _visibilitySubscription != null )
        {
            logger.Warning ( "Visibility subscription already exists. Skipping duplicate subscription." ) ;
            return ;
        }

        // Subscribe to VisibilityChanged
        _visibilitySubscription = mainWindow.VisibilityChanged
                                            .Where ( visibility => visibility != Visibility.Visible )
                                            .Subscribe ( async void ( _ ) =>
                                                         {
                                                             try
                                                             {
                                                                 await synchronizer.StoreSettingsAsync ( this ,
                                                                                                         CancellationToken
                                                                                                            .None ) ;
                                                             }
                                                             catch ( Exception ex )
                                                             {
                                                                 logger.Error ( ex ,
                                                                                "Failed to save settings when visibility changed." ) ;
                                                             }
                                                         } ) ;
    }
}