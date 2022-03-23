using System.Reactive.Concurrency ;
using Idasen.RESTAPI.Desk.Emulator.Desks ;
using Idasen.RESTAPI.Desk.Emulator.Idasen ;
using Idasen.RESTAPI.Desk.Emulator.Interfaces ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks ;

var builder = WebApplication.CreateBuilder ( args ) ;

// Add services to the container.

builder.Services.AddControllers ( ) ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer ( ) ;
builder.Services.AddSwaggerGen ( ) ;
builder.Services.AddSingleton < IRestDesk , RestDesk > ( ) ;
builder.Services.AddSingleton < IFakeDesk , FakeDesk > ( ) ;
builder.Services.AddSingleton < IScheduler > ( TaskPoolScheduler.Default ) ;
builder.Services.AddHealthChecks();

var app = builder.Build ( ) ;

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

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment ( ) )
{
    app.UseSwagger ( ) ;
    app.UseSwaggerUI ( ) ;
}

app.UseHttpsRedirection ( ) ;

app.UseAuthorization ( ) ;

app.MapControllers ( ) ;

app.Run ( ) ;