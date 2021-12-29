using System ;
using System.Threading.Tasks ;
using Dapr.Client ;
using Idasen.RESTAPI.Dtos ;
using Idasen.RESTAPI.Interfaces ;
using Microsoft.Extensions.Logging ;

namespace Idasen.RESTAPI.Repositories
{
    public class SettingsRepository
        : ISettingsRepository
    {
        public SettingsRepository ( ILogger < SettingsRepository > logger )
        {
            _logger = logger ;

            _client = new DaprClientBuilder ( ).Build ( ) ;
        }

        public async Task < SettingsDto > GetSettingsById ( string id )
        {
            try
            {
                return await _client.GetStateAsync < SettingsDto > ( StoreName ,
                                                                     id )
                                    .ConfigureAwait ( false ) ;
            }
            catch ( Exception e )
            {
                _logger.LogError ( e ,
                                   $"Failed to get settings for Id '{id}'" ) ;

                return null ;
            }
        }

        public Task < SettingsDto > GetDefaultSettings ( string id )
        {
            try
            {
                var dto = new SettingsDto ( ) ;

                return Task.FromResult ( dto ) ;
            }
            catch ( Exception e )
            {
                _logger.LogError ( e ,
                                   $"Failed to get default settings for Id '{id}'" ) ;

                return null ;
            }
        }

        public async Task < bool > InsertSettings ( SettingsDto dto )
        {
            try
            {
                await _client.SaveStateAsync ( StoreName ,
                                               dto.Id ,
                                               dto )
                             .ConfigureAwait ( false ) ;

                return false ;
            }
            catch ( Exception e )
            {
                _logger.LogError ( e ,
                                   $"Failed to insert settings '{dto}'" ) ;

                return false ;
            }
        }

        private const string StoreName = "statestore" ;

        private readonly DaprClient _client ;

        private readonly ILogger < SettingsRepository > _logger ;
    }
}