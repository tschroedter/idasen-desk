using System.Net ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.Settings ;
using Microsoft.Extensions.Hosting ;
using Serilog ;

namespace Idasen.RESTAPI.MicroService.Shared ;

public class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck              _healthCheck ;
    private readonly ILogger                         _logger ;
    private readonly IMicroServiceSettingsProvider   _provider ;
    private readonly IMicroServiceSettingsUriCreator _uriCreator ;

    public StartupBackgroundService ( ILogger                         logger ,
                                      IMicroServiceSettingsProvider   provider ,
                                      IMicroServiceSettingsUriCreator uriCreator ,
                                      StartupHealthCheck              healthCheck )
    {
        _logger      = logger ;
        _provider    = provider ;
        _uriCreator  = uriCreator ;
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
            _logger.Information($"Checking readiness for settings: {settings}...");

            var requestUri = _uriCreator.GetUri(settings);

            _logger.Information ( $"Checking readiness for {requestUri}..." ) ;

            using var httpClient = new HttpClient ( ) ;

            var result = await httpClient.GetAsync ( requestUri )
                                         .ConfigureAwait ( false ) ;

            _logger.Information ( $"Result for {requestUri}: {result.StatusCode}" ) ;

            return result.StatusCode == HttpStatusCode.OK ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            $"Failed - Checking readiness for settings: {settings}" ) ;

            return false ;
        }
    }
}