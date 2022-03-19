using Idasen.RESTAPI.MicroService.Desk ;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.ClearProviders();
builder.Host.UseSerilog((_,
                         lc) => lc
                               .WriteTo.Console()
                               .WriteTo.File("logs/desk.log"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// see https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0
builder.Services.AddHostedService<StartupBackgroundService>();
builder.Services.AddSingleton<StartupHealthCheck>();
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

app.MapGet("/desk",
           () => "ToDo: /desk")
   .WithName("GetDesk");

app.Run();