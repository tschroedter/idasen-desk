using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class CharacteristicBaseToStringConverterTests
        : CharacteristicBaseTests < TestCharacteristicBase >
    {
        [ TestMethod ]
        public async Task ToString_ForInvokedWithKeyAndNoProperty_Instance ( )
        {
            const string expected = "TestCharacteristicBase\r\n" +
                                    "RawValueKey = [01-02-03]\r\n" ;

            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper1 )
                          .Returns ( ( true , RawValue1 ) ) ;

            await sut.Initialize < TestCharacteristicBase > ( )
                     .Refresh ( ) ;

            sut.ToString ( )
               .Should ( )
               .Be ( expected ) ;
        }

        [ TestMethod ]
        public async Task ToString_ForInvokedWithKeyAndProperty_Instance ( )
        {
            Properties.Add ( TestCharacteristicBase.RawValueKey ,
                             GattCharacteristicProperties.None ) ;

            const string expected = "TestCharacteristicBase\r\n" +
                                    "RawValueKey = [01-02-03] (None)\r\n" ;

            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            RawValueReader.TryReadValueAsync ( CharacteristicWrapper1 )
                          .Returns ( ( true , RawValue1 ) ) ;

            await sut.Initialize < TestCharacteristicBase > ( )
                     .Refresh ( ) ;

            sut.ToString ( )
               .Should ( )
               .Be ( expected ) ;
        }

        [ TestMethod ]
        public void RawArrayEmpty_ForInvoked_Empty ( )
        {
            CharacteristicBaseToStringConverter.RawArrayEmpty
                                               .Should ( )
                                               .BeEmpty ( ) ;
        }

        protected override TestCharacteristicBase CreateSut ( )
        {
            return new TestCharacteristicBase ( Logger ,
                                                Scheduler ,
                                                Device ,
                                                ProviderFactory ,
                                                RawValueReader ,
                                                RawValueWriter ,
                                                new CharacteristicBaseToStringConverter ( ) ,
                                                DescriptionToUuid ) ;
        }

        protected override void PopulateWrappers ( )
        {
            Wrappers.Add ( TestCharacteristicBase.RawValueKey ,
                           CharacteristicWrapper1 ) ;
        }
    }
}