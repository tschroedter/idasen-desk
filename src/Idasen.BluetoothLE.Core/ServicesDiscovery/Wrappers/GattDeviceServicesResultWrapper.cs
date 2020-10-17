using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class GattDeviceServicesResultWrapper
        : IGattDeviceServicesResultWrapper
    {
        public delegate IGattDeviceServicesResultWrapper Factory(GattDeviceServicesResult service);

        private readonly GattDeviceServicesResult              _service;
        private readonly IEnumerable<GattDeviceServiceWrapper> _services;

        public GattDeviceServicesResultWrapper(
            [NotNull] IGattCharacteristicsResultWrapperFactory characteristicsFactory,
            [NotNull] GattDeviceServicesResult                 service)
        {
            Guard.ArgumentNotNull(characteristicsFactory,
                                  nameof(characteristicsFactory));
            Guard.ArgumentNotNull(service,
                                  nameof(service));

            _service = service;

            _services = _service.Services
                                .Select(s => new GattDeviceServiceWrapper(characteristicsFactory,
                                                                          s))
                                .ToArray();
        }

        /// <inheritdoc />
        public GattCommunicationStatus Status => _service.Status;

        /// <inheritdoc />
        public IEnumerable<IGattDeviceServiceWrapper> Services => _services;

        /// <inheritdoc />
        public byte? ProtocolError => _service.ProtocolError;
    }
}