using Autofac;
using Idasen.SystemTray.Win11.Utils;
using Wpf.Ui.Tray.Controls;

namespace Idasen.SystemTray.Win11.Interfaces;

public interface IUiDeskManager : IDisposable
{
    bool IsInitialize { get; }
    IObservable<StatusBarInfo> StatusBarInfoChanged { get; }
    StatusBarInfo LastStatusBarInfo { get; }
    UiDeskManager Initialize(IContainer container, NotifyIcon notifyIcon);
    Task Stand();
    Task Sit();
    Task AutoConnect();
    Task Disconnect();
    Task Hide();
    Task Exit();
    Task Stop();
    Task MoveLock();
    Task MoveUnlock();
}