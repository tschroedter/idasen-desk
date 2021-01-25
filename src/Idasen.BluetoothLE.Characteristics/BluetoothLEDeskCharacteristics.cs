using System.Diagnostics.CodeAnalysis ;
using Autofac ;
using Autofac.Extras.DynamicProxy ;
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
                   .As < IAllGattCharacteristicsProvider > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicProvider > ( )
                   .As < IGattCharacteristicProvider > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GattCharacteristicsProviderFactory > ( )
                   .As < IGattCharacteristicsProviderFactory > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < RawValueReader > ( )
                   .As < IRawValueReader > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < RawValueWriter > ( )
                   .As < IRawValueWriter > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GenericAccess > ( )
                   .As < IGenericAccess > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < GenericAttribute > ( )
                   .As < IGenericAttribute > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < ReferenceInput > ( )
                   .As < IReferenceInput > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < ReferenceOutput > ( )
                   .As < IReferenceOutput > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Dpg > ( )
                   .As < IDpg > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < Control > ( )
                   .As < IControl > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < CharacteristicBaseToStringConverter > ( )
                   .As < ICharacteristicBaseToStringConverter > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < BufferReader > ( )
                   .As < IBufferReader > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType < DescriptionToUuid > ( )
                   .As < IDescriptionToUuid > ( )
                   .EnableInterfaceInterceptors ( ) ;

            builder.RegisterType ( typeof ( CharacteristicBaseFactory ) )
                   .As ( typeof ( ICharacteristicBaseFactory ) )
                   .EnableInterfaceInterceptors ( ) ;
        }
    }
}