using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using FluentAssertions;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.ServicesDiscovery;
using NSubstitute;
using Selkie.AutoMocking;
using Serilog;

namespace Idasen.BluetoothLE.Tests.ServicesDiscovery
{
    [AutoDataTestClass]
    public class GattServicesProviderTests
    {
        [AutoDataTestMethod]
        public void Constructor_ForLoggerNull_Throws(
            Lazy<GattServicesProvider> sut,
            [BeNull] ILogger           logger)
        {
            // ReSharper disable once UnusedVariable
            Action action = () =>
                            {
                                var test = sut.Value;
                            };

            action.Should()
                  .Throw<ArgumentNullException>()
                  .WithParameter(nameof(logger));
        }

        [AutoDataTestMethod]
        public void Constructor_ForServicesNull_Throws(
            Lazy<GattServicesProvider>       sut,
            [BeNull] IGattServicesDictionary services)
        {
            // ReSharper disable once UnusedVariable
            Action action = () =>
                            {
                                var test = sut.Value;
                            };

            action.Should()
                  .Throw<ArgumentNullException>()
                  .WithParameter(nameof(services));
        }

        [AutoDataTestMethod]
        public void Constructor_ForRefreshedNull_Throws(
            Lazy<GattServicesProvider>                 sut,
            [BeNull] ISubject<GattCommunicationStatus> refreshed)
        {
            // ReSharper disable once UnusedVariable
            Action action = () =>
                            {
                                var test = sut.Value;
                            };

            action.Should()
                  .Throw<ArgumentNullException>()
                  .WithParameter(nameof(refreshed));
        }

        [AutoDataTestMethod]
        public void Constructor_ForDeviceNull_Throws(
            Lazy<GattServicesProvider>         sut,
            [BeNull] IBluetoothLeDeviceWrapper device)
        {
            // ReSharper disable once UnusedVariable
            Action action = () =>
                            {
                                var test = sut.Value;
                            };

            action.Should()
                  .Throw<ArgumentNullException>()
                  .WithParameter(nameof(device));
        }

        [AutoDataTestMethod]
        public void GattCommunicationStatus_ForGattResultIsNull_Unreachable(
            GattServicesProvider sut)
        {
            sut.GattCommunicationStatus
               .Should()
               .Be(GattCommunicationStatus.Unreachable);
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForDisconnected_SetsGattCommunicationStatusUnreachable(
            GattServicesProvider               sut,
            [Freeze] IBluetoothLeDeviceWrapper device)
        {
            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Disconnected);

            await sut.Refresh();

            sut.GattCommunicationStatus
               .Should()
               .Be(GattCommunicationStatus.Unreachable);
        }

        [AutoDataTestMethod]
        public async Task GattCommunicationStatus_ForConnectedAndServicesAvailable_Success(
            GattServicesProvider               sut,
            [Freeze] IBluetoothLeDeviceWrapper device,
            IGattDeviceServicesResultWrapper   resultWrapper)
        {
            resultWrapper.Status
                         .Returns(GattCommunicationStatus.Success);

            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Connected);

            device.GetGattServicesAsync()
                  .Returns(resultWrapper);

            await sut.Refresh();

            sut.GattCommunicationStatus
               .Should()
               .Be(resultWrapper.Status);
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForDisconnected_Notifies(
            GattServicesProvider                       sut,
            [Freeze] IBluetoothLeDeviceWrapper         device,
            [Freeze] ISubject<GattCommunicationStatus> refreshed)
        {
            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Disconnected);

            await sut.Refresh();

            refreshed.Received()
                     .OnNext(GattCommunicationStatus.Unreachable);
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForConnected_SetsGattCommunicationStatusUnreachable(
            GattServicesProvider               sut,
            [Freeze] IBluetoothLeDeviceWrapper device,
            IGattDeviceServicesResultWrapper   result)
        {
            result.Status
                  .Returns(GattCommunicationStatus.Unreachable);

            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Connected);

            device.GetGattServicesAsync()
                  .Returns(Task.FromResult(result));

            await sut.Refresh();

            sut.GattCommunicationStatus
               .Should()
               .Be(GattCommunicationStatus.Unreachable);
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForInvoked_ClearsServices(
            GattServicesProvider             sut,
            [Freeze] IGattServicesDictionary services)
        {
            await sut.Refresh();

            services.Received()
                    .Clear();
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForConnected_Notifies(
            GattServicesProvider                       sut,
            [Freeze] IBluetoothLeDeviceWrapper         device,
            [Freeze] ISubject<GattCommunicationStatus> refreshed,
            IGattDeviceServicesResultWrapper           result)
        {
            var expected = GattCommunicationStatus.ProtocolError;

            result.Status
                  .Returns(expected);

            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Connected);

            device.GetGattServicesAsync()
                  .Returns(Task.FromResult(result));

            await sut.Refresh();

            refreshed.Received()
                     .OnNext(expected);
        }

        [AutoDataTestMethod]
        public async Task Refresh_ForConnectedAndCharacteristicsSuccess_AddsService(
            GattServicesProvider                       sut,
            [Freeze] IBluetoothLeDeviceWrapper         device,
            [Freeze] ISubject<GattCommunicationStatus> refreshed,
            [Freeze] IGattServicesDictionary           services,
            IGattDeviceServicesResultWrapper           result,
            IGattDeviceServiceWrapper                  service,
            IGattCharacteristicsResultWrapper          characteristics)
        {
            result.Status
                  .Returns(GattCommunicationStatus.Success);

            result.Services
                  .Returns(new[] {service});

            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Connected);

            device.GetGattServicesAsync()
                  .Returns(Task.FromResult(result));

            characteristics.Status
                           .Returns(GattCommunicationStatus.Success);

            service.GetCharacteristicsAsync()
                   .Returns(characteristics);

            await sut.Refresh();

            services[service]
               .Should()
               .Be(characteristics);
        }


        [AutoDataTestMethod]
        public async Task Refresh_ForConnectedAndCharacteristicsUnreachable_DoesNotAddService(
            GattServicesProvider               sut,
            [Freeze] IBluetoothLeDeviceWrapper device,
            [Freeze] IGattServicesDictionary   services,
            IGattDeviceServicesResultWrapper   result,
            IGattDeviceServiceWrapper          service,
            IGattCharacteristicsResultWrapper  characteristics)
        {
            result.Status
                  .Returns(GattCommunicationStatus.Success);

            result.Services
                  .Returns(new[] {service});

            device.ConnectionStatus
                  .Returns(BluetoothConnectionStatus.Connected);

            device.GetGattServicesAsync()
                  .Returns(Task.FromResult(result));

            characteristics.Status
                           .Returns(GattCommunicationStatus.Unreachable);

            service.GetCharacteristicsAsync()
                   .Returns(characteristics);

            await sut.Refresh();

            services[service]
               .Should()
               .NotBe(characteristics);
        }

        // todo integration tests
    }
}