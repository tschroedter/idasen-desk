using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Subjects ;
using Autofac ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Device = Idasen.BluetoothLE.Core.DevicesDiscovery.Device ;
using DeviceFactory = Idasen.BluetoothLE.Core.DevicesDiscovery.DeviceFactory ;
using IDevice = Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery.IDevice ;
using IDeviceFactory = Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery.IDeviceFactory ;

namespace Idasen.BluetoothLE.Core
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class BluetoothLECoreModule
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterModule < BluetoothLEAop > ( ) ;

            builder.RegisterGeneric ( typeof ( Subject <> ) )
                   .As ( typeof ( ISubject <> ) ) ;

            builder.RegisterInstance ( TaskPoolScheduler.Default )
                   .As < IScheduler > ( ) ;

            builder.RegisterType < ObservableTimerFactory > ( )
                   .As < IObservableTimerFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < DateTimeOffsetWrapper > ( )
                   .As < IDateTimeOffset > ( ) ;

            builder.RegisterType < DeviceComparer > ( )
                   .As < IDeviceComparer > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Device > ( )
                   .As < IDevice > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < DeviceMonitor > ( )
                   .As < IDeviceMonitor > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < DeviceMonitorWithExpiry > ( )
                   .As < IDeviceMonitorWithExpiry > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Devices > ( )
                   .As < IDevices > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Watcher > ( )
                   .As < IWatcher > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Wrapper > ( )
                   .As < IWrapper > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < DeviceFactory > ( )
                   .As < IDeviceFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < StatusMapper > ( )
                   .As < IStatusMapper > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicValueChangedObservables > ( )
                   .As < IGattCharacteristicValueChangedObservables > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicWrapper > ( )
                   .As < IGattCharacteristicWrapper > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicWrapperFactory > ( )
                   .As < IGattCharacteristicWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < MatchMaker > ( )
                   .As < IMatchMaker > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < OfficialGattServices > ( )
                   .As < IOfficialGattServices > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattServicesDictionary > ( )
                   .As < IGattServicesDictionary > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattServicesProvider > ( )
                   .As < IGattServicesProvider > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattServicesProviderFactory > ( )
                   .As < IGattServicesProviderFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < ServicesDiscovery.Device > ( )
                   .As < Interfaces.ServicesDiscovery.IDevice > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < ServicesDiscovery.DeviceFactory > ( )
                   .As < Interfaces.ServicesDiscovery.IDeviceFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < BluetoothLeDeviceWrapper > ( )
                   .As < IBluetoothLeDeviceWrapper > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < BluetoothLeDeviceWrapperFactory > ( )
                   .As < IBluetoothLeDeviceWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattDeviceServicesResultWrapper > ( )
                   .As < IGattDeviceServicesResultWrapper > ( ) ;

            builder.RegisterType < GattDeviceServicesResultWrapperFactory > ( )
                   .As < IGattDeviceServicesResultWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicsResultWrapper > ( )
                   .As < IGattCharacteristicsResultWrapper > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicsResultWrapperFactory > ( )
                   .As < IGattCharacteristicsResultWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattReadResultWrapper > ( )
                   .As < IGattReadResultWrapper > ( ) ;

            builder.RegisterType < GattReadResultWrapperFactory > ( )
                   .As < IGatReadResultWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattWriteResultWrapper > ( )
                   .As < IGattWriteResultWrapper > ( ) ;

            builder.RegisterType < GattWriteResultWrapperFactory > ( )
                   .As < IGattWriteResultWrapperFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;
        }
    }
}