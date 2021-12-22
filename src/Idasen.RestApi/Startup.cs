using System.Reflection ;
using FluentValidation.AspNetCore ;
using Idasen.RESTAPI.Desks ;
using Idasen.RESTAPI.Filters ;
using Idasen.RESTAPI.Interfaces ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.RESTAPI
{
    public class Startup
    {
        private const bool UseFakeDeskManager = true ;

        public void ConfigureServices ( IServiceCollection services )
        {
            services.AddAutoMapper ( Assembly.GetExecutingAssembly ( ) ) ;

            services.AddControllers ( options => { options.Filters.Add ( new ValidationFilter ( ) ) ; } ) ;

            services.AddFluentValidation ( options =>
                                           {
                                               options.RegisterValidatorsFromAssemblyContaining < Startup > ( ) ;
                                           } ) ;

            services.AddHealthChecks ( )
                    .AddCheck < DeskManagerHealthCheck > ( "Desk Manager" ) ;

            // todo the flag UseFakeDeskManager will come form settings or config file
            // ReSharper disable once RedundantArgumentDefaultValue
            services.AddSingleton ( c => CreateDeskManager ( UseFakeDeskManager ) ) ;
        }

        public void Configure ( IApplicationBuilder builder )
        {
            builder.UseRouting ( ) ;
            builder.UseHealthChecks ( "/health" ) ;
            builder.UseEndpoints ( endpoints => { endpoints.MapControllers ( ) ; } ) ;
        }

        public IDeskManager CreateDeskManager ( bool fakeDeskManager = false )
        {
            return fakeDeskManager
                       ? DeskManagerRegistrations.CreateFakeDeskManager ( )
                       : DeskManagerRegistrations.CreateRealDeskManager ( ) ;
        }
    }
}