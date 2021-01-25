using System ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Subjects ;
using Windows.Devices.Bluetooth ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    /// <inheritdoc />
    [ ExcludeFromCodeCoverage ]
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class BluetoothLeDeviceWrapperFactory
        : IBluetoothLeDeviceWrapperFactory
    {
        [ SuppressMessage ( "NDepend" ,
                            "ND1004:AvoidMethodsWithTooManyParameters" ,
                            Justification = "The factory hides a real BluetoothLe device behind an interface." ) ]
        public BluetoothLeDeviceWrapperFactory (
            [ NotNull ] ILogger                                         logger ,
            [ NotNull ] IGattServicesProviderFactory                    providerFactory ,
            [ NotNull ] IGattDeviceServicesResultWrapperFactory         servicesFactory ,
            [ NotNull ] Func < IGattServicesDictionary >                gattServicesDictionaryFactory ,
            [ NotNull ] IGattCharacteristicsResultWrapperFactory        characteristicsFactory ,
            [ NotNull ] Func < ISubject < BluetoothConnectionStatus > > connectionStatusChangedFactory ,
            [ NotNull ] BluetoothLeDeviceWrapper.Factory                factory )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( providerFactory ,
                                    nameof ( providerFactory ) ) ;
            Guard.ArgumentNotNull ( servicesFactory ,
                                    nameof ( servicesFactory ) ) ;
            Guard.ArgumentNotNull ( gattServicesDictionaryFactory ,
                                    nameof ( gattServicesDictionaryFactory ) ) ;
            Guard.ArgumentNotNull ( characteristicsFactory ,
                                    nameof ( characteristicsFactory ) ) ;
            Guard.ArgumentNotNull ( connectionStatusChangedFactory ,
                                    nameof ( connectionStatusChangedFactory ) ) ;
            Guard.ArgumentNotNull ( factory ,
                                    nameof ( factory ) ) ;

            _logger                         = logger ;
            _providerFactory                = providerFactory ;
            _servicesFactory                = servicesFactory ;
            _gattServicesDictionaryFactory  = gattServicesDictionaryFactory ;
            _characteristicsFactory         = characteristicsFactory ;
            _connectionStatusChangedFactory = connectionStatusChangedFactory ;
            _factory                        = factory ;
        }

        /// <inheritdoc />
        public IBluetoothLeDeviceWrapper Create ( BluetoothLEDevice device )
        {
            return _factory ( _logger ,
                              _providerFactory ,
                              _servicesFactory ,
                              _gattServicesDictionaryFactory ( ) ,
                              _characteristicsFactory ,
                              _connectionStatusChangedFactory ( ) ,
                              device ) ;
        }

        private readonly IGattCharacteristicsResultWrapperFactory        _characteristicsFactory ;
        private readonly Func < ISubject < BluetoothConnectionStatus > > _connectionStatusChangedFactory ;
        private readonly BluetoothLeDeviceWrapper.Factory                _factory ;
        private readonly Func < IGattServicesDictionary >                _gattServicesDictionaryFactory ;
        private readonly ILogger                                         _logger ;
        private readonly IGattServicesProviderFactory                    _providerFactory ;
        private readonly IGattDeviceServicesResultWrapperFactory         _servicesFactory ;
    }
}