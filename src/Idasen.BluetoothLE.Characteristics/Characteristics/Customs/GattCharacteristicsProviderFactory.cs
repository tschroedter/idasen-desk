using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Customs
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattCharacteristicsProviderFactory
        : IGattCharacteristicsProviderFactory
    {
        public GattCharacteristicsProviderFactory (
            [ NotNull ] GattCharacteristicProvider.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        public IGattCharacteristicProvider Create (
            IGattCharacteristicsResultWrapper wrapper )
        {
            return _factory ( wrapper ) ;
        }

        private readonly GattCharacteristicProvider.Factory _factory ;
    }
}