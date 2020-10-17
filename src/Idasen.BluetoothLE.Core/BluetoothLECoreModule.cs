using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Autofac;
using Idasen.BluetoothLE.Core.DevicesDiscovery;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.ServicesDiscovery;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.Interfaces;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Device = Idasen.BluetoothLE.Core.DevicesDiscovery.Device;
using DeviceFactory = Idasen.BluetoothLE.Core.DevicesDiscovery.DeviceFactory;
using IDevice = Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery.IDevice;
using IDeviceFactory = Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery.IDeviceFactory;

namespace Idasen.BluetoothLE.Core
{
    // ReSharper disable once InconsistentNaming
    [ExcludeFromCodeCoverage]
    public class BluetoothLECoreModule
        : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Subject<>))
                   .As(typeof(ISubject<>));

            builder.RegisterInstance(TaskPoolScheduler.Default)
                   .As<IScheduler>();

            builder.RegisterType<ObservableTimerFactory>()
                   .As<IObservableTimerFactory>();

            builder.RegisterType<DateTimeOffsetWrapper>()
                   .As<IDateTimeOffset>();

            builder.RegisterType<DeviceComparer>()
                   .As<IDeviceComparer>();
            builder.RegisterType<Device>()
                   .As<IDevice>();
            builder.RegisterType<DeviceMonitor>()
                   .As<IDeviceMonitor>();
            builder.RegisterType<DeviceMonitorWithExpiry>()
                   .As<IDeviceMonitorWithExpiry>();
            builder.RegisterType<Devices>()
                   .As<IDevices>();
            builder.RegisterType<Watcher>()
                   .As<IWatcher>();
            builder.RegisterType<Wrapper>()
                   .As<IWrapper>();
            builder.RegisterType<DeviceFactory>()
                   .As<IDeviceFactory>();
            builder.RegisterType<StatusMapper>()
                   .As<IStatusMapper>();

            builder.RegisterType<GattCharacteristicValueChangedObservables>()
                   .As<IGattCharacteristicValueChangedObservables>();
            builder.RegisterType<GattCharacteristicWrapper>()
                   .As<IGattCharacteristicWrapper>();
            builder.RegisterType<GattCharacteristicWrapperFactory>()
                   .As<IGattCharacteristicWrapperFactory>();

            builder.RegisterType<MatchMaker>()
                   .As<IMatchMaker>();
            builder.RegisterType<OfficialGattServices>()
                   .As<IOfficialGattServices>();
            builder.RegisterType<GattServicesDictionary>()
                   .As<IGattServicesDictionary>();
            builder.RegisterType<GattServicesProvider>()
                   .As<IGattServicesProvider>();
            builder.RegisterType<GattServicesProviderFactory>()
                   .As<IGattServicesProviderFactory>();
            builder.RegisterType<Core.ServicesDiscovery.Device>()
                   .As<BluetoothLE.Core.Interfaces.ServicesDiscovery.IDevice>();
            builder.RegisterType<Core.ServicesDiscovery.DeviceFactory>()
                   .As<BluetoothLE.Core.Interfaces.ServicesDiscovery.IDeviceFactory>();

            builder.RegisterType<BluetoothLeDeviceWrapper>()
                   .As<IBluetoothLeDeviceWrapper>();
            builder.RegisterType<BluetoothLeDeviceWrapperFactory>()
                   .As<IBluetoothLeDeviceWrapperFactory>();
            builder.RegisterType<GattDeviceServicesResultWrapper>()
                   .As<IGattDeviceServicesResultWrapper>();
            builder.RegisterType<GattDeviceServicesResultWrapperFactory>()
                   .As<IGattDeviceServicesResultWrapperFactory>();

            builder.RegisterType<GattCharacteristicsResultWrapper>()
                   .As<IGattCharacteristicsResultWrapper>();
            builder.RegisterType<GattCharacteristicsResultWrapperFactory>()
                   .As<IGattCharacteristicsResultWrapperFactory>();
        }
    }
}