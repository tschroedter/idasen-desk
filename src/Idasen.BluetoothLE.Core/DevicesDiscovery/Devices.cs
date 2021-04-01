using System.Collections.Generic ;
using System.Linq ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Core.DevicesDiscovery
{
    /// <inheritdoc cref="IDevices" />
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class Devices
        : IDevices
    {
        public Devices ( [ NotNull ] ILogger logger )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger = logger ;
        }

        /// <inheritdoc />
        public IReadOnlyCollection < IDevice > DiscoveredDevices
        {
            get
            {
                lock ( _padLock )
                {
                    return _discoveredDevices.Values
                                             .ToList ( )
                                             .AsReadOnly ( ) ;
                }
            }
        }

        /// <inheritdoc />
        public void AddOrUpdateDevice ( IDevice device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            lock ( _padLock )
            {
                if ( _discoveredDevices.TryGetValue ( device.Address ,
                                                      out var storedDevice ) )
                    UpdateDevice ( device ,
                                   storedDevice ) ;
                else
                    AddDevice ( device ) ;
            }
        }

        /// <inheritdoc />
        public void RemoveDevice ( IDevice device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            lock ( _padLock )
            {
                _discoveredDevices.Remove ( device.Address ) ;

                _logger.Information ( $"[{device.MacAddress}] Device removed" ) ;
            }
        }

        /// <inheritdoc />
        public void Clear ( )
        {
            lock ( _padLock )
            {
                _discoveredDevices.Clear ( ) ;
            }
        }

        /// <inheritdoc />
        public bool ContainsDevice ( IDevice device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            lock ( _padLock )
            {
                return _discoveredDevices.ContainsKey ( device.Address ) ;
            }
        }

        /// <inheritdoc />
        public bool TryGetDevice ( ulong       address ,
                                   out IDevice device )
        {
            lock ( _padLock )
            {
                var result = _discoveredDevices.TryGetValue ( address ,
                                                              out var storedDevice ) ;

                device = storedDevice ;

                return result ;
            }
        }

        private void AddDevice ( IDevice device )
        {
            var newDevice = new Device ( device.BroadcastTime ,
                                         device.Address ,
                                         device.Name ,
                                         device.RawSignalStrengthInDBm ) ;

            _discoveredDevices [ device.Address ] = newDevice ;

            _logger.Information ( $"[{device.MacAddress}] Device added" ) ;
        }

        private void UpdateDevice ( IDevice device ,
                                    Device  storedDevice )
        {
            if ( string.IsNullOrWhiteSpace ( storedDevice.Name ) &&
                 ! string.IsNullOrWhiteSpace ( device.Name ) )
                storedDevice.Name = device.Name ;

            storedDevice.RawSignalStrengthInDBm = device.RawSignalStrengthInDBm ;
            storedDevice.BroadcastTime          = device.BroadcastTime ;

            _logger.Information ( $"[{device.MacAddress}] Device updated" ) ;
        }

        private readonly Dictionary < ulong , Device > _discoveredDevices = new Dictionary < ulong , Device > ( ) ;
        private readonly ILogger                       _logger ;
        private readonly object                        _padLock = new object ( ) ;
    }
}