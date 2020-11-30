using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Autofac ;
using AutofacSerilogIntegration ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Serilog ;
using Serilog.Events ;

namespace Idasen.ConsoleApp
{
    public class Demo
        : IDemo
    {
        public Demo ( )
        {
            const string template =
                "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message}{NewLine}{Exception}" ;
            // "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration ( )
                        .Enrich.WithCaller ( )
                        .MinimumLevel.Information ( )
                        .WriteTo
                        .ColoredConsole ( LogEventLevel.Debug ,
                                          template )
                        .CreateLogger ( ) ;

            var builder = new ContainerBuilder ( ) ;

            builder.RegisterLogger ( ) ;
            builder.RegisterModule < BluetoothLECoreModule > ( ) ;
            builder.RegisterModule < BluetoothLELinakModule > ( ) ;

            _container = builder.Build ( ) ;

            _logger       = _container.Resolve < ILogger > ( ) ;
            _deskDetector = _container.Resolve < IDeskDetector > ( ) ;
        }

        /// <inheritdoc />
        public IDemo Initialize ( )
        {
            _logger.Information ("Initialize...");

            _deskDetector.Initialize (  );

            _deskDetected = _deskDetector.DeskDetected
                                         .ObserveOn ( _container.Resolve < IScheduler > ( ) )
                                         .Subscribe ( OnDeskDetected ) ;

            return this ;
        }

        /// <inheritdoc />
        public IDemo Detect ( )
        {
            _logger.Information("Trying to detect desk...");

            _deskDetector.Start (  );

            return this ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _deskDetected?.Dispose ( ) ;
            _deskDetector?.Dispose ( ) ;
            _container?.Dispose ( ) ;
        }

        /// <inheritdoc />
        public void OnDeskDetected ( IDesk desk )
        {
            _logger.Debug ( "Detected desk " ) ; // todo '{desk.Name}' with address {desk.Address} " );
            _desk = desk ;

            _deskDetector.Stop();

            _desk.MoveTo ( 7200u ) ;
        }

        private readonly IContainer    _container ;
        private          IDisposable   _deskDetected ;
        private readonly IDeskDetector _deskDetector ;
        private readonly ILogger       _logger ;

        private IDesk _desk ;
    }

    public interface IDemo
    : IDisposable
    {
        /// <inheritdoc />
        IDemo Detect ( ) ;

        /// <inheritdoc />
        IDemo Initialize ( ) ;
    }
}