using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattDeviceServicesResultWrapperFactory
        : IGattDeviceServicesResultWrapperFactory
    {
        public GattDeviceServicesResultWrapperFactory (
            [ JetBrains.Annotations.NotNull ] GattDeviceServicesResultWrapper.Factory servicesFactory )
        {
            Guard.ArgumentNotNull ( servicesFactory ,
                                    nameof ( servicesFactory ) ) ;

            _servicesFactory = servicesFactory ;
        }

        /// <inheritdoc />
        public IGattDeviceServicesResultWrapper Create ( GattDeviceServicesResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            return _servicesFactory.Invoke ( result ) ;
        }

        private readonly GattDeviceServicesResultWrapper.Factory _servicesFactory ;
    }
}