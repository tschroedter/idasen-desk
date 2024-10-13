using System.Reactive.Subjects ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public interface INotifySettingsChanges
{
    ISubject<bool> AdvancedSettingsChanged { get; }
    ISubject<bool> LockSettingsChanged     { get; }
}