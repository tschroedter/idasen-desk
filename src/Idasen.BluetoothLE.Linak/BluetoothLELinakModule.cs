using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using Autofac ;
using Idasen.BluetoothLE.Characteristics ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.BluetoothLE.Linak
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class BluetoothLELinakModule
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterModule < BluetoothLEDeskModule > ( ) ;

            var scheduler = TaskPoolScheduler.Default ;

            builder.RegisterInstance ( scheduler )
                   .As < IScheduler > ( ) ;

            builder.RegisterType < DeskCharacteristics > ( )
                   .As < IDeskCharacteristics > ( ) ;

            builder.RegisterType < Desk > ( )
                   .As < IDesk > ( ) ;

            builder.RegisterType < DeskFactory > ( )
                   .As < IDeskFactory > ( ) ;

            builder.RegisterType < DeskHeightAndSpeed > ( )
                   .As < IDeskHeightAndSpeed > ( ) ;

            builder.RegisterType < DeskHeightAndSpeedFactory > ( )
                   .As < IDeskHeightAndSpeedFactory > ( ) ;

            builder.RegisterType < RawValueToHeightAndSpeedConverter > ( )
                   .As < IRawValueToHeightAndSpeedConverter > ( ) ;

            builder.RegisterType < DeskCommandExecutor > ( )
                   .As < IDeskCommandExecutor > ( ) ;

            builder.RegisterType < DeskCommandExecutorFactory > ( )
                   .As < IDeskCommandExecutorFactory > ( ) ;

            builder.RegisterType < DeskCommandsProvider > ( )
                   .As < IDeskCommandsProvider > ( ) ;

            builder.RegisterType < DeskMover > ( )
                   .As < IDeskMover > ( ) ;

            builder.RegisterType < DeskMoverFactory > ( )
                   .As < IDeskMoverFactory > ( ) ;

            builder.RegisterType < RawValueChangedDetailsCollector > ( )
                   .As < IRawValueChangedDetailsCollector > ( ) ;

            builder.RegisterType < StoppingHeightCalculator > ( )
                   .As < IStoppingHeightCalculator > ( ) ;

            builder.RegisterType < HasReachedTargetHeightCalculator > ( )
                   .As < IHasReachedTargetHeightCalculator > ( ) ;

            builder.RegisterType < InitialHeightProvider > ( )
                   .As < IInitialHeightProvider > ( ) ;

            builder.RegisterType < InitialHeightAndSpeedProviderFactory > ( )
                   .As < IInitialHeightAndSpeedProviderFactory > ( ) ;

            builder.RegisterType < DeskCharacteristicsCreator > ( )
                   .As < IDeskCharacteristicsCreator > ( ) ;

            builder.RegisterType < DeskConnector > ( )
                   .As < IDeskConnector > ( ) ;

            builder.RegisterType<DeskDetector>()
                   .As<IDeskDetector>();

            builder.RegisterType<DeskProvider>()
                   .As<IDeskProvider>();
        }
    }
}