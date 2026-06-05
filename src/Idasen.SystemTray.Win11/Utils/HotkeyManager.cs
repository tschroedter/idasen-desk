using System.Windows.Input ;
using Idasen.SystemTray.Win11.Interfaces ;
using NHotkey ;
using Serilog ;
using Wpf.Ui.Controls ;
using WpfHotkeyManager = NHotkey.Wpf.HotkeyManager ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class HotkeyManager : IHotkeyManager
{
    private const string HotkeyNameStanding = "Standing" ;
    private const string HotkeyNameSeating  = "Seating" ;
    private const string HotkeyNameCustom1  = "Custom 1" ;
    private const string HotkeyNameCustom2  = "Custom 2" ;

    private static readonly KeyGesture StandingGesture = new(Key.Up ,
                                                             ModifierKeys.Control | ModifierKeys.Alt |
                                                             ModifierKeys.Shift) ;

    private static readonly KeyGesture SittingGesture = new(Key.Down ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly KeyGesture Custom1Gesture = new(Key.Left ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly KeyGesture Custom2Gesture = new(Key.Right ,
                                                            ModifierKeys.Control | ModifierKeys.Alt |
                                                            ModifierKeys.Shift) ;

    private static readonly char [ ] ModifierSeparators = [ ',' , ' ' ] ;

    private readonly ILogger           _logger ;
    private readonly ISettingsManager  _settingsManager ;
    private readonly INotifications    _notifications ;

    private bool _disposed ;

    public HotkeyManager (
        ILogger           logger ,
        ISettingsManager  settingsManager ,
        INotifications    notifications )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;
        ArgumentNullException.ThrowIfNull ( settingsManager ) ;
        ArgumentNullException.ThrowIfNull ( notifications ) ;

        _logger           = logger ;
        _settingsManager  = settingsManager ;
        _notifications    = notifications ;

        WpfHotkeyManager.HotkeyAlreadyRegistered += HotkeyManager_HotkeyAlreadyRegistered ;
    }

    public event EventHandler < EventArgs > ? StandingHotkeyPressed ;
    public event EventHandler < EventArgs > ? SeatingHotkeyPressed ;
    public event EventHandler < EventArgs > ? Custom1HotkeyPressed ;
    public event EventHandler < EventArgs > ? Custom2HotkeyPressed ;

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( HotkeyManager ) ) ;

        try
        {
            _logger.Debug ( "Unregistering global hotkeys..." ) ;
            WpfHotkeyManager.HotkeyAlreadyRegistered -= HotkeyManager_HotkeyAlreadyRegistered ;
            WpfHotkeyManager.Current.Remove ( HotkeyNameStanding ) ;
            WpfHotkeyManager.Current.Remove ( HotkeyNameSeating ) ;
            WpfHotkeyManager.Current.Remove ( HotkeyNameCustom1 ) ;
            WpfHotkeyManager.Current.Remove ( HotkeyNameCustom2 ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to unregister hotkeys during disposal (hotkeys may not have been registered)" ) ;
        }
    }

    public void RegisterGlobalHotkeys ( )
    {
        var hotkeySettings = _settingsManager.CurrentSettings.HotkeySettings ;

        _logger.Information ( "Registering global hotkeys..." ) ;

        // Create gestures from settings or use defaults
        var standingGesture = CreateKeyGesture ( hotkeySettings.StandingKey ,
                                                hotkeySettings.StandingModifiers ,
                                                StandingGesture ) ;

        var seatingGesture = CreateKeyGesture ( hotkeySettings.SeatingKey ,
                                               hotkeySettings.SeatingModifiers ,
                                               SittingGesture ) ;

        var custom1Gesture = CreateKeyGesture ( hotkeySettings.Custom1Key ,
                                               hotkeySettings.Custom1Modifiers ,
                                               Custom1Gesture ) ;

        var custom2Gesture = CreateKeyGesture ( hotkeySettings.Custom2Key ,
                                               hotkeySettings.Custom2Modifiers ,
                                               Custom2Gesture ) ;

        // Register hotkeys - safely handle if they don't exist
        SafeAddOrReplaceHotkey ( HotkeyNameStanding , standingGesture , OnGlobalHotKeyStanding ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameSeating , seatingGesture , OnGlobalHotKeySeating ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameCustom1 , custom1Gesture , OnGlobalHotKeyCustom1 ) ;
        SafeAddOrReplaceHotkey ( HotkeyNameCustom2 , custom2Gesture , OnGlobalHotKeyCustom2 ) ;

        _logger.Information ( "Global hotkeys registered successfully" ) ;
    }

    public void UnregisterGlobalHotkeys ( )
    {
        _logger.Information ( "Unregistering global hotkeys..." ) ;

        var failedHotkeys = new List<string> ( ) ;
        Exception? firstException = null ;

        var standingException = TryUnregisterGlobalHotkey ( HotkeyNameStanding ) ;
        if ( standingException is not null )
        {
            failedHotkeys.Add ( HotkeyNameStanding ) ;
            firstException ??= standingException ;
        }

        var seatingException = TryUnregisterGlobalHotkey ( HotkeyNameSeating ) ;
        if ( seatingException is not null )
        {
            failedHotkeys.Add ( HotkeyNameSeating ) ;
            firstException ??= seatingException ;
        }

        var custom1Exception = TryUnregisterGlobalHotkey ( HotkeyNameCustom1 ) ;
        if ( custom1Exception is not null )
        {
            failedHotkeys.Add ( HotkeyNameCustom1 ) ;
            firstException ??= custom1Exception ;
        }

        var custom2Exception = TryUnregisterGlobalHotkey ( HotkeyNameCustom2 ) ;
        if ( custom2Exception is not null )
        {
            failedHotkeys.Add ( HotkeyNameCustom2 ) ;
            firstException ??= custom2Exception ;
        }

        if ( failedHotkeys.Count == 0 )
        {
            _logger.Information ( "Successfully unregistered all global hotkeys" ) ;
            return ;
        }

        throw new InvalidOperationException (
            $"Failed to unregister global hotkeys: {string.Join ( ", " , failedHotkeys )}. Hotkeys may still be active." ,
            firstException ) ;
    }

    private void SafeAddOrReplaceHotkey ( string name , KeyGesture gesture , EventHandler < HotkeyEventArgs > handler )
    {
        try
        {
            // Try to remove first to avoid the "not registered" error when using AddOrReplace
            try
            {
                WpfHotkeyManager.Current.Remove ( name ) ;
                _logger.Debug ( "Removed existing hotkey: {HotkeyName}" , name ) ;
            }
            catch ( Exception ex )
            {
                // Hotkey doesn't exist, that's fine
                _logger.Debug ( ex , "Hotkey {HotkeyName} was not previously registered" , name ) ;
            }

            // Now add the hotkey
            WpfHotkeyManager.Current.AddOrReplace ( name , gesture , handler ) ;
            _logger.Debug ( "Registered hotkey: {HotkeyName} with gesture: {Gesture}" , name , gesture ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to register hotkey: {HotkeyName}" ,
                            name ) ;
        }
    }

    private Exception? TryUnregisterGlobalHotkey ( string hotkeyName )
    {
        try
        {
            _logger.Debug ( "Attempting to remove hotkey: {HotkeyName}" , hotkeyName ) ;
            WpfHotkeyManager.Current.Remove ( hotkeyName ) ;
            _logger.Information ( "Successfully removed hotkey: {HotkeyName}" , hotkeyName ) ;
            return null ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to remove hotkey: {HotkeyName}" ,
                            hotkeyName ) ;
            return e ;
        }
    }

    private void HotkeyManager_HotkeyAlreadyRegistered ( object ? sender , NHotkey.Wpf.HotkeyAlreadyRegisteredEventArgs e )
    {
        _logger.Warning ( "The hotkey {Name} is already registered by another application" ,
                          e.Name ) ;

        _notifications.Show ( "Hotkey already registered" ,
                              $"The hotkey '{e.Name}' is already registered by another application." ,
                              InfoBarSeverity.Warning ) ;
    }

    /// <summary>
    ///     Parses a key string to a Key enum value.
    ///     Maps user-friendly number keys ("1", "2", etc.) to their Key enum equivalents ("D1", "D2", etc.).
    /// </summary>
    private static Key ParseKey ( string keyString )
    {
        // Handle user-friendly number key display names (1-9, 0) by mapping to their Key enum names (D1-D9, D0)
        if ( keyString.Length == 1 && char.IsDigit ( keyString [ 0 ] ) )
        {
            keyString = "D" + keyString ;
        }

        if ( Enum.TryParse < Key > ( keyString , true , out var key ) )
            return key ;

        throw new ArgumentException ( $"Invalid key string: '{keyString}'" ,
                                      nameof ( keyString ) ) ;
    }

    /// <summary>
    ///     Parses a modifier string (e.g., "Control, Alt, Shift") to ModifierKeys enum value.
    /// </summary>
    private static ModifierKeys ParseModifierKeys ( string modifierString )
    {
        var modifiers = ModifierKeys.None ;

        if ( string.IsNullOrWhiteSpace ( modifierString ) )
            return modifiers ;

        var parts = modifierString.Split ( ModifierSeparators ,
                                           StringSplitOptions.RemoveEmptyEntries ) ;

        foreach ( var part in parts )
            if ( Enum.TryParse < ModifierKeys > ( part , true , out var modifier ) )
                modifiers |= modifier ;

        return modifiers ;
    }

    /// <summary>
    ///     Creates a KeyGesture from key and modifier strings, with fallback to default gesture.
    /// </summary>
    private KeyGesture CreateKeyGesture ( string keyString ,
                                         string modifierString ,
                                         KeyGesture defaultGesture )
    {
        try
        {
            var key       = ParseKey ( keyString ) ;
            var modifiers = ParseModifierKeys ( modifierString ) ;

            return new KeyGesture ( key ,
                                   modifiers ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to parse hotkey configuration (Key: '{Key}', Modifiers: '{Modifiers}'). Using default." ,
                             keyString ,
                             modifierString ) ;

            return defaultGesture ;
        }
    }

    private void OnGlobalHotKeyStanding ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger.Information ( "Received global hot key for 'Standing' command..." ) ;
            StandingHotkeyPressed?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception exception )
        {
            _logger.Error ( exception ,
                            "Failed to handle global hot key command for 'Standing'." ) ;
        }
    }

    private void OnGlobalHotKeySeating ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger.Information ( "Received global hot key for 'Seating' command..." ) ;
            SeatingHotkeyPressed?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception exception )
        {
            _logger.Error ( exception ,
                            "Failed to handle global hot key command for 'Seating'." ) ;
        }
    }

    private void OnGlobalHotKeyCustom1 ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger.Information ( "Received global hot key for 'Custom1' command..." ) ;
            Custom1HotkeyPressed?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception exception )
        {
            _logger.Error ( exception ,
                            "Failed to handle global hot key command for 'Custom1'." ) ;
        }
    }

    private void OnGlobalHotKeyCustom2 ( object ? sender , HotkeyEventArgs e )
    {
        try
        {
            _logger.Information ( "Received global hot key for 'Custom2' command..." ) ;
            Custom2HotkeyPressed?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception exception )
        {
            _logger.Error ( exception ,
                            "Failed to handle global hot key command for 'Custom2'." ) ;
        }
    }
}
