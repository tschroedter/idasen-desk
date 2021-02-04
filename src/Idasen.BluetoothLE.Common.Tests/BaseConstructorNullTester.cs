using Autofac ;
using AutofacSerilogIntegration ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using Selkie.DefCon.One ;
using Selkie.DefCon.One.Common ;
using Selkie.DefCon.One.Interfaces ;
using Serilog ;
using Serilog.Events ;
using Serilog.Sinks.SystemConsole.Themes ;

namespace Idasen.BluetoothLE.Common.Tests
{
    public abstract class BaseConstructorNullTester < T > where T : class
    {
        public virtual int NumberOfConstructorsPassed { get ; } = 1 ;
        public virtual int NumberOfConstructorsFailed { get ; } = 0 ;

        [ TestCleanup ]
        public virtual void Cleanup ( )
        {
            _container.Dispose ( ) ;
        }


        [ TestInitialize ]
        public virtual void Initialize ( )
        {
            const string template = "[{Timestamp:HH:mm:ss.ffff} " +
                                    "{Level:u3}] "                +
                                    "{Message}\r\n" ;

            Log.Logger = new LoggerConfiguration ( )
                        .Enrich.WithCaller ( )
                        .MinimumLevel.Information ( )
                        .WriteTo.Console ( LogEventLevel.Debug ,
                                           template ,
                                           theme : AnsiConsoleTheme.Code )
                        .CreateLogger ( ) ;

            var builder = new ContainerBuilder ( ) ;

            builder.RegisterLogger ( ) ;
            builder.RegisterModule < DefConOneModule > ( ) ;

            _container = builder.Build ( ) ;
        }

        [ TestMethod ]
        public virtual void Constructor_ForAnyParameterNullThrows_AllPassing ( )
        {
            var tester = CreateTester ( ) ;

            tester.Test < T > ( ) ;

            using ( new AssertionScope ( ) )
            {
                tester.HasPassed
                      .Should ( )
                      .BeTrue ( "Has Passed" ) ;

                tester.ConstructorsToTest
                      .Should ( )
                      .Be ( NumberOfConstructorsPassed ,
                            "ConstructorsToTest" ) ;

                tester.ConstructorsFailed
                      .Should ( )
                      .Be ( NumberOfConstructorsFailed ,
                            "ConstructorsFailed" ) ;
            }
        }

        protected virtual INotNullTester CreateTester ( )
        {
            return _container.Resolve < INotNullTester > ( ) ;
        }

        private IContainer _container ;
    }
}