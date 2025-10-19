using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Toolkit.Uwp.Notifications ;

namespace Idasen.SystemTray.Win11.Utils ;

public partial class ToastService : IToastService
{
    public void Show ( NotificationParameters parameters )
    {
        var builder = new ToastContentBuilder ( )
                     .AddText ( parameters.Title )
                     .AddText ( parameters.Text )
                     .SetToastDuration ( ToastDuration.Short ) ;

        builder.Show ( ) ;
    }
}