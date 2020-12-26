using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    public class GattReadResultWrapperFactory
        : IGatReadResultWrapperFactory
    {
        public GattReadResultWrapperFactory ( [ NotNull ] GattReadResultWrapper.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        /// <inheritdoc />
        public IGattReadResultWrapper Create ( GattReadResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            return _factory.Invoke ( result ) ;
        }

        private readonly GattReadResultWrapper.Factory _factory ;
    }
}