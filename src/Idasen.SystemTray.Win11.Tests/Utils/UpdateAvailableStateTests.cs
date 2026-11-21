using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public sealed class UpdateAvailableStateTests
{
    [ Fact ]
    public void SetUpdateAvailable_PublishesStateChanges ( )
    {
        using var sut = new UpdateAvailableState ( ) ;
        var notifications = new List < bool > ( ) ;
        using var subscription = sut.UpdateAvailableChanged.Subscribe ( notifications.Add ) ;

        sut.SetUpdateAvailable ( true ) ;
        sut.SetUpdateAvailable ( false ) ;

        sut.IsUpdateAvailable.Should ( ).BeFalse ( ) ;
        notifications.Should ( ).Equal ( false , true , false ) ;
    }

    [ Fact ]
    public void SetUpdateAvailable_WhenStateDoesNotChange_DoesNotPublishAgain ( )
    {
        using var sut = new UpdateAvailableState ( ) ;
        var notifications = new List < bool > ( ) ;
        using var subscription = sut.UpdateAvailableChanged.Subscribe ( notifications.Add ) ;

        sut.SetUpdateAvailable ( true ) ;
        sut.SetUpdateAvailable ( true ) ;

        sut.IsUpdateAvailable.Should ( ).BeTrue ( ) ;
        notifications.Should ( ).Equal ( false , true ) ;
    }
}
