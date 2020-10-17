using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <inheritdoc />
    public class GattServicesProvider
        : IGattServicesProvider
    {
        public delegate IGattServicesProvider Factory([NotNull] IBluetoothLeDeviceWrapper device);

        private readonly IBluetoothLeDeviceWrapper         _device;
        private readonly ILogger                           _logger;
        private readonly ISubject<GattCommunicationStatus> _refreshed;
        private readonly IGattServicesDictionary           _services;
        private          IGattDeviceServicesResultWrapper  _gattResult;

        public GattServicesProvider([NotNull] ILogger                           logger,
                                    [NotNull] IGattServicesDictionary           services,
                                    [NotNull] ISubject<GattCommunicationStatus> refreshed,
                                    [NotNull] IBluetoothLeDeviceWrapper         device)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(services,
                                  nameof(services));
            Guard.ArgumentNotNull(refreshed,
                                  nameof(refreshed));
            Guard.ArgumentNotNull(device,
                                  nameof(device));

            _logger    = logger;
            _services  = services;
            _refreshed = refreshed;
            _device    = device;
        }

        /// <inheritdoc />
        public GattCommunicationStatus GattCommunicationStatus =>
            _gattResult?.Status ?? GattCommunicationStatus.Unreachable;

        /// <inheritdoc />
        public async Task Refresh()
        {
            _services.Clear();

            if (_device.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                _logger.Error($"{_device.DeviceId} {_device.ConnectionStatus}");

                _refreshed.OnNext(GattCommunicationStatus.Unreachable);

                return;
            }

            _gattResult = await _device.GetGattServicesAsync();

            if (_gattResult.Status == GattCommunicationStatus.Success)
                await GetCharacteristicsAsync(_gattResult);
            else
                // todo log everywhere [device.DeviceID]
                _logger.Error($"[{_device.DeviceId}] Gatt communication status " +
                              $"'{_gattResult.Status}'");

            _refreshed.OnNext(_gattResult.Status);
        }

        /// <inheritdoc />
        public IObservable<GattCommunicationStatus> Refreshed => _refreshed;

        /// <inheritdoc />
        public IReadOnlyDictionary<IGattDeviceServiceWrapper, IGattCharacteristicsResultWrapper> Services =>
            _services.ReadOnlyDictionary;

        private async Task GetCharacteristicsAsync(IGattDeviceServicesResultWrapper gatt)
        {
            foreach (var service in gatt.Services)
            {
                var characteristics = await service.GetCharacteristicsAsync();

                if (characteristics.Status != GattCommunicationStatus.Success)
                {
                    _logger.Error("Could not get Characteristics for device " +
                                  $"'{service.DeviceId}' and service '{service.Uuid}'");

                    continue;
                }

                _services[service] = characteristics;
            }
        }
    }
}