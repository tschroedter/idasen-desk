using System.Reflection ;
using System.Threading.Tasks ;
using FluentValidation.AspNetCore ;
using Idasen.RESTAPI.Desks ;
using Idasen.RESTAPI.Dtos.Validators ;
using Idasen.RESTAPI.Interfaces ;
using JetBrains.Annotations ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.RESTAPI
{
    public class Startup
    {
        private const bool UseFakeDeskManager = true ;

        [ UsedImplicitly ]
        private static Task < bool > DeskManager => DeskManagerRegistrations.DeskManager ;

        public void ConfigureServices ( IServiceCollection services )
        {
            services.AddAutoMapper ( Assembly.GetExecutingAssembly ( ) ) ;

            services.AddControllers ( options =>
                                      {
                                          options.Filters.Add ( new ValidationFilter ( ) ) ;
                                      } );

            services.AddFluentValidation ( options =>
                                           {
                                               options.RegisterValidatorsFromAssemblyContaining< Startup > ( ) ;
                                           } ) ;

            services.AddHealthChecks ( )
                    .AddCheck < DeskManagerHealthCheck > ( "Desk Manager" ) ;

            // todo the flag UseFakeDeskManager will come form settings or config file
            // ReSharper disable once RedundantArgumentDefaultValue
            services.AddSingleton ( c => CreateDeskManager (UseFakeDeskManager) ) ;
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