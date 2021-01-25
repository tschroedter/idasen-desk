using System.Collections.Generic ;
using System.Text ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskCharacteristics
        : IDeskCharacteristics
    {
        public DeskCharacteristics (
            [ NotNull ] ILogger                     logger ,
            [ NotNull ] IDeskCharacteristicsCreator creator )
        {
            Guard.ArgumentNotNull ( creator ,
                                    nameof ( creator ) ) ;
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;

            _logger  = logger ;
            _creator = creator ;
        }

        public async Task Refresh ( )
        {
            foreach ( var characteristicBase in _available.Values )
            {
                await characteristicBase.Refresh ( ) ;
            }
        }

        public IDeskCharacteristics Initialize ( IDevice device )
        {
            Guard.ArgumentNotNull ( device ,
                                    nameof ( device ) ) ;

            _creator.Create ( this ,
                              device ) ;

            return this ;
        }

        public IGenericAccess GenericAccess =>
            _available.As < IGenericAccess > ( DeskCharacteristicKey.GenericAccess ) ;

        public IGenericAttribute GenericAttribute =>
            _available.As < IGenericAttribute > ( DeskCharacteristicKey.GenericAttribute ) ;

        public IReferenceInput ReferenceInput =>
            _available.As < IReferenceInput > ( DeskCharacteristicKey.ReferenceInput ) ;

        public IReferenceOutput ReferenceOutput =>
            _available.As < IReferenceOutput > ( DeskCharacteristicKey.ReferenceOutput ) ;

        public IDpg Dpg => _available.As < IDpg > ( DeskCharacteristicKey.Dpg ) ;

        public IControl Control => _available.As < IControl > ( DeskCharacteristicKey.Control ) ;

        public IDeskCharacteristics WithCharacteristics (
            DeskCharacteristicKey           key ,
            [ NotNull ] ICharacteristicBase characteristic )
        {
            Guard.ArgumentNotNull ( characteristic ,
                                    nameof ( characteristic ) ) ;

            characteristic.Initialize < ICharacteristicBase > ( ) ;

            if ( _available.TryGetValue ( key ,
                                          out var oldCharacteristic ) )
                oldCharacteristic.Dispose ( ) ;

            _available [ key ] = characteristic ;

            _logger.Debug ( $"Added characteristic {characteristic} for key {key}" ) ;

            return this ;
        }

        public IReadOnlyDictionary < DeskCharacteristicKey , ICharacteristicBase > Characteristics => _available ;

        public override string ToString ( )
        {
            var builder = new StringBuilder ( ) ;

            builder.AppendLine ( GenericAccess.ToString ( ) ) ;
            builder.AppendLine ( GenericAttribute.ToString ( ) ) ;
            builder.AppendLine ( ReferenceInput.ToString ( ) ) ;
            builder.AppendLine ( ReferenceOutput.ToString ( ) ) ;
            builder.AppendLine ( Dpg.ToString ( ) ) ;
            builder.AppendLine ( Control.ToString ( ) ) ;

            return builder.ToString ( ) ;
        }

        private readonly Dictionary < DeskCharacteristicKey , ICharacteristicBase > _available =
            new Dictionary < DeskCharacteristicKey , ICharacteristicBase > ( ) ;

        private readonly IDeskCharacteristicsCreator _creator ;
        private readonly ILogger                     _logger ;
    }
}