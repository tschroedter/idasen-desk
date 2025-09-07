namespace Idasen.SystemTray.Win11.Interfaces ;

public interface ISettingsViewModel : IDisposable
{
    uint Standing { get; set; }
    uint MinHeight { get; set; }
    uint MaxHeight { get; set; }
    uint Seating { get; set; }
    uint Custom1 { get; set; }
    uint Custom2 { get; set; }
    uint LastKnownDeskHeight { get; set; }
    string DeskName { get; set; }
    string DeskAddress { get; set; }
    bool ParentalLock { get; set; }
    bool Notifications { get; set; }
    Wpf.Ui.Appearance.ApplicationTheme CurrentTheme { get; set; }
}
