﻿using FluentAssertions;
using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class SettingsTests
{
    [ Fact ]
    public void ToString_ShouldReturnCorrectFormat ( )
    {
        // Arrange  
        var deviceSettings = new DeviceSettings
        {
            DeviceName = "TestDevice",
            DeviceAddress = 123456789,
            DeviceMonitoringTimeout = 30,
            DeviceLocked = true,
            NotificationsEnabled = false
        };

        var heightSettings = new HeightSettings
        {
            StandingHeightInCm = 120,
            SeatingHeightInCm = 70,
            DeskMinHeightInCm = 60,
            DeskMaxHeightInCm = 130,
            LastKnowDeskHeight = 100
        };

        var settings = new Settings
        {
            DeviceSettings = deviceSettings,
            HeightSettings = heightSettings
        };

        var expected =
            $"DeviceSettings = {deviceSettings}, HeightSettings = {heightSettings}";

        // Act  
        var result = settings.ToString();

        // Assert  
        result.Should()
              .Be(expected);
    }
}