using System;
using System.Reactive.Subjects;
using FluentAssertions;
using Idasen.BluetoothLE.Common.Tests;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery;
using Idasen.BluetoothLE.Linak.Interfaces;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [TestClass]
    public class DeskDetectorTests
    {
        private const string DeviceName = nameof(DeviceName);
        private const uint DeviceAddress = 123;
        private const uint DeviceTimeout = 456;

        private ILogger _logger;
        private TestScheduler _scheduler;
        private IDeviceMonitorWithExpiry _monitor;
        private IDeskFactory _factory;
        private ISubject<IDesk> _deskDetected;
        private ISubject<IDevice> _updated;
        private Subject<IDevice> _discovered;
        private Subject<IDevice> _nameChanged;
        private Subject<IDevice> _deskFound;
        private IDesk _desk;
        private IDevice _device;
        private IDesk _deskOther;

        [TestInitialize]
        public void Initialize()
        {
            _logger = Substitute.For<ILogger>();
            _scheduler = new TestScheduler();
            _monitor = Substitute.For<IDeviceMonitorWithExpiry>();
            _factory = Substitute.For<IDeskFactory>();
            _deskDetected = new Subject<IDesk>();

            _updated = new Subject<IDevice>();
            _monitor.DeviceUpdated.Returns(_updated);

            _discovered = new Subject<IDevice>();
            _monitor.DeviceDiscovered.Returns(_discovered);

            _nameChanged = new Subject<IDevice>();
            _monitor.DeviceNameUpdated.Returns(_discovered);

            _deskFound = new Subject<IDevice>();
            _monitor.DeviceUpdated.Returns(_deskFound);

            _device = Substitute.For<IDevice>();
            _device.Name.Returns(DeviceName);
            _device.Address.Returns(DeviceAddress);

            _desk = Substitute.For<IDesk>();
            _deskOther = Substitute.For<IDesk>();
            _factory.CreateAsync(_device.Address)
                .Returns(_desk, _deskOther);
        }

        [TestMethod]
        public void Initialize_ForDeviceNameIsNull_Throws()
        {
            Action action = () => CreateSut().Initialize(null!,
                    DeviceAddress,
                    DeviceTimeout);

            action.Should()
                .Throw<ArgumentNullException>()
                .WithParameter("deviceName");
        }

        [TestMethod]
        public void Initialize_Invoked_SetsTimeout()
        {
            CreateSut().Initialize(DeviceName,
                DeviceAddress,
                DeviceTimeout);

            _monitor.TimeOut
                .Should()
                .Be(TimeSpan.FromSeconds (DeviceTimeout));
        }

        [TestMethod]
        public void Start_ConnectedToAnotherDesk_DisposesOldDesk()
        {
            var sut = CreateSut().Initialize(DeviceName,
                DeviceAddress,
                DeviceTimeout);

            // connect to desk
            sut.Start();

            _discovered.OnNext(_device);

            _scheduler.Start();

            // connect to desk again, so that the old one is disposed
            sut.Start();

            _desk.Received()
                .Dispose();
        }

        [TestMethod]
        public void Dispose_Invoked_DisposesMonitor()
        {
            var sut = CreateSut();

            sut.Dispose();

            _monitor.Received()
                .Dispose();
        }

        // todo figure out how to test disposing of IDisposables of Subjects
        // todo improve code coverage

        private DeskDetector CreateSut()
        {
            return new DeskDetector(_logger,
                _scheduler,
                _monitor,
                _factory,
                _deskDetected);
        }

    }
}