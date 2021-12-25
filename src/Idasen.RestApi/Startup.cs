using System.Reflection ;
using FluentValidation.AspNetCore ;
using Idasen.RESTAPI.Desks ;
using Idasen.RESTAPI.Filters ;
using Idasen.RESTAPI.Interfaces ;
using Microsoft.AspNetCore.Builder ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;

namespace Idasen.RESTAPI
{
    public class Startup
    {
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

            services.AddTransient < ISettingsRepository , SettingsRepository > ( ) ;

            // todo the flag UseFakeDeskManager will come form settings or config file
            // ReSharper disable once RedundantArgumentDefaultValue
            services.AddSingleton ( c => CreateDeskManager ( ) ) ;
        }

        public void Configure ( IApplicationBuilder builder )
        {
            builder.UseRouting ( ) ;
            builder.UseHealthChecks ( "/health" ) ;
            builder.UseEndpoints ( endpoints => { endpoints.MapControllers ( ) ; } ) ;
        }

        public IDeskManager CreateDeskManager ( )
        {
            var useFake = GetUseFakeDeskManager ( ) ;

            return useFake
                       ? DeskManagerRegistrations.CreateFakeDeskManager ( )
                       : DeskManagerRegistrations.CreateRealDeskManager ( ) ;
        }

        private static bool GetUseFakeDeskManager()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json",
                                                                 optional: true,
                                                                 reloadOnChange: true);

            var configuration = builder.Build();

            return configuration.GetValue<bool>("use-fake-desk-manager");
        }
    }
}