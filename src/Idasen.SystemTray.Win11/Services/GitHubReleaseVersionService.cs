using System.Diagnostics.CodeAnalysis ;
using System.Net.Http ;
using System.Net.Http.Json ;
using System.Text.Json ;
using System.Text.Json.Serialization ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Extensions.Hosting ;
using Serilog ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Services ;

public sealed class GitHubReleaseVersionService (
    HttpClient httpClient ,
    ILogger logger ,
    IVersionProvider versionProvider ,
    INotifications notifications ,
    IUpdateAvailableState updateAvailableState )
    : BackgroundService
{
    private const string LatestReleaseEndpoint = "repos/tschroedter/idasen-desk/releases/latest" ;

    public async Task LogLatestReleaseVersionAsync ( CancellationToken cancellationToken )
    {
        try
        {
            using var response = await httpClient.GetAsync ( LatestReleaseEndpoint ,
                                                             HttpCompletionOption.ResponseHeadersRead ,
                                                             cancellationToken )
                                                 .ConfigureAwait ( false ) ;
            response.EnsureSuccessStatusCode ( ) ;

            var release = await response.Content.ReadFromJsonAsync < LatestReleaseResponse >
                                             ( cancellationToken )
                                        .ConfigureAwait ( false ) ;
            var latestReleaseVersion = release?.TagName ;

            if ( string.IsNullOrWhiteSpace ( latestReleaseVersion ) )
                throw new JsonException ( "The latest release response did not contain a tag_name." ) ;
            var currentAppVersion = versionProvider.GetVersion ( ) ;

            if ( ! TryParseVersion ( latestReleaseVersion ,
                                     out var latestVersion ) )
                throw new JsonException ( $"The latest release tag '{latestReleaseVersion}' is not a valid version." ) ;

            if ( ! TryParseVersion ( currentAppVersion ,
                                     out var currentVersion ) )
                throw new
                    InvalidOperationException ( $"The current app version '{currentAppVersion}' is not a valid version." ) ;

            var updateAvailable = latestVersion > currentVersion ;
            updateAvailableState.SetUpdateAvailable ( updateAvailable ) ;

            logger.Information ( "App version {CurrentVersion}; latest GitHub release version {LatestVersion}; update available: {UpdateAvailable}" ,
                                 currentAppVersion ,
                                 latestReleaseVersion ,
                                 updateAvailable ) ;

            if ( updateAvailable )
                notifications.Show ( "Idasen Desk update available" ,
                                     $"Version {latestReleaseVersion} is available. You are using {currentAppVersion}." ,
                                     InfoBarSeverity.Informational ,
                                     true ) ;
        }
        catch ( OperationCanceledException ) when ( cancellationToken.IsCancellationRequested )
        {
            logger.Debug ( "Latest GitHub release version check was canceled" ) ;
        }
        catch ( HttpRequestException ex )
        {
            logger.Error ( ex ,
                           "Failed to fetch latest GitHub release version" ) ;
        }
        catch ( JsonException ex )
        {
            logger.Error ( ex ,
                           "Failed to parse latest GitHub release version" ) ;
        }
        catch ( InvalidOperationException ex )
        {
            logger.Error ( ex ,
                           "Failed to compare current app version with latest GitHub release version" ) ;
        }
        catch ( OperationCanceledException ex )
        {
            logger.Error ( ex ,
                           "Timed out fetching latest GitHub release version" ) ;
        }
    }

    private static bool TryParseVersion ( string versionText ,
                                          [ NotNullWhen ( true ) ] out Version ? version )
    {
        var normalizedVersion = versionText.Trim ( ) ;

        if ( normalizedVersion.StartsWith ( "v" ,
                                            StringComparison.OrdinalIgnoreCase ) )
            normalizedVersion = normalizedVersion [ 1.. ] ;

        return Version.TryParse ( normalizedVersion ,
                                  out version ) ;
    }

    protected override async Task ExecuteAsync ( CancellationToken stoppingToken )
    {
        using var timer = new PeriodicTimer ( TimeSpan.FromDays ( 1 ) ) ;

        await LogLatestReleaseVersionAsync ( stoppingToken ).ConfigureAwait ( false ) ;

        while ( await timer.WaitForNextTickAsync ( stoppingToken ).ConfigureAwait ( false ) )
            await LogLatestReleaseVersionAsync ( stoppingToken ).ConfigureAwait ( false ) ;
    }

    private sealed record LatestReleaseResponse
    {
        [ JsonPropertyName ( "tag_name" ) ] public string ? TagName { get ; init ; }
    }
}
