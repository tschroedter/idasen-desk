using System.Net.Http ;
using System.Net.Http.Json ;
using System.Text.Json ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Extensions.Hosting ;
using Serilog ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Services ;

public sealed class GitHubReleaseVersionService : BackgroundService
{
    private const string LatestReleaseEndpoint = "repos/tschroedter/idasen-desk/releases/latest" ;

    private readonly HttpClient       _httpClient ;
    private readonly ILogger          _logger ;
    private readonly INotifications       _notifications ;
    private readonly IUpdateAvailableState _updateAvailableState ;
    private readonly IVersionProvider     _versionProvider ;

    public GitHubReleaseVersionService ( HttpClient           httpClient ,
                                         ILogger              logger ,
                                         IVersionProvider     versionProvider ,
                                         INotifications       notifications ,
                                         IUpdateAvailableState updateAvailableState )
    {
        _httpClient           = httpClient ;
        _logger               = logger ;
        _versionProvider      = versionProvider ;
        _notifications        = notifications ;
        _updateAvailableState = updateAvailableState ;
    }

    public async Task LogLatestReleaseVersionAsync ( CancellationToken cancellationToken )
    {
        try
        {
            using var response = await _httpClient.GetAsync ( LatestReleaseEndpoint ,
                                                              HttpCompletionOption.ResponseHeadersRead ,
                                                              cancellationToken )
                                                  .ConfigureAwait ( false ) ;
            response.EnsureSuccessStatusCode ( ) ;

            var release = await response.Content.ReadFromJsonAsync < JsonElement >
                              ( cancellationToken : cancellationToken )
                                                  .ConfigureAwait ( false ) ;

            if ( release.ValueKind != JsonValueKind.Object ||
                 ! release.TryGetProperty ( "tag_name" , out var tagNameElement ) ||
                 tagNameElement.ValueKind != JsonValueKind.String ||
                 string.IsNullOrWhiteSpace ( tagNameElement.GetString ( ) ) )
                throw new JsonException ( "The latest release response did not contain a tag_name." ) ;

            var latestReleaseVersion = tagNameElement.GetString ( )! ;
            var currentAppVersion    = _versionProvider.GetVersion ( ) ;

            if ( ! TryParseVersion ( latestReleaseVersion , out var latestVersion ) )
                throw new JsonException ( $"The latest release tag '{latestReleaseVersion}' is not a valid version." ) ;

            if ( ! TryParseVersion ( currentAppVersion , out var currentVersion ) )
                throw new InvalidOperationException ( $"The current app version '{currentAppVersion}' is not a valid version." ) ;

            var updateAvailable = latestVersion > currentVersion ;
            _updateAvailableState.SetUpdateAvailable ( updateAvailable ) ;

            _logger.Information ( "App version {CurrentVersion}; latest GitHub release version {LatestVersion}; update available: {UpdateAvailable}" ,
                                  currentAppVersion ,
                                  latestReleaseVersion ,
                                  updateAvailable ) ;

            if ( updateAvailable )
                _notifications.Show ( "Idasen Desk update available" ,
                                      $"Version {latestReleaseVersion} is available. You are using {currentAppVersion}." ,
                                      InfoBarSeverity.Informational ,
                                      alwaysShow : true ) ;
        }
        catch ( OperationCanceledException ) when ( cancellationToken.IsCancellationRequested )
        {
        }
        catch ( HttpRequestException ex )
        {
            _logger.Error ( ex ,
                            "Failed to fetch latest GitHub release version" ) ;
        }
        catch ( JsonException ex )
        {
            _logger.Error ( ex ,
                            "Failed to parse latest GitHub release version" ) ;
        }
        catch ( InvalidOperationException ex )
        {
            _logger.Error ( ex ,
                            "Failed to compare current app version with latest GitHub release version" ) ;
        }
        catch ( OperationCanceledException ex )
        {
            _logger.Error ( ex ,
                            "Timed out fetching latest GitHub release version" ) ;
        }
    }

    private static bool TryParseVersion ( string versionText , out Version version )
    {
        var normalizedVersion = versionText.Trim ( ) ;

        if ( normalizedVersion.StartsWith ( "v" , StringComparison.OrdinalIgnoreCase ) )
            normalizedVersion = normalizedVersion [ 1.. ] ;

        return Version.TryParse ( normalizedVersion , out version! ) ;
    }

    protected override Task ExecuteAsync ( CancellationToken stoppingToken )
    {
        return LogLatestReleaseVersionAsync ( stoppingToken ) ;
    }
}
