using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Windows.Devices.Bluetooth ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <inheritdoc />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class Device
        : IDevice
    {
        public Device ( [ NotNull ] IScheduler                scheduler ,
                        [ NotNull ] IBluetoothLeDeviceWrapper wrapper )
        {
            Guard.ArgumentNotNull ( wrapper ,
                                    nameof ( wrapper ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;

            _wrapper = wrapper ;

            _subscriber = _wrapper.ConnectionStatusChanged
                                  .SubscribeOn ( scheduler )
                                  .Subscribe ( _ => Connect ( ) ) ;
        }

        /// <inheritdoc />
        public IObservable < BluetoothConnectionStatus > ConnectionStatusChanged => _wrapper.ConnectionStatusChanged ;

        /// <inheritdoc />
        public GattCommunicationStatus GattCommunicationStatus => _wrapper.GattCommunicationStatus ;

        /// <inheritdoc />
        public ulong BluetoothAddress => _wrapper.BluetoothAddress ;

        /// <inheritdoc />
        public string BluetoothAddressType => _wrapper.BluetoothAddressType ;

        /// <inheritdoc />
        public void Connect ( )
        {
            if ( ConnectionStatus == BluetoothConnectionStatus.Connected )
                return ;

            _wrapper.Connect ( ) ;
        }

        /// <inheritdoc />
        public string Name => _wrapper.Name ;

        /// <inheritdoc />
        public string Id => _wrapper.DeviceId ;

        /// <inheritdoc />
        public bool IsPaired => _wrapper.IsPaired ;

        /// <inheritdoc />
        public BluetoothConnectionStatus ConnectionStatus => _wrapper.ConnectionStatus ;

        /// <inheritdoc />
        public IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > GattServices =>
            _wrapper.GattServices ;

        /// <inheritdoc />
        public IObservable < GattCommunicationStatus > GattServicesRefreshed => _wrapper.GattServicesRefreshed ;

        /// <inheritdoc />
        public void Dispose ( )
        {
            _wrapper?.Dispose ( ) ;
            _subscriber?.Dispose ( ) ;
        }

        public delegate IDevice Factory ( IBluetoothLeDeviceWrapper wrapper ) ;

        private readonly IDisposable               _subscriber ;
        private readonly IBluetoothLeDeviceWrapper _wrapper ;
    }
}