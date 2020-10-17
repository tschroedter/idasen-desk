using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class GattDeviceServiceWrapper
        : IGattDeviceServiceWrapper
    {
        private readonly IGattCharacteristicsResultWrapperFactory _characteristicsFactory;
        private readonly GattDeviceService                        _gattDeviceService;

        public GattDeviceServiceWrapper(
            [NotNull] IGattCharacteristicsResultWrapperFactory characteristicsFactory,
            [NotNull] GattDeviceService                        gattDeviceService)
        {
            Guard.ArgumentNotNull(characteristicsFactory,
                                  nameof(characteristicsFactory));
            Guard.ArgumentNotNull(gattDeviceService,
                                  nameof(gattDeviceService));

            _characteristicsFactory = characteristicsFactory;
            _gattDeviceService      = gattDeviceService;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _gattDeviceService.Dispose();
        }

        /// <inheritdoc />
        public Guid   Uuid     => _gattDeviceService.Uuid;

        /// <inheritdoc />
        public string DeviceId => _gattDeviceService.DeviceId;

        /// <inheritdoc />
        public Task<IGattCharacteristicsResultWrapper> GetCharacteristicsAsync()
        {
            var characteristics = _gattDeviceService.GetCharacteristicsAsync().AsTask();

            var result = _characteristicsFactory.Create(characteristics.Result)
                                                .Initialize();

            return result;
        }
    }
}