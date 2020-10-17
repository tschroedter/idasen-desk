using System.Diagnostics.CodeAnalysis;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class GattCharacteristicsResultWrapperFactory
        : IGattCharacteristicsResultWrapperFactory
    {
        private readonly GattCharacteristicsResultWrapper.Factory _factory;

        public GattCharacteristicsResultWrapperFactory(
            [NotNull] GattCharacteristicsResultWrapper.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        /// <inheritdoc />
        public IGattCharacteristicsResultWrapper Create(
            GattCharacteristicsResult result)
        {
            Guard.ArgumentNotNull(result,
                                  nameof(result));

            return _factory(result);
        }
    }
}