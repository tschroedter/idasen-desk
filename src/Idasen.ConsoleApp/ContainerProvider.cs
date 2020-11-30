using Autofac ;
using AutofacSerilogIntegration ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Serilog ;
using Serilog.Events ;

namespace Idasen.ConsoleApp
{
    public static class ContainerProvider
    {
        public static IContainer Create()
        {
            const string template =
                "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message}{NewLine}{Exception}";
            // "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                        .Enrich.WithCaller()
                        .MinimumLevel.Information()
                        .WriteTo
                        .ColoredConsole(LogEventLevel.Debug,
                                        template)
                        .CreateLogger();

            var builder = new ContainerBuilder();

            builder.RegisterLogger();
            builder.RegisterModule<BluetoothLECoreModule>();
            builder.RegisterModule<BluetoothLELinakModule>();

            return builder.Build();
        }
    }
}