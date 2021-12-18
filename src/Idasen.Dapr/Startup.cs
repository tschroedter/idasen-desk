using System.Reflection ;
using System.Threading.Tasks ;
using Autofac ;
using Idasen.Launcher ;
using JetBrains.Annotations ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.Dapr
{
    public class Startup
    {
        [ UsedImplicitly ] private static Task < bool > DeskManager ;

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
                       ? CreateFakeDeskManager ( )
                       : CreateRealDeskManager ( ) ;
        }

        private IDeskManager CreateFakeDeskManager ( )
        {
            return new FakeDeskManager ( ) ;
        }

        private static IDeskManager CreateRealDeskManager ( )
        {
            ContainerProvider.Builder.RegisterModule ( new IdasenDaprModule ( ) ) ;

            var container = ContainerProvider.Create ( "Idasen.ConsoleDapr" ,
                                                       "Idasen.ConsoleDapr.log" ) ;

            var manager = container.Resolve < IDeskManager > ( ) ;

            while ( ! ( DeskManager is { Result: true } ) )
            {
                DeskManager = Task.Run ( async ( ) => await manager.Initialise ( )
                                                                   .ConfigureAwait ( false ) ) ;
            }

            return manager ;
        }
    }
}