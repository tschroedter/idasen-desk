using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using FluentAssertions;
using Idasen.BluetoothLE.Common.Tests;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery;
using Idasen.BluetoothLE.Linak.Interfaces;
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
        private IScheduler _scheduler;
        private IDeviceMonitorWithExpiry _monitor;
        private IDeskFactory _factory;
        private ISubject<IDesk> _deskDetected;
        private ISubject<IDevice> _updated;
        private Subject<IDevice> _discovered;
        private Subject<IDevice> _nameChanged;
        private Subject<IDevice> _deskFound;
        private IDesk _desk;

        [TestInitialize]
        public void Initialize()
        {
            _logger = Substitute.For<ILogger>();
            _scheduler = Substitute.For<IScheduler>();
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

            _desk = Substitute.For<IDesk>();
        }

        [TestMethod]
        public void Initialize_ForDeviceNameIsNull_Throws()
        {
            Action action = () => CreateSut().Initialize(null,
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
        public void Start_ConnectedToDesk_DisposesDesk()
        {
            CreateSut().Initialize(DeviceName,
                DeviceAddress,
                DeviceTimeout);

            // todo connect to desk

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

        [TestMethod]
        public void Dispose_Invoked_DisposesUpdated()
        {
            // todo
            var disposable = Substitute.For<IDisposable>();
            _updated.Subscribe(Arg.Any<Action<IDevice>>())
                .Returns(disposable);

            var sut = CreateSut();

            sut.Dispose();

            disposable.Received()
                .Dispose();
        }

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