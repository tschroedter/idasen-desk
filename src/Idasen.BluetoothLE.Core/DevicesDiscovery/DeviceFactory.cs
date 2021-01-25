using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeviceFactory
        : IDeviceFactory
    {
        public DeviceFactory ( [ NotNull ] Device.Factory factory )
        {
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _factory = factory ;
        }

        /// <inheritdoc />
        public IDevice Create ( IDateTimeOffset broadcastTime ,
                                ulong           address ,
                                string          name ,
                                short           rawSignalStrengthInDBm )
        {
            Guard.ArgumentNotNull ( broadcastTime ,
                                    nameof ( broadcastTime ) ) ;

            return _factory.Invoke ( broadcastTime ,
                                     address ,
                                     name ,
                                     rawSignalStrengthInDBm ) ;
        }

        private readonly Device.Factory _factory ;
    }
}