using System.Net ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Services ;
using Idasen.TestLogger ;
using NSubstitute ;
using Serilog.Events ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.Services ;

public sealed class GitHubReleaseVersionServiceTests
{
    [ Fact ]
    public async Task LogLatestReleaseVersionAsync_WhenReleaseIsAvailable_LogsBothVersionsAndComparison ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IVersionProvider > ( ) ;
        versionProvider.GetVersion ( ).Returns ( "v1.2.0" ) ;
        var notifications = Substitute.For < INotifications > ( ) ;
        var handler = new StubHttpMessageHandler ( new HttpResponseMessage ( HttpStatusCode.OK )
        {
            Content = new StringContent ( "{\"tag_name\":\"v1.2.3\"}" )
        } ) ;
        using var httpClient = new HttpClient ( handler ) ;
        httpClient.BaseAddress = new Uri ( "https://api.github.com/" ) ;
        var updateAvailableState = Substitute.For < IUpdateAvailableState > ( ) ;
        using var sut = new GitHubReleaseVersionService ( httpClient ,
                                                          logger ,
                                                          versionProvider ,
                                                          notifications ,
                                                          updateAvailableState ) ;

        // Act
        await sut.LogLatestReleaseVersionAsync ( CancellationToken.None ) ;

        // Assert
        handler.RequestUri.Should ( )
               .Be ( new Uri ( "https://api.github.com/repos/tschroedter/idasen-desk/releases/latest" ) ) ;
        logger.Output.Should ( )
              .Contain ( "App version \"v1.2.0\"; latest GitHub release version \"v1.2.3\"; update available: True" ) ;
        logger.CountLinesContaining ( "App version" ).Should ( ).Be ( 1 ) ;
        notifications.Received ( 1 ).Show ( "Idasen Desk update available" ,
                                            "Version v1.2.3 is available. You are using v1.2.0." ,
                                            InfoBarSeverity.Informational ,
                                            true ) ;
        updateAvailableState.Received ( 1 ).SetUpdateAvailable ( true ) ;
    }

    [ Fact ]
    public async Task LogLatestReleaseVersionAsync_WhenVersionsMatch_LogsNoUpdateAvailable ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IVersionProvider > ( ) ;
        versionProvider.GetVersion ( ).Returns ( "v1.2.3" ) ;
        var notifications = Substitute.For < INotifications > ( ) ;
        var handler = new StubHttpMessageHandler ( new HttpResponseMessage ( HttpStatusCode.OK )
        {
            Content = new StringContent ( "{\"tag_name\":\"v1.2.3\"}" )
        } ) ;
        using var httpClient = new HttpClient ( handler ) ;
        httpClient.BaseAddress = new Uri ( "https://api.github.com/" ) ;
        var updateAvailableState = Substitute.For < IUpdateAvailableState > ( ) ;
        using var sut = new GitHubReleaseVersionService ( httpClient ,
                                                          logger ,
                                                          versionProvider ,
                                                          notifications ,
                                                          updateAvailableState ) ;

        // Act
        await sut.LogLatestReleaseVersionAsync ( CancellationToken.None ) ;

        // Assert
        logger.Output.Should ( )
              .Contain ( "App version \"v1.2.3\"; latest GitHub release version \"v1.2.3\"; update available: False" ) ;
        notifications.DidNotReceiveWithAnyArgs ( ).Show ( null! ,
                                                          null! ,
                                                          default ) ;
        updateAvailableState.Received ( 1 ).SetUpdateAvailable ( false ) ;
    }

    [ Fact ]
    public async Task LogLatestReleaseVersionAsync_WhenCurrentVersionIsNewer_DoesNotNotify ( )
    {
        // Arrange
        using var logger          = new InMemoryLogger ( ) ;
        var       versionProvider = Substitute.For < IVersionProvider > ( ) ;
        versionProvider.GetVersion ( ).Returns ( "v1.3.0" ) ;
        var notifications = Substitute.For < INotifications > ( ) ;
        var handler = new StubHttpMessageHandler ( new HttpResponseMessage ( HttpStatusCode.OK )
        {
            Content = new StringContent ( "{\"tag_name\":\"v1.2.3\"}" )
        } ) ;
        using var httpClient = new HttpClient ( handler )
        {
            BaseAddress = new Uri ( "https://api.github.com/" )
        } ;
        var updateAvailableState = Substitute.For < IUpdateAvailableState > ( ) ;
        using var sut = new GitHubReleaseVersionService ( httpClient ,
                                                          logger ,
                                                          versionProvider ,
                                                          notifications ,
                                                          updateAvailableState ) ;

        // Act
        await sut.LogLatestReleaseVersionAsync ( CancellationToken.None ) ;

        // Assert
        updateAvailableState.Received ( 1 ).SetUpdateAvailable ( false ) ;
        notifications.DidNotReceiveWithAnyArgs ( ).Show ( null! ,
                                                          null! ,
                                                          default ) ;
    }

    [ Theory ]
    [ InlineData ( "{}" ) ]
    [ InlineData ( "{\"tag_name\":123}" ) ]
    [ InlineData ( "{\"tag_name\":\"v1.2.3-beta\"}" ) ]
    public async Task LogLatestReleaseVersionAsync_WhenReleaseTagIsInvalid_LogsFailure ( string responseContent )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var handler = new StubHttpMessageHandler ( new HttpResponseMessage ( HttpStatusCode.OK )
        {
            Content = new StringContent ( responseContent )
        } ) ;
        using var httpClient = new HttpClient ( handler )
        {
            BaseAddress = new Uri ( "https://api.github.com/" )
        } ;
        var versionProvider      = Substitute.For < IVersionProvider > ( ) ;
        var notifications        = Substitute.For < INotifications > ( ) ;
        var updateAvailableState = Substitute.For < IUpdateAvailableState > ( ) ;
        using var sut = new GitHubReleaseVersionService ( httpClient ,
                                                          logger ,
                                                          versionProvider ,
                                                          notifications ,
                                                          updateAvailableState ) ;

        // Act
        await sut.LogLatestReleaseVersionAsync ( CancellationToken.None ) ;

        // Assert
        logger.Contains ( "Failed to parse latest GitHub release version" ,
                          LogEventLevel.Error )
              .Should ( )
              .BeTrue ( ) ;
        updateAvailableState.DidNotReceiveWithAnyArgs ( ).SetUpdateAvailable ( default ) ;
        notifications.DidNotReceiveWithAnyArgs ( ).Show ( null! ,
                                                          null! ,
                                                          default ) ;
    }

    private sealed class StubHttpMessageHandler ( HttpResponseMessage response ) : HttpMessageHandler
    {
        public Uri ? RequestUri { get ; private set ; }

        protected override Task < HttpResponseMessage > SendAsync ( HttpRequestMessage request ,
                                                                    CancellationToken cancellationToken )
        {
            RequestUri = request.RequestUri ;
            return Task.FromResult ( response ) ;
        }
    }
}
