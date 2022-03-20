using Idasen.RESTAPI.Desk ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Idasen.RESTAPI.MicroService.Shared.RequestProcessing ;
using Idasen.RESTAPI.MicroService.Shared.Settings ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Logging.ClearProviders ( ) ;
// builder.Logging.AddConsole ( ) ;
builder.Logging.ClearProviders();
builder.Host.UseSerilog((_,
                          lc) => lc
                                 .WriteTo.Console()
                                 .WriteTo.File("logs/desk.log"));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
builder.Services.AddHostedService(serviceProvider =>
#pragma warning disable CS8604
                                        new StartupBackgroundService(serviceProvider.GetService<Serilog.ILogger>(),
                                                                     serviceProvider.GetService<IMicroServiceSettingsProvider>(),
                                                                     serviceProvider.GetService<StartupHealthCheck>()));
#pragma warning restore CS8604
builder.Services.AddSingleton<StartupHealthCheck>();
builder.Services.AddTransient<IMicroServiceSettingsProvider, MicroServiceSettingsProvider>();
builder.Services.AddTransient<ISettingsCaller, SettingsCaller>();
builder.Services.AddTransient<IMicroServiceCaller, MicroServiceCaller>();
builder.Services.AddTransient<IRequestForwarder, RequestForwarder>();
builder.Services.AddTransient<IMicroServiceSettingsDictionary, MicroServiceSettingsDictionary>();
builder.Services.AddTransient<IList<MicroServiceSettings>>(_ =>
                                                           {
                                                               return builder.Configuration
                                                                             .GetSection(nameof(MicroServicesSettings))
                                                                             .Get<IList<MicroServiceSettings>>(); ;
                                                           });
builder.Services.AddHealthChecks()
       .AddCheck<StartupHealthCheck>("Startup",
                                          tags: new[] { "ready" });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/healthz");
app.MapHealthChecks("/healthz/ready",
                      new HealthCheckOptions
                      {
                          Predicate = healthCheck => healthCheck.Tags.Contains("ready")
                      });
app.MapHealthChecks("/healthz/live",
                      new HealthCheckOptions
                      {
                          Predicate = _ => false
                      });

app.MapGet("/desk/",
             async httpContext =>
             {
                 await app.Services
                          .GetService<IRequestForwarder>()!
                          .Forward(httpContext);
             })
   .WithName("GetDesk");

app.MapGet("/Height/",
             async httpContext =>
             {
                 await app.Services
                          .GetService<IRequestForwarder>()!
                          .Forward(httpContext);
             })
   .WithName("GetHeight");

app.MapGet("/settings",
             async httpContext =>
             {
                 await new SettingsResponseCreator().Response(app.Services,
                                                                  httpContext);
             })
   .WithName("GetSettings");

app.Run();