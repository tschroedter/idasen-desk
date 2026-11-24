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
        using var httpClient = new HttpClient ( handler ) ;
        httpClient.BaseAddress = new Uri ( "https://api.github.com/" ) ;
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
        updateAvailableState.DidNotReceiveWithAnyArgs ( ).SetUpdateAvailable ( false ) ;
        notifications.DidNotReceiveWithAnyArgs ( ).Show ( null! ,
                                                          null! ,
                                                          default ) ;
    }

    [ Fact ]
    public async Task ExecuteAsync_ChecksImmediatelyAndAgainAfterOneDay ( )
    {
        // Arrange
        using var logger = new InMemoryLogger ( ) ;
        var versionProvider = Substitute.For < IVersionProvider > ( ) ;
        versionProvider.GetVersion ( ).Returns ( "v1.2.3" ) ;
        var notifications = Substitute.For < INotifications > ( ) ;
        var handler = new CountingStubHttpMessageHandler ( ) ;
        using var httpClient = new HttpClient ( handler )
        {
            BaseAddress = new Uri ( "https://api.github.com/" )
        } ;
        var updateAvailableState = Substitute.For < IUpdateAvailableState > ( ) ;
        var timeProvider = new ManualTimeProvider ( ) ;
        using var sut = new GitHubReleaseVersionService ( httpClient ,
                                                          logger ,
                                                          versionProvider ,
                                                          notifications ,
                                                          updateAvailableState ,
                                                          timeProvider ) ;

        // Act
        var testCancellationToken = TestContext.Current.CancellationToken ;
        await sut.StartAsync ( testCancellationToken ) ;
        await handler.FirstRequest.WaitAsync ( TimeSpan.FromSeconds ( 5 ) ,
                                               testCancellationToken ) ;
        timeProvider.Timer.DueTime.Should ( ).Be ( TimeSpan.FromDays ( 1 ) ) ;
        timeProvider.Advance ( TimeSpan.FromDays ( 1 ) ) ;
        timeProvider.Timer.Fire ( ) ;
        await handler.SecondRequest.WaitAsync ( TimeSpan.FromSeconds ( 5 ) ,
                                                testCancellationToken ) ;
        await sut.StopAsync ( testCancellationToken ) ;

        // Assert
        handler.RequestCount.Should ( ).Be ( 2 ) ;
        updateAvailableState.Received ( 2 ).SetUpdateAvailable ( false ) ;
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

    private sealed class CountingStubHttpMessageHandler : HttpMessageHandler
    {
        private readonly TaskCompletionSource _firstRequest = new ( TaskCreationOptions.RunContinuationsAsynchronously ) ;
        private readonly TaskCompletionSource _secondRequest = new ( TaskCreationOptions.RunContinuationsAsynchronously ) ;

        public int RequestCount { get ; private set ; }

        public Task FirstRequest => _firstRequest.Task ;

        public Task SecondRequest => _secondRequest.Task ;

        protected override Task < HttpResponseMessage > SendAsync ( HttpRequestMessage request ,
                                                                    CancellationToken cancellationToken )
        {
            RequestCount ++ ;

            if ( RequestCount == 1 )
                _firstRequest.TrySetResult ( ) ;
            else if ( RequestCount == 2 )
                _secondRequest.TrySetResult ( ) ;

            return Task.FromResult ( new HttpResponseMessage ( HttpStatusCode.OK )
            {
                Content = new StringContent ( "{\"tag_name\":\"v1.2.3\"}" )
            } ) ;
        }
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private long _timestamp ;

        public ManualTimer Timer { get ; private set ; } = null! ;

        public override long TimestampFrequency => TimeSpan.TicksPerSecond ;

        public override long GetTimestamp ( ) => _timestamp ;

        public override ITimer CreateTimer ( TimerCallback callback ,
                                             object ? state ,
                                             TimeSpan dueTime ,
                                             TimeSpan period )
        {
            Timer = new ManualTimer ( callback , state ) ;
            Timer.Change ( dueTime , period ) ;
            return Timer ;
        }

        public void Advance ( TimeSpan elapsed ) => _timestamp += elapsed.Ticks ;
    }

    private sealed class ManualTimer ( TimerCallback callback ,
                                       object ? state ) : ITimer
    {
        public TimeSpan DueTime { get ; private set ; }

        public bool Change ( TimeSpan dueTime , TimeSpan period )
        {
            DueTime = dueTime ;
            return true ;
        }

        public void Fire ( ) => callback ( state ) ;

        public void Dispose ( ) => DueTime = Timeout.InfiniteTimeSpan ;

        public ValueTask DisposeAsync ( )
        {
            Dispose ( ) ;
            return ValueTask.CompletedTask ;
        }
    }
}
