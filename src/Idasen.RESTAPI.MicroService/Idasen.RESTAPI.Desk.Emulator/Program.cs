using System.Reactive.Concurrency ;
using Idasen.RESTAPI.Desk.Emulator.Desks ;
using Idasen.RESTAPI.Desk.Emulator.Idasen ;
using Idasen.RESTAPI.Desk.Emulator.Interfaces ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton < IRestDesk , RestDesk > ( ) ;
builder.Services.AddSingleton < IFakeDesk , FakeDesk > ( ) ;
builder.Services.AddSingleton < IScheduler > ( TaskPoolScheduler.Default ) ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();