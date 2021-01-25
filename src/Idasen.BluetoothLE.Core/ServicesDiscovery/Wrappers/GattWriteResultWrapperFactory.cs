using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
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
        public IGattWriteResultWrapper Create ( GattWriteResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            return _factory.Invoke ( result ) ;
        }

        private readonly GattWriteResultWrapper.Factory _factory ;
    }
}