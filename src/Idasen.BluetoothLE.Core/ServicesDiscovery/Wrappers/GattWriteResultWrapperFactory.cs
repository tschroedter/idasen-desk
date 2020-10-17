using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    public class GattWriteResultWrapperFactory
        : IGattWriteResultWrapperFactory
    {
        public GattWriteResultWrapperFactory ( [ NotNull ] GattWriteResultWrapper.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        /// <inheritdoc />
        public IGattWriteResult Create ( GattWriteResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            return _factory.Invoke ( result ) ;
        }

        private readonly GattWriteResultWrapper.Factory _factory ;
    }
}