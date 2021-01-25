using System ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskFactory
        : IDeskFactory
    {
        public DeskFactory (
            [ NotNull ] IDeviceFactory                    deviceFactory ,
            [ NotNull ] Func < IDevice , IDeskConnector > deskConnectorFactory ,
            [ NotNull ] Func < IDeskConnector , IDesk >   deskFactory )
        {
            Guard.ArgumentNotNull ( deskConnectorFactory ,
                                    nameof ( deskConnectorFactory ) ) ;
            Guard.ArgumentNotNull ( deviceFactory ,
                                    nameof ( deviceFactory ) ) ;
            Guard.ArgumentNotNull ( deskFactory ,
                                    nameof ( deskFactory ) ) ;

            _deviceFactory        = deviceFactory ;
            _deskConnectorFactory = deskConnectorFactory ;
            _deskFactory          = deskFactory ;
        }

        public async Task < IDesk > CreateAsync ( ulong address )
        {
            var device    = await _deviceFactory.FromBluetoothAddressAsync ( address ) ;
            var connector = _deskConnectorFactory.Invoke ( device ) ;

            return _deskFactory.Invoke ( connector ) ;
        }

        private readonly Func < IDevice , IDeskConnector > _deskConnectorFactory ;
        private readonly Func < IDeskConnector , IDesk >   _deskFactory ;
        private readonly IDeviceFactory                    _deviceFactory ;
    }
}