using Idasen.SystemTray.Win11.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using Wpf.Ui.Controls;

namespace Idasen.SystemTray.Win11.Utils;

public class ToastService : IToastService
{
    public void Show(NotificationParameters parameters)
    {
        var builder = new ToastContentBuilder()
            .AddText(parameters.Title)
            .AddText(parameters.Text)
            .SetToastDuration(ToastDuration.Short);

        builder.Show();
    }
}