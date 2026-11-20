using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public sealed class NotificationsTests : IDisposable
{
    private readonly ISettingsManager _settingsManager ;
    private readonly IToastService    _toastService ;
    private readonly Notifications    _notifications ;

    public NotificationsTests ( )
    {
        _settingsManager = Substitute.For < ISettingsManager > ( ) ;
        var logger = new InMemoryLogger ( ) ;
        _toastService    = Substitute.For < IToastService > ( ) ;
        _notifications   = new Notifications ( logger ,
                                              _settingsManager ,
                                              Substitute.For < IVersionProvider > ( ) ,
                                              _toastService ) ;
    }

    [ Fact ]
    public void Show_WhenNotificationsAreDisabled_DoesNotShowByDefault ( )
    {
        // Arrange
        _settingsManager.CurrentSettings.Returns ( new Settings
        {
            DeviceSettings = new DeviceSettings
            {
                NotificationsEnabled = false
            }
        } ) ;

        // Act
        _notifications.Show ( "Title" , "Text" , InfoBarSeverity.Informational ) ;

        // Assert
        _toastService.DidNotReceive ( ).Show ( Arg.Any < NotificationParameters > ( ) ) ;
    }

    [ Fact ]
    public void Show_WhenNotificationsAreDisabledAndAlwaysShowIsTrue_ShowsNotification ( )
    {
        // Arrange
        _settingsManager.CurrentSettings.Returns ( new Settings
        {
            DeviceSettings = new DeviceSettings
            {
                NotificationsEnabled = false
            }
        } ) ;

        // Act
        _notifications.Show ( "Title" , "Text" , InfoBarSeverity.Informational , alwaysShow: true ) ;

        // Assert
        _toastService.Received ( 1 ).Show ( Arg.Is < NotificationParameters > ( parameters =>
            parameters.Title == "Title" &&
            parameters.Text == "Text" &&
            parameters.Severity == InfoBarSeverity.Informational ) ) ;
    }

    [ Fact ]
    public void Show_WhenNotificationsAreEnabled_ShowsByDefault ( )
    {
        // Arrange
        _settingsManager.CurrentSettings.Returns ( new Settings
        {
            DeviceSettings = new DeviceSettings
            {
                NotificationsEnabled = true
            }
        } ) ;

        // Act
        _notifications.Show ( "Title" , "Text" , InfoBarSeverity.Informational ) ;

        // Assert
        _toastService.Received ( 1 ).Show ( Arg.Is < NotificationParameters > ( parameters =>
            parameters.Title == "Title" &&
            parameters.Text == "Text" &&
            parameters.Severity == InfoBarSeverity.Informational ) ) ;
    }

    public void Dispose ( )
    {
        _notifications.Dispose ( ) ;
    }
}
