using FluentAssertions ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.TraySettings ;

public class HeightSettingsTests
{
    [ Fact ]
    public void StandingHeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.StandingHeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.HeightStandingInCm ) ;
    }

    [ Fact ]
    public void SeatingHeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.SeatingHeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.HeightSeatingInCm ) ;
    }

    [ Fact ]
    public void Custom1HeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.Custom1HeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.HeightStandingInCm ) ;
    }

    [ Fact ]
    public void Custom2HeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.Custom2HeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.HeightSeatingInCm ) ;
    }

    [ Fact ]
    public void DeskMinHeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.DeskMinHeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.DeskMinHeightInCm ) ;
    }

    [ Fact ]
    public void DeskMaxHeightInCm_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.DeskMaxHeightInCm
                .Should ( )
                .Be ( AppConfiguration.Defaults.DeskMaxHeightInCm ) ;
    }

    [ Fact ]
    public void LastKnowDeskHeight_ShouldHaveDefaultValue ( )
    {
        var settings = new HeightSettings ( ) ;

        settings.LastKnownDeskHeight
                .Should ( )
                .Be ( AppConfiguration.Defaults.DeskMinHeightInCm ) ;
    }
}
