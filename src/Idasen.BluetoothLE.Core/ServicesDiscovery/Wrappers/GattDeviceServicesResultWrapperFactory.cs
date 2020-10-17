using System.Diagnostics.CodeAnalysis;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class GattDeviceServicesResultWrapperFactory
        : IGattDeviceServicesResultWrapperFactory
    {
        private readonly GattDeviceServicesResultWrapper.Factory _servicesFactory;

        public GattDeviceServicesResultWrapperFactory(
            [NotNull] GattDeviceServicesResultWrapper.Factory servicesFactory)
        {
            Guard.ArgumentNotNull(servicesFactory,
                                  nameof(servicesFactory));

            _servicesFactory = servicesFactory;
        }

        /// <inheritdoc />
        public IGattDeviceServicesResultWrapper Create(GattDeviceServicesResult result)
        {
            Guard.ArgumentNotNull(result,
                                  nameof(result));

            return _servicesFactory.Invoke(result);
        }
    }
}