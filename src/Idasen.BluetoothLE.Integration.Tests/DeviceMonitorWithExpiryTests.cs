using System.Reactive.Concurrency;
using Autofac;
using Idasen.BluetoothLE.Interfaces;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Idasen.BluetoothLE.Integration.Tests
{
    [TestClass]
    public class DeviceMonitorWithExpiryTests
    {
        private IDeviceMonitorWithExpiry _sut;

        [TestInitialize]
        public void Initialize()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<BluetoothLEModule>();
            containerBuilder.RegisterType<TestScheduler>()
                            .As<IScheduler>()
                            .SingleInstance();

            var container = containerBuilder.Build();

            _sut = container.Resolve<IDeviceMonitorWithExpiry>();
        }

        // todo
    }
}