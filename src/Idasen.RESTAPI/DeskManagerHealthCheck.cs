using System ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.RESTAPI.Interfaces ;
using JetBrains.Annotations ;
using Microsoft.Extensions.Diagnostics.HealthChecks ;

namespace Idasen.RESTAPI
{
    public class DeskManagerHealthCheck : IHealthCheck
    {
        public DeskManagerHealthCheck ( [ NotNull ] IDeskManager manager )
        {
            Guard.ArgumentNotNull ( manager ,
                                    nameof ( manager ) ) ;

            _manager = manager ;
        }

        public Task < HealthCheckResult > CheckHealthAsync ( HealthCheckContext context ,
                                                             CancellationToken  cancellationToken = default )
        {
            try
            {
                return _manager.IsReady
                           ? Task.FromResult ( HealthCheckResult.Healthy ( ) )
                           : Task.FromResult ( HealthCheckResult.Unhealthy ( ) ) ;
            }
            catch ( Exception ex )
            {
                return Task.FromResult ( HealthCheckResult.Unhealthy ( ex.Message ) ) ;
            }
        }

        private readonly IDeskManager _manager ;
    }
}