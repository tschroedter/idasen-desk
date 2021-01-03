using System.Threading.Tasks ;
using Windows.Storage.Streams ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class ControlTest
        : CharacteristicBaseTests < Control >
    {
        [ TestMethod ]
        public void RawControl2_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < Control > ( ) ;

            sut.RawControl2
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawControl2_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < Control > ( )
                     .Refresh ( ) ;

            sut.RawControl2
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        [ TestMethod ]
        public void RawControl3_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < Control > ( ) ;

            sut.RawControl3
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawControl3_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < Control > ( )
                     .Refresh ( ) ;

            sut.RawControl3
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        [ TestMethod ]
        public async Task TryWriteRawValue_ForKnownCharacteristics_WritesRawValuesAsync ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < Control > ( )
                     .Refresh ( ) ;

            await sut.TryWriteRawControl2 ( RawValue1 ) ;

            await RawValueWriter.Received ( )
                                .TryWriteValueAsync ( CharacteristicWrapper1 ,
                                                      Arg.Is < IBuffer > ( x => x.Length == RawValue1.Length ) ) ;
        }


        protected override Control CreateSut ( )
        {
            return new Control ( Logger ,
                                 Scheduler ,
                                 Device ,
                                 ProviderFactory ,
                                 RawValueReader ,
                                 RawValueWriter ,
                                 ToStringConverter ,
                                 DescriptionToUuid ) ;
        }

        protected override void PopulateWrappers ( )
        {
            Wrappers.Add ( Control.Control2Key ,
                           CharacteristicWrapper1 ) ;

            Wrappers.Add ( Control.Control3Key ,
                           CharacteristicWrapper1 ) ;
        }
    }
}