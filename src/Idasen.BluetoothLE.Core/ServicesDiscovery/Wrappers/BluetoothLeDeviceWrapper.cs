using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery;
using Serilog;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class BluetoothLeDeviceWrapper
        : IBluetoothLeDeviceWrapper
    {
        public delegate IBluetoothLeDeviceWrapper Factory(
            ILogger                                  logger,
            IGattServicesProviderFactory             providerFactory,
            IGattDeviceServicesResultWrapperFactory  servicesFactory,
            IGattServicesDictionary                  gattServicesDictionary,
            IGattCharacteristicsResultWrapperFactory characteristicsFactory,
            ISubject<BluetoothConnectionStatus>      connectionStatusChanged,
            BluetoothLEDevice                        device);

        private readonly ISubject<BluetoothConnectionStatus>     _connectionStatusChanged;
        private readonly BluetoothLEDevice                       _device;
        private readonly IGattServicesDictionary                 _gattServicesDictionary;
        private readonly ILogger                                 _logger;
        private readonly IGattServicesProvider                   _provider;
        private readonly IGattDeviceServicesResultWrapperFactory _servicesFactory;
        private readonly IDisposable                             _subscriberConnectionStatus;
        private          GattSession                             _session;

        public BluetoothLeDeviceWrapper(
            [NotNull] ILogger                                 logger,
            [NotNull] IScheduler                              scheduler,
            [NotNull] IGattServicesProviderFactory            providerFactory,
            [NotNull] IGattDeviceServicesResultWrapperFactory servicesFactory,
            [NotNull] IGattServicesDictionary                 gattServicesDictionary,
            [NotNull] ISubject<BluetoothConnectionStatus>     connectionStatusChanged,
            [NotNull] BluetoothLEDevice                       device)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(scheduler,
                                  nameof(scheduler));
            Guard.ArgumentNotNull(providerFactory,
                                  nameof(providerFactory));
            Guard.ArgumentNotNull(servicesFactory,
                                  nameof(servicesFactory));
            Guard.ArgumentNotNull(gattServicesDictionary,
                                  nameof(gattServicesDictionary));
            Guard.ArgumentNotNull(connectionStatusChanged,
                                  nameof(connectionStatusChanged));
            Guard.ArgumentNotNull(device,
                                  nameof(device));

            _logger                  = logger;
            _servicesFactory         = servicesFactory;
            _provider                = providerFactory.Create(this);
            _gattServicesDictionary  = gattServicesDictionary;
            _connectionStatusChanged = connectionStatusChanged;
            _device                  = device;

            var statusChanged =
                Observable.FromEventPattern<object>(_device,
                                                    "ConnectionStatusChanged");

            _subscriberConnectionStatus = statusChanged.SubscribeOn(scheduler)
                                                       .Subscribe(OnConnectionStatusChanged);
        }

        /// <inheritdoc />
        public IObservable<BluetoothConnectionStatus> ConnectionStatusChanged => _connectionStatusChanged;

        /// <inheritdoc />
        public IObservable<GattCommunicationStatus> GattServicesRefreshed => _provider.Refreshed;

        /// <inheritdoc />
        public GattCommunicationStatus GattCommunicationStatus => _provider.GattCommunicationStatus;

        /// <inheritdoc />
        public async void Connect()
        {
            if (ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                _logger.Debug($"[{DeviceId}] Already connected");

                return;
            }

            if (!IsPaired)
            {
                _logger.Debug($"[{DeviceId}] Not paired");

                return;
            }

            await CreateSession();
        }

        /// <inheritdoc />
        public Task<IGattDeviceServicesResultWrapper> GetGattServicesAsync()
        {
            var gattServicesAsync = _device.GetGattServicesAsync().AsTask();

            var result = _servicesFactory.Create(gattServicesAsync.Result);

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public string Name => _device.Name;

        /// <inheritdoc />
        public string DeviceId => _device.DeviceId;

        /// <inheritdoc />
        public bool IsPaired => _device.DeviceInformation.Pairing.IsPaired;

        /// <inheritdoc />
        public BluetoothConnectionStatus ConnectionStatus => _device.ConnectionStatus;

        /// <inheritdoc />
        public IReadOnlyDictionary<IGattDeviceServiceWrapper, IGattCharacteristicsResultWrapper> GattServices =>
            _provider.Services;

        /// <inheritdoc />
        public void Dispose()
        {
            _gattServicesDictionary.Dispose();
            _session.Dispose();
            _subscriberConnectionStatus.Dispose();
            _device.Dispose();
        }

        private async Task CreateSession()
        {
            _session?.Dispose();

            _session                    = await GattSession.FromDeviceIdAsync(_device.BluetoothDeviceId);
            _session.MaintainConnection = true;
        }

        private async void OnConnectionStatusChanged(object args)
        {
            if (ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                _logger.Debug($"{DeviceId} ");

                await _provider.Refresh();
            }

            _connectionStatusChanged.OnNext(_device.ConnectionStatus);
        }
    }
}