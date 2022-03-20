using System.Net ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.Settings ;
using ILogger = Serilog.ILogger ;

namespace Idasen.RESTAPI.Desk ;

public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck            _healthCheck ;
    private readonly ILogger                       _logger ;
    private readonly IMicroServiceSettingsProvider _provider ;

    public StartupBackgroundService ( ILogger                       logger ,
                                      IMicroServiceSettingsProvider provider ,
                                      StartupHealthCheck            healthCheck )
    {
        _logger      = logger ;
        _provider    = provider ;
        _healthCheck = healthCheck ;
    }

    protected override async Task ExecuteAsync ( CancellationToken stoppingToken )
    {
        bool allMicroServicesReady ;

        do
        {
            allMicroServicesReady = true ;

            foreach ( var settings in _provider.MicroServices.Values )
            {
                var result = await CheckReadinessOfService ( settings ) ;

                if ( ! result )
                    allMicroServicesReady = false ;
            }

            await Task.Delay ( TimeSpan.FromSeconds ( 1 ) ,
                               stoppingToken ) ;

            _healthCheck.StartupCompleted = allMicroServicesReady ;
        } while ( ! allMicroServicesReady ) ;
    }

    private async Task < bool > CheckReadinessOfService ( MicroServiceSettings settings )
    {
        try
        {
            _logger.Information ( $"Checking readiness for {GetReadinessUri ( settings )}..." ) ;

            using var httpClient = new HttpClient ( ) ;

            var result = await httpClient.GetAsync ( GetReadinessUri ( settings ) )
                                         .ConfigureAwait ( false ) ;

            _logger.Information ( $"Result for {GetReadinessUri ( settings )}: {result.StatusCode}" ) ;

            return result.StatusCode == HttpStatusCode.OK ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            $"Failed - Checking readiness for {GetReadinessUri ( settings )}" ) ;

            return false ;
        }
    }

    private Uri GetReadinessUri ( MicroServiceSettings settings )
    {
        var uriString = settings.Protocol +
                        settings.Host     +
                        settings.Path ;

        return new Uri ( uriString ) ;
    }
}