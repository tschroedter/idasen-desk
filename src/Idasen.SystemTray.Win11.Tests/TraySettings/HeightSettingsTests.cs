﻿using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class HeightSettingsTests
{
    [Fact]
    public void StandingHeightInCm_ShouldHaveDefaultValue()
    {
        var settings = new HeightSettings();

        settings.StandingHeightInCm
                .Should()
                .Be(Constants.DefaultHeightStandingInCm);
    }

    [Fact]
    public void SeatingHeightInCm_ShouldHaveDefaultValue()
    {
        var settings = new HeightSettings();

        settings.SeatingHeightInCm
                .Should()
                .Be(Constants.DefaultHeightSeatingInCm);
    }

    [Fact]
    public void DeskMinHeightInCm_ShouldHaveDefaultValue()
    {
        var settings = new HeightSettings();

        settings.DeskMinHeightInCm
                .Should()
                .Be(Constants.DefaultDeskMinHeightInCm);
    }

    [Fact]
    public void DeskMaxHeightInCm_ShouldHaveDefaultValue()
    {
        var settings = new HeightSettings();

        settings.DeskMaxHeightInCm
                .Should()
                .Be(Constants.DefaultDeskMaxHeightInCm);
    }

    [Fact]
    public void LastKnowDeskHeight_ShouldHaveDefaultValue()
    {
        var settings = new HeightSettings();

        settings.LastKnowDeskHeight
                .Should()
                .Be(Constants.DefaultDeskMinHeightInCm);
    }
}

