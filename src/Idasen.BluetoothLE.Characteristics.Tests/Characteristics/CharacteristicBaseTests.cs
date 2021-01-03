using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public abstract class CharacteristicBaseTests < T >
        where T : CharacteristicBase
    {
        public const string ToStringResult = "Some Text" ;

        [ TestInitialize ]
        public virtual void Initialize ( )
        {
            Logger                 = Substitute.For < ILogger > ( ) ;
            Device                 = Substitute.For < IDevice > ( ) ;
            Scheduler              = Substitute.For < IScheduler > ( ) ;
            ProviderFactory        = Substitute.For < IGattCharacteristicsProviderFactory > ( ) ;
            CharacteristicProvider = Substitute.For < IGattCharacteristicProvider > ( ) ;
            RawValueReader         = Substitute.For < IRawValueReader > ( ) ;
            RawValueWriter         = Substitute.For < IRawValueWriter > ( ) ;
            ToStringConverter      = Substitute.For < ICharacteristicBaseToStringConverter > ( ) ;
            DescriptionToUuid      = new DescriptionToUuid ( ) ;

            ServiceWrapper = Substitute.For < IGattDeviceServiceWrapper > ( ) ;
            ResultWrapper  = Substitute.For < IGattCharacteristicsResultWrapper > ( ) ;

            WrappersReadOnly.Add ( ServiceWrapper ,
                                   ResultWrapper ) ;

            IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > gattServices =
                WrappersReadOnly ;

            Device.GattServices
                  .Returns ( gattServices ) ;

            ProviderFactory.Create ( ResultWrapper )
                           .Returns ( CharacteristicProvider ) ;

            CharacteristicWrapper1  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper2  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper3  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper4  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper5  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper6  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper7  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper8  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper9  = Substitute.For < IGattCharacteristicWrapper > ( ) ;
            CharacteristicWrapper10 = Substitute.For < IGattCharacteristicWrapper > ( ) ;

            Wrappers = new Dictionary < string , IGattCharacteristicWrapper > ( ) ;

            CharacteristicProvider.Characteristics
                                  .Returns ( Wrappers ) ;

            Properties = new Dictionary < string , GattCharacteristicProperties > ( ) ;

            CharacteristicProvider.Properties
                                  .Returns ( Properties ) ;

            RawValue1  = new byte [ ] { 1 , 2 , 3 } ;
            RawValue2  = new byte [ ] { 4 , 5 , 6 } ;
            RawValue3  = new byte [ ] { 7 , 8 , 9 } ;
            RawValue4  = new byte [ ] { 10 , 11 , 12 } ;
            RawValue4  = new byte [ ] { 13 , 14 , 15 } ;
            RawValue5  = new byte [ ] { 16 , 17 , 18 } ;
            RawValue6  = new byte [ ] { 19 , 20 , 21 } ;
            RawValue7  = new byte [ ] { 22 , 23 , 24 } ;
            RawValue8  = new byte [ ] { 25 , 25 , 27 } ;
            RawValue9  = new byte [ ] { 28 , 29 , 30 } ;
            RawValue10 = new byte [ ] { 31 , 32 , 33 } ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper1 )
                          .Returns ( ( true , RawValue1 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper2 )
                          .Returns ( ( true , RawValue2 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper3 )
                          .Returns ( ( true , RawValue3 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper4 )
                          .Returns ( ( true , RawValue4 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper5 )
                          .Returns ( ( true , RawValue5 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper6 )
                          .Returns ( ( true , RawValue6 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper7 )
                          .Returns ( ( true , RawValue7 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper8 )
                          .Returns ( ( true , RawValue8 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper9 )
                          .Returns ( ( true , RawValue9 ) ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper10 )
                          .Returns ( ( true , RawValue10 ) ) ;

            ToStringConverter.ToString ( Arg.Any < CharacteristicBase > ( ) )
                             .Returns ( ToStringResult ) ;

            PopulateWrappers ( ) ;

            AfterInitialize ( ) ;
        }

        protected virtual void AfterInitialize ( )
        {
        }

        [ TestMethod ]
        public void Initialize_ForKnownGattServiceUuid_SetsCharacteristics ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < T > ( )
               .Characteristics
               .Should ( )
               .Be ( CharacteristicProvider ) ;
        }

        protected abstract T    CreateSut ( ) ;
        protected abstract void PopulateWrappers ( ) ;

        protected readonly Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > WrappersReadOnly
            =
            new Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ( ) ;

        protected IGattCharacteristicProvider                          CharacteristicProvider ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper1 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper10 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper2 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper3 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper4 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper5 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper6 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper7 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper8 ;
        protected IGattCharacteristicWrapper                           CharacteristicWrapper9 ;
        protected IDescriptionToUuid                                   DescriptionToUuid ;
        protected IDevice                                              Device ;
        protected ILogger                                              Logger ;
        protected Dictionary < string , GattCharacteristicProperties > Properties ;
        protected IGattCharacteristicsProviderFactory                  ProviderFactory ;

        protected byte [ ]                                           RawValue1 ;
        protected byte [ ]                                           RawValue10 ;
        protected byte [ ]                                           RawValue2 ;
        protected byte [ ]                                           RawValue3 ;
        protected byte [ ]                                           RawValue4 ;
        protected byte [ ]                                           RawValue5 ;
        protected byte [ ]                                           RawValue6 ;
        protected byte [ ]                                           RawValue7 ;
        protected byte [ ]                                           RawValue8 ;
        protected byte [ ]                                           RawValue9 ;
        protected IRawValueReader                                    RawValueReader ;
        protected IRawValueWriter                                    RawValueWriter ;
        protected IGattCharacteristicsResultWrapper                  ResultWrapper ;
        protected IScheduler                                         Scheduler ;
        protected IGattDeviceServiceWrapper                          ServiceWrapper ;
        protected ICharacteristicBaseToStringConverter               ToStringConverter ;
        protected Dictionary < string , IGattCharacteristicWrapper > Wrappers ;
    }
}