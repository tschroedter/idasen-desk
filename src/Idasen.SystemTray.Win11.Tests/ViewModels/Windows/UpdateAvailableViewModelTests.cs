using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.ViewModels.Windows ;
using Idasen.TestLogger ;
using NSubstitute ;
using Serilog ;
using System.Reactive.Subjects ;
using System.Windows ;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Windows ;

public sealed class UpdateAvailableViewModelTests
{
    [ Theory ]
    [ InlineData ( false , Visibility.Collapsed ) ]
    [ InlineData ( true , Visibility.Visible ) ]
    public void Constructor_SetsVisibilityFromCurrentUpdateState ( bool isUpdateAvailable ,
                                                                    Visibility expectedVisibility )
    {
        using var updates = new Subject < bool > ( ) ;
        var updateState = Substitute.For < IUpdateAvailableState > ( ) ;
        updateState.IsUpdateAvailable.Returns ( isUpdateAvailable ) ;
        updateState.UpdateAvailableChanged.Returns ( updates ) ;

        using var sut = CreateViewModel ( updateState ) ;

        sut.UpdateAvailableVisibility.Should ( ).Be ( expectedVisibility ) ;
    }

    [ Fact ]
    public void UpdateAvailableChanged_UpdatesVisibility ( )
    {
        using var updates = new Subject < bool > ( ) ;
        var updateState = Substitute.For < IUpdateAvailableState > ( ) ;
        updateState.UpdateAvailableChanged.Returns ( updates ) ;
        using var sut = CreateViewModel ( updateState ) ;

        updates.OnNext ( true ) ;
        sut.UpdateAvailableVisibility.Should ( ).Be ( Visibility.Visible ) ;

        updates.OnNext ( false ) ;
        sut.UpdateAvailableVisibility.Should ( ).Be ( Visibility.Collapsed ) ;
    }

    [ Fact ]
    public void Dispose_UnsubscribesFromUpdateAvailableChanges ( )
    {
        using var updates = new Subject < bool > ( ) ;
        var updateState = Substitute.For < IUpdateAvailableState > ( ) ;
        updateState.UpdateAvailableChanged.Returns ( updates ) ;
        var sut = CreateViewModel ( updateState ) ;

        sut.Dispose ( ) ;
        sut.Dispose ( ) ;
        updates.OnNext ( true ) ;

        sut.UpdateAvailableVisibility.Should ( ).Be ( Visibility.Collapsed ) ;
    }

    [ Fact ]
    public void OpenLatestReleaseCommand_LogsWarningWhenOpeningFails ( )
    {
        var releaseLinkService = Substitute.For < ILatestReleaseLinkService > ( ) ;
        releaseLinkService.OpenLatestRelease ( ).Returns ( false ) ;
        using var logger = new InMemoryLogger ( ) ;
        using var sut = CreateViewModel ( latestReleaseLinkService: releaseLinkService , logger: logger ) ;

        sut.OpenLatestReleaseCommand.Execute ( null ) ;

        releaseLinkService.Received ( 1 ).OpenLatestRelease ( ) ;
        logger.Contains ( "Failed to open the latest Idasen Desk release website." )
              .Should ( )
              .BeTrue ( ) ;
    }

    [ Fact ]
    public void OpenLatestReleaseCommand_OpensLatestRelease ( )
    {
        var releaseLinkService = Substitute.For < ILatestReleaseLinkService > ( ) ;
        releaseLinkService.OpenLatestRelease ( ).Returns ( true ) ;
        using var logger = new InMemoryLogger ( ) ;
        using var sut = CreateViewModel ( latestReleaseLinkService: releaseLinkService , logger: logger ) ;

        sut.OpenLatestReleaseCommand.Execute ( null ) ;

        releaseLinkService.Received ( 1 ).OpenLatestRelease ( ) ;
        logger.Contains ( "Failed to open the latest Idasen Desk release website." )
              .Should ( )
              .BeFalse ( ) ;
    }

    private static UpdateAvailableViewModel CreateViewModel (
        IUpdateAvailableState ? updateState = null ,
        ILatestReleaseLinkService ? latestReleaseLinkService = null ,
        ILogger ? logger = null )
    {
        updateState ??= Substitute.For < IUpdateAvailableState > ( ) ;
        latestReleaseLinkService ??= Substitute.For < ILatestReleaseLinkService > ( ) ;
        logger ??= Serilog.Core.Logger.None ;

        return new UpdateAvailableViewModel ( logger , updateState , latestReleaseLinkService ) ;
    }
}
