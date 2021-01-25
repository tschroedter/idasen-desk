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
    public class GattCharacteristicsResultWrapperFactory
        : IGattCharacteristicsResultWrapperFactory
    {
        public GattCharacteristicsResultWrapperFactory (
            [ NotNull ] GattCharacteristicsResultWrapper.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        /// <inheritdoc />
        public IGattCharacteristicsResultWrapper Create (
            GattCharacteristicsResult result )
        {
            Guard.ArgumentNotNull ( result ,
                                    nameof ( result ) ) ;

            return _factory ( result ) ;
        }

        private readonly GattCharacteristicsResultWrapper.Factory _factory ;
    }
}