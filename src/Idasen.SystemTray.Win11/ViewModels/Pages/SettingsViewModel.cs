using System.ComponentModel ;
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
// Alias Wpf.Ui dialog types to avoid ambiguity with System.Windows.MessageBox
using UiMessageBox = Wpf.Ui.Controls.MessageBox ;
using UiMessageBoxResult = Wpf.Ui.Controls.MessageBoxResult ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsViewModel : ObservableObject , INavigationAware , ISettingsViewModel
{
    private readonly ILogger                  _logger ;
    private readonly ILoggingSettingsManager  _settingsManager ;
    private readonly IScheduler               _scheduler ;
    private readonly ISettingsSynchronizer    _synchronizer ;
    private readonly IApplicationThemeManager _themeManager ;
    private readonly IMainWindow              _mainWindow ;
    private readonly IAvailableKeysProvider   _availableKeysProvider ;

    private IDisposable? _autoSaveSubscription;

    [ ObservableProperty ] public partial string AppVersion { get; set; }

    [ ObservableProperty ] public partial ApplicationTheme CurrentTheme { get; set; }

    [ ObservableProperty ] public partial uint Custom1 { get; set; }

    [ ObservableProperty ] public partial bool Custom1IsVisibleInContextMenu { get; set; }

    [ ObservableProperty ] public partial string Custom1Name { get; set; }

    [ ObservableProperty ] public partial uint Custom2 { get; set; }

    [ ObservableProperty ] public partial bool Custom2IsVisibleInContextMenu { get; set; }

    [ ObservableProperty ] public partial string Custom2Name { get; set; }

    [ ObservableProperty ] public partial string DeskAddress { get; set; }

    [ ObservableProperty ] public partial string DeskName { get; set; }

    private bool _disposed ;

    private bool _isInitialized ;
    private bool _isLoadingSettings ;

    [ ObservableProperty ] public partial uint LastKnownDeskHeight { get; set; }

    [ ObservableProperty ] public partial string LogFolderPath { get; set; }

    [ ObservableProperty ] public partial uint MaxHeight { get; set; }

    [ ObservableProperty ] public partial uint MaxSpeedToStopMovement { get; set; }

    [ ObservableProperty ] public partial uint MinHeight { get; set; }

    [ ObservableProperty ] public partial bool Notifications { get; set; }

    [ ObservableProperty ] public partial bool ParentalLock { get; set; }

    [ ObservableProperty ] public partial uint Seating { get; set; }

    [ ObservableProperty ] public partial bool SeatingIsVisibleInContextMenu { get; set; }

    [ ObservableProperty ] public partial string SettingsFileFullPath { get; set; }

    private IDisposable ? _settingsSaved ;

    [ ObservableProperty ] public partial uint Standing { get; set; }

    [ ObservableProperty ] public partial bool StandingIsVisibleInContextMenu { get; set; }

    [ ObservableProperty ] public partial bool StopIsVisibleInContextMenu { get; set; }
    [ ObservableProperty ] public partial bool GlobalHotkeysEnabled { get; set; }

    [ ObservableProperty ] public partial string StandingName { get; set; }
    [ ObservableProperty ] public partial string StandingKey { get; set; }
    [ ObservableProperty ] public partial string StandingModifiers { get; set; }
    [ ObservableProperty ] public partial string SeatingName { get; set; }
    [ ObservableProperty ] public partial string SeatingKey { get; set; }
    [ ObservableProperty ] public partial string SeatingModifiers { get; set; }
    [ ObservableProperty ] public partial string Custom1Key { get; set; }
    [ ObservableProperty ] public partial string Custom1Modifiers { get; set; }
    [ ObservableProperty ] public partial string Custom2Key { get; set; }
    [ ObservableProperty ] public partial string Custom2Modifiers { get; set; }

    private bool _isUpdatingModifiers ;
    private static readonly char [ ] ModifierSeparators = [ ',' , ' ' ] ;

    public SettingsViewModel (
        ILogger                  logger ,
        ILoggingSettingsManager  settingsManager ,
        IScheduler               scheduler ,
        ISettingsSynchronizer    synchronizer ,
        IApplicationThemeManager themeManager ,
        IMainWindow              mainWindow ,
        IAvailableKeysProvider   availableKeysProvider )
    {
        _logger                 = logger ;
        _settingsManager        = settingsManager ;
        _scheduler              = scheduler ;
        _synchronizer           = synchronizer ;
        _themeManager           = themeManager ;
        _mainWindow             = mainWindow ;
        _availableKeysProvider  = availableKeysProvider ;

        // Initialize properties with default values
        AppVersion                        = string.Empty ;
        CurrentTheme                      = ApplicationTheme.Unknown ;
        Custom1                           = 100 ;
        Custom1IsVisibleInContextMenu     = true ;
        Custom1Name                       = Constants.DefaultCustom1Name ;
        Custom2                           = 90 ;
        Custom2IsVisibleInContextMenu     = true ;
        Custom2Name                       = Constants.DefaultCustom2Name ;
        DeskAddress                       = string.Empty ;
        DeskName                          = string.Empty ;
        LastKnownDeskHeight               = Constants.DefaultDeskMinHeightInCm ;
        LogFolderPath                     = string.Empty ;
        MaxHeight                         = 90 ;
        MaxSpeedToStopMovement            = Idasen.BluetoothLE.Linak.Control.StoppingHeightCalculatorSettings.MaxSpeedToStopMovement ;
        MinHeight                         = 90 ;
        Notifications                     = true ;
        ParentalLock                      = false ;
        Seating                           = 90 ;
        SeatingIsVisibleInContextMenu     = true ;
        SettingsFileFullPath              = string.Empty ;
        Standing                          = 100 ;
        StandingIsVisibleInContextMenu    = true ;
        StopIsVisibleInContextMenu        = true ;
        GlobalHotkeysEnabled              = Constants.DefaultGlobalHotkeysEnabled ;
        StandingName                      = Constants.DefaultStandingName ;
        StandingKey                       = Constants.DefaultStandingKey ;
        StandingModifiers                 = Constants.DefaultHotkeyModifiers ;
        SeatingName                       = Constants.DefaultSeatingName ;
        SeatingKey                        = Constants.DefaultSeatingKey ;
        SeatingModifiers                  = Constants.DefaultHotkeyModifiers ;
        Custom1Key                        = Constants.DefaultCustom1Key ;
        Custom1Modifiers                  = Constants.DefaultHotkeyModifiers ;
        Custom2Key                        = Constants.DefaultCustom2Key ;
        Custom2Modifiers                  = Constants.DefaultHotkeyModifiers ;
    }

    /// <summary>
    ///     Gets the available keys for hotkey configuration.
    /// </summary>
    public IReadOnlyList < string > AvailableKeys => _availableKeysProvider.AvailableKeys ;

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

        _logger.Debug ( "value = {Value}", value );

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

        _logger.Debug("value = {Value}", value);

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

        _logger.Debug("value = {Value}", value);

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

        _logger.Debug("value = {Value}", value);

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
        await _synchronizer.StoreSettingsAsync ( this ,
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

        SettingsFileFullPath = _settingsManager.SettingsFileName ;
        LogFolderPath        = LoggingFile.Path ;

        _settingsSaved = _settingsManager.SettingsSaved
                                        .ObserveOn ( _scheduler )
                                        .Subscribe ( OnSettingsSaved ) ;

        // Start auto-save after load
        SetupAutoSave ( ) ;

        // Subscribe to main window visibility changes
        SubscribeToMainWindowVisibility ( ) ;

        _isLoadingSettings = false ;
    }

    private async Task LoadAndApplySettings ( CancellationToken token )
    {
        await _synchronizer.LoadSettingsAsync ( this ,
                                               token ) ;

        await _themeManager.ApplyAsync(CurrentTheme);
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
                                           _scheduler )
                               .Select ( _ => Observable.FromAsync ( cancellationToken =>
                                                                         _synchronizer.StoreSettingsAsync ( this ,
                                                                                                           cancellationToken ) ) )
                               .Switch ( )
                               .Subscribe ( _ => { } ,
                                            ex => _logger.Error ( ex ,
                                                                 "Failed to auto-save settings" ) ) ;
    }

    private void OnSettingsSaved ( ISettings settings )
    {
        // Always marshal back to the main application dispatcher to update bindable properties
        var appDispatcher = Application.Current?.Dispatcher ;

        if ( appDispatcher != null &&
             ! appDispatcher.CheckAccess ( ) )
        {
            _logger.Debug ( "Dispatching call on UI thread" ) ;

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
        _synchronizer.ChangeTheme ( parameter ) ;
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

            await _settingsManager.ResetSettingsAsync ( CancellationToken.None ) ;

            await LoadAndApplySettings ( CancellationToken.None ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
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
            _logger.Warning ( "Visibility subscription already exists. Skipping duplicate subscription." ) ;
            return ;
        }

        // Subscribe to VisibilityChanged
        _visibilitySubscription = _mainWindow.VisibilityChanged
                                            .Where ( visibility => visibility != Visibility.Visible )
                                            .Subscribe ( async void ( _ ) =>
                                                         {
                                                             try
                                                             {
                                                                 await _synchronizer.StoreSettingsAsync ( this ,
                                                                                                         CancellationToken
                                                                                                            .None ) ;
                                                             }
                                                             catch ( Exception ex )
                                                             {
                                                                 _logger.Error ( ex ,
                                                                                "Failed to save settings when visibility changed." ) ;
                                                             }
                                                         } ) ;
    }
}
