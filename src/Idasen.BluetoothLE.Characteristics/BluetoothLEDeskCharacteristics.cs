using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;

namespace Idasen.BluetoothLE.Characteristics
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class BluetoothLEDeskCharacteristics
        : Module
    {
        protected override void Load ( ContainerBuilder builder )
        {
            builder.RegisterModule < BluetoothLECoreModule > ( ) ;

            builder.RegisterType < AllGattCharacteristicsProvider > ( )
                   .As < IAllGattCharacteristicsProvider > ( ) ;

            builder.RegisterType < GattCharacteristicProvider > ( )
                   .As < IGattCharacteristicProvider > ( ) ;

            builder.RegisterType < GattCharacteristicsProviderFactory > ( )
                   .As < IGattCharacteristicsProviderFactory > ( ) ;

            builder.RegisterType < RawValueReader > ( )
                   .As < IRawValueReader > ( ) ;

            builder.RegisterType < RawValueWriter > ( )
                   .As < IRawValueWriter > ( ) ;

            builder.RegisterType < GenericAccess > ( )
                   .As < IGenericAccess > ( ) ;

            builder.RegisterType < GenericAttribute > ( )
                   .As < IGenericAttribute > ( ) ;

            builder.RegisterType < ReferenceInput > ( )
                   .As < IReferenceInput > ( ) ;

            builder.RegisterType < ReferenceOutput > ( )
                   .As < IReferenceOutput > ( ) ;

            builder.RegisterType < Dpg > ( )
                   .As < IDpg > ( ) ;

            builder.RegisterType < Control > ( )
                   .As < IControl > ( ) ;

            builder.RegisterType < CharacteristicBaseToStringConverter > ( )
                   .As < ICharacteristicBaseToStringConverter > ( ) ;

            builder.RegisterType < BufferReader > ( )
                   .As < IBufferReader > ( ) ;

            builder.RegisterType < DescriptionToUuid > ( )
                   .As < IDescriptionToUuid > ( ) ;

            builder.RegisterType ( typeof ( CharacteristicBaseFactory ) )
                   .As ( typeof ( ICharacteristicBaseFactory ) ) ;
        }
    }
}