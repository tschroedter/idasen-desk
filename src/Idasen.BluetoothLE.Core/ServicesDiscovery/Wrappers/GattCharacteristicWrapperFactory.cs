using System.Diagnostics.CodeAnalysis;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    public class GattCharacteristicWrapperFactory
        : IGattCharacteristicWrapperFactory
    {
        private readonly GattCharacteristicWrapper.Factory _factory;

        public GattCharacteristicWrapperFactory(
            [NotNull] GattCharacteristicWrapper.Factory factory)
        {
            Guard.ArgumentNotNull(factory,
                                  nameof(factory));

            _factory = factory;
        }

        [ExcludeFromCodeCoverage]
        public IGattCharacteristicWrapper Create(GattCharacteristic characteristic)
        {
            return _factory(characteristic);
        }
    }
}