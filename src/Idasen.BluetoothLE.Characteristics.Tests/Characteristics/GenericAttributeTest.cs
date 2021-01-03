using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class GenericAttributeTest
        : CharacteristicBaseTests < GenericAttribute >
    {
        [ TestMethod ]
        public void RawDpg_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < Dpg > ( ) ;

            sut.RawServiceChanged
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawDpg_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAttribute > ( )
                     .Refresh ( ) ;

            sut.RawServiceChanged
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        protected override GenericAttribute CreateSut ( )
        {
            return new GenericAttribute ( Logger ,
                                          Scheduler ,
                                          Device ,
                                          ProviderFactory ,
                                          RawValueReader ,
                                          RawValueWriter ,
                                          ToStringConverter ,
                                          DescriptionToUuid ,
                                          new AllGattCharacteristicsProvider ( ) ) ;
        }

        protected override void PopulateWrappers ( )
        {
            Wrappers.Add ( GenericAttribute.CharacteristicServiceChanged ,
                           CharacteristicWrapper1 ) ;
        }
    }
}