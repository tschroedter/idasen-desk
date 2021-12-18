using System.Reflection ;
using System.Threading.Tasks ;
using JetBrains.Annotations ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.RESTAPI
{
    public class Startup
    {
        [ UsedImplicitly ]
        private static Task < bool > DeskManager => DeskManagerRegistrations.DeskManager ;

        public void ConfigureServices ( IServiceCollection services )
        {
            services.AddAutoMapper ( Assembly.GetExecutingAssembly ( ) ) ;

            services.AddControllers ( ) ;
            services.AddHealthChecks ( )
                    .AddCheck < DeskManagerHealthCheck > ( "Desk Manager" ) ;

            services.AddSingleton ( c => CreateDeskManager ( true ) ) ;
        }

        public void Configure ( IApplicationBuilder app )
        {
            app.UseRouting ( ) ;
            app.UseHealthChecks ( "/health" ) ;
            app.UseEndpoints ( endpoints => { endpoints.MapControllers ( ) ; } ) ;
        }

        public IDeskManager CreateDeskManager ( bool fakeDeskManager = false )
        {
            return fakeDeskManager
                       ? DeskManagerRegistrations.CreateFakeDeskManager( )
                       : DeskManagerRegistrations.CreateRealDeskManager( ) ;
        }
    }
}