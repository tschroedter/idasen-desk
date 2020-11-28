using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <inheritdoc />
    public class GattServicesProviderFactory
        : IGattServicesProviderFactory
    {
        public GattServicesProviderFactory ( [ NotNull ] GattServicesProvider.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        /// <inheritdoc />
        public IGattServicesProvider Create ( IBluetoothLeDeviceWrapper wrapper )
        {
            Guard.ArgumentNotNull ( wrapper ,
                                    nameof ( wrapper ) ) ;

            return _factory.Invoke ( wrapper ) ;
        }

        private readonly GattServicesProvider.Factory _factory ;
    }
}