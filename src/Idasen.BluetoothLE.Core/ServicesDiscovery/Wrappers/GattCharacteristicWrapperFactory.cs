using System.Diagnostics.CodeAnalysis ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattCharacteristicWrapperFactory
        : IGattCharacteristicWrapperFactory
    {
        public GattCharacteristicWrapperFactory (
            [ NotNull ] GattCharacteristicWrapper.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        [ ExcludeFromCodeCoverage ]
        public IGattCharacteristicWrapper Create ( GattCharacteristic characteristic )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;

            return _factory ( characteristic ) ;
        }

        private readonly GattCharacteristicWrapper.Factory _factory ;
    }
}