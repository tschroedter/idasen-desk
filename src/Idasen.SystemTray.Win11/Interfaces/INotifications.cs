using Autofac;
using Idasen.SystemTray.Win11.Utils;
using Wpf.Ui.Tray.Controls;

namespace Idasen.SystemTray.Win11.Interfaces;

public interface INotifications : IDisposable
{
    void Show(NotificationParameters parameters);

    INotifications Initialize(IContainer container, NotifyIcon notifyIcon);

    void Show(string title,
                string text,
                Visibility visibilityBulbGreen = Visibility.Hidden,
                Visibility visibilityBulbYellow = Visibility.Hidden,
                Visibility visibilityBulbRed = Visibility.Hidden);
}