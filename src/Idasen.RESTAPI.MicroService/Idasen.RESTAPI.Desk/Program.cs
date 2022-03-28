using Idasen.RESTAPI.MicroService.Shared.Extensions ;
using Idasen.RESTAPI.MicroService.Shared.Interfaces ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks ;
using Serilog ;

var builder = WebApplication.CreateBuilder ( args ) ;

// Add services to the container.
// builder.Logging.ClearProviders ( ) ;
// builder.Logging.AddConsole ( ) ;
builder.Logging.ClearProviders ( ) ;
builder.Host.UseSerilog ( ( _ ,
                            lc ) => lc
                                   .WriteTo.Console ( )
                                   .WriteTo.File ( "logs/desk.log" ) ) ;


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ( ) ;
builder.Services.AddSwaggerGen ( ) ;

builder.AddMicroServiceShared ( ) ;

var app = builder.Build ( ) ;

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwagger ( ) ;
    app.UseSwaggerUI ( ) ;
}

app.UseHttpsRedirection ( ) ;
app.MapHealthChecks ( "/healthz" ) ;
app.MapHealthChecks ( "/healthz/ready" ,
                      new HealthCheckOptions
                      {
                          Predicate = healthCheck => healthCheck.Tags.Contains ( "ready" )
                      } ) ;
app.MapHealthChecks ( "/healthz/live" ,
                      new HealthCheckOptions
                      {
                          Predicate = _ => false
                      } ) ;

app.MapGet ( "/desk/" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService < IRequestForwarder > ( )!
                          .Forward ( httpContext ) ;
             } )
   .WithName ( "GetDesk" ) ;

app.MapGet ( "/desk/height/" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService < IRequestForwarder > ( )!
                          .Forward ( httpContext ) ;
             } )
   .WithName ( "GetDeskHeight" ) ;

app.MapGet ( "/settings" ,
             async httpContext =>
             {
                 await app.Services
                          .GetService <ISettingsResponseCreator> (  )!
                          .Response ( app.Services ,
                                      httpContext ) ;
             } )
   .WithName ( "GetSettings" ) ;

app.Run ( ) ;