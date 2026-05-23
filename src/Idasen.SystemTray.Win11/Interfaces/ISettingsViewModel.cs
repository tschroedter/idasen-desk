using Wpf.Ui.Appearance ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsViewModel : IDisposable
{
    uint             Standing                       { get ; set ; }
    string           StandingName                   { get ; set ; }
    uint             MinHeight                      { get ; set ; }
    uint             MaxHeight                      { get ; set ; }
    uint             Seating                        { get ; set ; }
    string           SeatingName                    { get ; set ; }
    uint             Custom1                        { get ; set ; }
    string           Custom1Name                    { get ; set ; }
    uint             Custom2                        { get ; set ; }
    string           Custom2Name                    { get ; set ; }
    uint             LastKnownDeskHeight            { get ; set ; }
    string           DeskName                       { get ; set ; }
    string           DeskAddress                    { get ; set ; }
    bool             ParentalLock                   { get ; set ; }
    bool             Notifications                  { get ; set ; }
    ApplicationTheme CurrentTheme                   { get ; set ; }
    bool             StandingIsVisibleInContextMenu { get ; set ; }
    bool             SeatingIsVisibleInContextMenu  { get ; set ; }
    bool             Custom1IsVisibleInContextMenu  { get ; set ; }
    bool             Custom2IsVisibleInContextMenu  { get ; set ; }
    bool             StopIsVisibleInContextMenu     { get ; set ; }
    uint             MaxSpeedToStopMovement         { get ; set ; }
    bool             GlobalHotkeysEnabled           { get ; set ; }
    string           StandingKey                    { get ; set ; }
    string           StandingModifiers              { get ; set ; }
    string           SeatingKey                     { get ; set ; }
    string           SeatingModifiers               { get ; set ; }
    string           Custom1Key                     { get ; set ; }
    string           Custom1Modifiers               { get ; set ; }
    string           Custom2Key                     { get ; set ; }
    string           Custom2Modifiers               { get ; set ; }
}