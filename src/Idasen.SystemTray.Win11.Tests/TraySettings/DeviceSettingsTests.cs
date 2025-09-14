using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class DeviceSettingsTests
{
    [ Fact ]
    public void DeviceName_ShouldReturnDefaultValue ( )
    {
        // Arrange
        var settings = new DeviceSettings ( ) ;

        // Act
        var deviceName = settings.DeviceName ;

        // Assert
        deviceName.Should ( )
                  .Be ( Constants.DefaultDeviceName ) ;
    }

    [ Fact ]
    public void DeviceAddress_ShouldReturnDefaultValue ( )
    {
        // Arrange
        var settings = new DeviceSettings ( ) ;

        // Act
        var deviceAddress = settings.DeviceAddress ;

        // Assert
        deviceAddress.Should ( )
                     .Be ( Constants.DefaultDeviceAddress ) ;
    }

    [ Fact ]
    public void DeviceMonitoringTimeout_ShouldReturnDefaultValue ( )
    {
        // Arrange
        var settings = new DeviceSettings ( ) ;

        // Act
        var timeout = settings.DeviceMonitoringTimeout ;

        // Assert
        timeout.Should ( )
               .Be ( Constants.DefaultDeviceMonitoringTimeout ) ;
    }

    [ Fact ]
    public void DeviceLocked_ShouldReturnDefaultValue ( )
    {
        // Arrange
        var settings = new DeviceSettings ( ) ;

        // Act
        var locked = settings.DeviceLocked ;

        // Assert
        locked.Should ( )
              .Be ( Constants.DefaultLocked ) ;
    }

    [ Fact ]
    public void NotificationsEnabled_ShouldReturnDefaultValue ( )
    {
        // Arrange
        var settings = new DeviceSettings ( ) ;

        // Act
        var notificationsEnabled = settings.NotificationsEnabled ;

        // Assert
        notificationsEnabled.Should ( )
                            .Be ( Constants.NotificationsEnabled ) ;
    }
}