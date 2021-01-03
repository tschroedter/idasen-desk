using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Characteristics.Factories ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Factories ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core ;

namespace Idasen.BluetoothLE.Characteristics
{
    // ReSharper disable once InconsistentNaming
    [ ExcludeFromCodeCoverage ]
    public class BluetoothLEDeskModule
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

            builder.RegisterType < GenericAccessFactory > ( )
                   .As < IGenericAccessFactory > ( ) ;

            builder.RegisterType < GenericAttribute > ( )
                   .As < IGenericAttribute > ( ) ;

            builder.RegisterType < GenericAttributeFactory > ( )
                   .As < IGenericAttributeFactory > ( ) ;

            builder.RegisterType < ReferenceInput > ( )
                   .As < IReferenceInput > ( ) ;

            builder.RegisterType < ReferenceInputFactory > ( )
                   .As < IReferenceInputFactory > ( ) ;

            builder.RegisterType < ReferenceOutput > ( )
                   .As < IReferenceOutput > ( ) ;

            builder.RegisterType < ReferenceOutputFactory > ( )
                   .As < IReferenceOutputFactory > ( ) ;

            builder.RegisterType < Dpg > ( )
                   .As < IDpg > ( ) ;

            builder.RegisterType < DpgFactory > ( )
                   .As < IDpgFactory > ( ) ;

            builder.RegisterType < Control > ( )
                   .As < IControl > ( ) ;

            builder.RegisterType < ControlFactory > ( )
                   .As < IControlFactory > ( ) ;

            builder.RegisterType < CharacteristicBaseToStringConverter > ( )
                   .As < ICharacteristicBaseToStringConverter > ( ) ;

            builder.RegisterType < BufferReader > ( )
                   .As < IBufferReader > ( ) ;

            builder.RegisterType<DescriptionToUuid>()
                   .As<IDescriptionToUuid>();
        }
    }
}