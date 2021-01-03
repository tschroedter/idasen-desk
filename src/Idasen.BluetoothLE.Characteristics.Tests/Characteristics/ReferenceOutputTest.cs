using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class ReferenceOutputTest
        : CharacteristicBaseTests < ReferenceOutput >
    {
        [ TestMethod ]
        public void RawHeightSpeed_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawHeightSpeed
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawHeightSpeed_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawHeightSpeed
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        [ TestMethod ]
        public void RawTwo_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawTwo
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawTwo_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawTwo
               .Should ( )
               .BeEquivalentTo ( RawValue2 ) ;
        }

        [ TestMethod ]
        public void RawThree_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawThree
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawThree_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawThree
               .Should ( )
               .BeEquivalentTo ( RawValue3 ) ;
        }

        [ TestMethod ]
        public void RawFour_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawFour
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawFour_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawFour
               .Should ( )
               .BeEquivalentTo ( RawValue4 ) ;
        }

        [ TestMethod ]
        public void RawFive_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawFive
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawFive_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawFive
               .Should ( )
               .BeEquivalentTo ( RawValue5 ) ;
        }

        [ TestMethod ]
        public void RawSix_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawSix
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawSix_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawSix
               .Should ( )
               .BeEquivalentTo ( RawValue6 ) ;
        }

        [ TestMethod ]
        public void RawSeven_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawSeven
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawSeven_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawSeven
               .Should ( )
               .BeEquivalentTo ( RawValue7 ) ;
        }

        [ TestMethod ]
        public void RawEight_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawEight
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawEight_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawEight
               .Should ( )
               .BeEquivalentTo ( RawValue8 ) ;
        }

        [ TestMethod ]
        public void RawMask_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawMask
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawMask_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawMask
               .Should ( )
               .BeEquivalentTo ( RawValue9 ) ;
        }

        [ TestMethod ]
        public void RawDetectMask_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            sut.RawDetectMask
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawDetectMask_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            sut.RawDetectMask
               .Should ( )
               .BeEquivalentTo ( RawValue10 ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForCharacteristicsHeightSpeedNotFound_DoesNotNotify ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            Wrappers.Remove ( ReferenceOutput.HeightSpeed ) ;

            await sut.Refresh ( ) ;

            _subjectHeightSpeed.DidNotReceive ( )
                               .OnNext ( Arg.Any < RawValueChangedDetails > ( ) ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForCharacteristicsHeightSpeedFound_Notifies ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;


            await sut.Initialize < ReferenceOutput > ( )
                     .Refresh ( ) ;

            _subjectHeightSpeed.Received ( )
                               .OnNext ( Arg.Is < RawValueChangedDetails > ( x => x.Description ==
                                                                                  ReferenceOutput.HeightSpeed ) ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForInvoked_NotifiesHeightAndSpeed ( )
        {
            var scheduler = new TestScheduler ( ) ;
            var subject   = new Subject < RawValueChangedDetails > ( ) ;

            var sut = CreateSut ( scheduler ,
                                  subject ) ;

            RawValueChangedDetails heightAndSpeed = null ;

            subject
               .ObserveOn ( scheduler )
               .Subscribe ( x => { heightAndSpeed = x ; } ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < ReferenceOutput > ( ) ;

            await sut.Refresh ( ) ;

            scheduler.Start ( ) ;

            heightAndSpeed.Value
                          .Should ( )
                          .BeEquivalentTo ( RawValue1 ) ;
        }

        private ReferenceOutput CreateSut ( IScheduler                          scheduler ,
                                            ISubject < RawValueChangedDetails > subject )
        {
            return new ReferenceOutput ( Logger ,
                                         scheduler ,
                                         Device ,
                                         ProviderFactory ,
                                         RawValueReader ,
                                         RawValueWriter ,
                                         ToStringConverter ,
                                         DescriptionToUuid ,
                                         subject ) ;
        }

        protected override ReferenceOutput CreateSut ( )
        {
            return new ReferenceOutput ( Logger ,
                                         Scheduler ,
                                         Device ,
                                         ProviderFactory ,
                                         RawValueReader ,
                                         RawValueWriter ,
                                         ToStringConverter ,
                                         DescriptionToUuid ,
                                         _subjectHeightSpeed ) ;
        }

        protected override void AfterInitialize ( )
        {
            base.AfterInitialize ( ) ;

            _subjectHeightSpeed = Substitute.For < ISubject < RawValueChangedDetails > > ( ) ;
        }

        protected override void PopulateWrappers ( )
        {
            Wrappers.Add ( ReferenceOutput.HeightSpeed ,
                           CharacteristicWrapper1 ) ;

            Wrappers.Add ( ReferenceOutput.Two ,
                           CharacteristicWrapper2 ) ;

            Wrappers.Add ( ReferenceOutput.Three ,
                           CharacteristicWrapper3 ) ;

            Wrappers.Add ( ReferenceOutput.Four ,
                           CharacteristicWrapper4 ) ;

            Wrappers.Add ( ReferenceOutput.Five ,
                           CharacteristicWrapper5 ) ;

            Wrappers.Add ( ReferenceOutput.Six ,
                           CharacteristicWrapper6 ) ;

            Wrappers.Add ( ReferenceOutput.Seven ,
                           CharacteristicWrapper7 ) ;

            Wrappers.Add ( ReferenceOutput.Eight ,
                           CharacteristicWrapper8 ) ;

            Wrappers.Add ( ReferenceOutput.Mask ,
                           CharacteristicWrapper9 ) ;

            Wrappers.Add ( ReferenceOutput.DetectMask ,
                           CharacteristicWrapper10 ) ;
        }

        private ISubject < RawValueChangedDetails > _subjectHeightSpeed ;
    }
}