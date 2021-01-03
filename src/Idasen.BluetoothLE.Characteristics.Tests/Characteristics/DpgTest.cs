using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class DpgTest
        : CharacteristicBaseTests < Dpg >
    {
        [ TestMethod ]
        public void RawDpg_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < Dpg > ( ) ;

            sut.RawDpg
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawDpg_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < Dpg > ( )
                     .Refresh ( ) ;

            sut.RawDpg
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        protected override Dpg CreateSut ( )
        {
            return new Dpg ( Logger ,
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
            Wrappers.Add ( Dpg.DpgKey ,
                           CharacteristicWrapper1 ) ;
        }
    }
}