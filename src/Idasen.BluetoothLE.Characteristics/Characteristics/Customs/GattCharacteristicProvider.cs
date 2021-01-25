using System ;
using System.Collections.Generic ;
using System.Linq ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Customs
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class GattCharacteristicProvider
        : IGattCharacteristicProvider
    {
        public GattCharacteristicProvider (
            [ NotNull ] ILogger                           logger ,
            [ NotNull ] IGattCharacteristicsResultWrapper gattCharacteristics )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( gattCharacteristics ,
                                    nameof ( gattCharacteristics ) ) ;

            _logger              = logger ;
            _gattCharacteristics = gattCharacteristics ;
        }

        public IReadOnlyDictionary < string , IGattCharacteristicWrapper > Characteristics => _characteristics ;

        public IReadOnlyCollection < string > UnavailableCharacteristics => _unavailable ;
        public IReadOnlyDictionary < string , GattCharacteristicProperties > Properties => _properties ;

        public virtual void Refresh (
            IReadOnlyDictionary < string , Guid > customCharacteristic )
        {
            Guard.ArgumentNotNull ( customCharacteristic ,
                                    nameof ( customCharacteristic ) ) ;

            _logger.Information ( $"{_gattCharacteristics}" ) ;

            _characteristics.Clear ( ) ;
            _unavailable.Clear ( ) ;

            foreach ( var keyValuePair in customCharacteristic )
            {
                var characteristic = _gattCharacteristics.Characteristics
                                                         .FirstOrDefault ( x => x.Uuid == keyValuePair.Value ) ;

                if ( characteristic != null )
                {
                    _logger.Information ( $"Found characteristic {characteristic.Uuid} " +
                                          $"for description '{keyValuePair.Key}'" ) ;

                    _characteristics [ keyValuePair.Key ] = characteristic ;
                    _properties [ keyValuePair.Key ]      = characteristic.CharacteristicProperties ;
                }
                else
                {
                    _logger.Information ( "Did not find characteristic " +
                                          $"for description '{keyValuePair.Key}' and Uuid '{keyValuePair.Value}'" ) ;

                    _unavailable.Add ( keyValuePair.Key ) ;
                }
            }
        }

        public delegate IGattCharacteristicProvider
            Factory ( IGattCharacteristicsResultWrapper gattCharacteristics ) ;

        private readonly Dictionary < string , IGattCharacteristicWrapper > _characteristics =
            new Dictionary < string , IGattCharacteristicWrapper > ( ) ;

        private readonly IGattCharacteristicsResultWrapper _gattCharacteristics ;
        private readonly ILogger                           _logger ;

        private readonly Dictionary < string , GattCharacteristicProperties > _properties =
            new Dictionary < string , GattCharacteristicProperties > ( ) ;

        private readonly List < string > _unavailable = new List < string > ( ) ;
    }
}