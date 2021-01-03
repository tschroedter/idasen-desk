using System ;
using System.Collections.Generic ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class GenericAccessTest
        : CharacteristicBaseTests < GenericAccess >
    {
        [ TestMethod ]
        public void RawResolution_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < GenericAccess > ( ) ;

            sut.RawResolution
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawResolution_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            sut.RawResolution
               .Should ( )
               .BeEquivalentTo ( RawValue4 ) ;
        }

        [ TestMethod ]
        public void RawParameters_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < GenericAccess > ( ) ;

            sut.RawParameters
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawParameters_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            sut.RawParameters
               .Should ( )
               .BeEquivalentTo ( RawValue3 ) ;
        }

        [ TestMethod ]
        public void RawAppearance_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < GenericAccess > ( ) ;

            sut.RawAppearance
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawAppearance_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            sut.RawAppearance
               .Should ( )
               .BeEquivalentTo ( RawValue2 ) ;
        }

        [ TestMethod ]
        public void RawDeviceName_ForNotRefreshedAndInvoked_EmptyBytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            sut.Initialize < GenericAccess > ( ) ;

            sut.RawDeviceName
               .Should ( )
               .BeEquivalentTo ( CharacteristicBase.RawArrayEmpty ) ;
        }

        [ TestMethod ]
        public async Task RawDeviceName_ForRefreshedAndInvoked_Bytes ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            sut.RawDeviceName
               .Should ( )
               .BeEquivalentTo ( RawValue1 ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForInvoked_AppearanceChanged ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            _subjectAppearanceChanged.Received ( )
                                     .OnNext ( RawValue1 ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForInvoked_ParametersChanged ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            _subjectParametersChanged.Received ( )
                                     .OnNext ( RawValue4 ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForInvoked_ResolutionChanged ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            _subjectResolutionChanged.Received ( )
                                     .OnNext ( RawValue3 ) ;
        }

        [ TestMethod ]
        public async Task Refresh_ForInvoked_DeviceNameChanged ( )
        {
            var sut = CreateSut ( ) ;

            ServiceWrapper.Uuid
                          .Returns ( sut.GattServiceUuid ) ;

            await sut.Initialize < GenericAccess > ( )
                     .Refresh ( ) ;

            _subjectDeviceNameChanged.Received ( )
                                     .OnNext ( RawValue2 ) ;
        }

        protected override void AfterInitialize ( )
        {
            base.AfterInitialize ( ) ;

            _subjectAppearanceChanged = Substitute.For < ISubject < IEnumerable < byte > > > ( ) ;
            _subjectParametersChanged = Substitute.For < ISubject < IEnumerable < byte > > > ( ) ;
            _subjectResolutionChanged = Substitute.For < ISubject < IEnumerable < byte > > > ( ) ;
            _subjectDeviceNameChanged = Substitute.For < ISubject < IEnumerable < byte > > > ( ) ;

            _subjectFactory = Substitute.For < Func < ISubject < IEnumerable < byte > > > > ( ) ;

            _subjectFactory.Invoke ( )
                           .Returns ( _subjectAppearanceChanged ,
                                      _subjectParametersChanged ,
                                      _subjectResolutionChanged ,
                                      _subjectDeviceNameChanged ) ;
        }

        protected override GenericAccess CreateSut ( )
        {
            return new GenericAccess ( Logger ,
                                       Scheduler ,
                                       Device ,
                                       ProviderFactory ,
                                       RawValueReader ,
                                       RawValueWriter ,
                                       ToStringConverter ,
                                       DescriptionToUuid ,
                                       _subjectFactory ,
                                       new AllGattCharacteristicsProvider ( ) ) ;
        }

        protected override void PopulateWrappers ( )
        {
            Wrappers.Add ( GenericAccess.CharacteristicDeviceName ,
                           CharacteristicWrapper1 ) ;
            Wrappers.Add ( GenericAccess.CharacteristicAppearance ,
                           CharacteristicWrapper2 ) ;
            Wrappers.Add ( GenericAccess.CharacteristicParameters ,
                           CharacteristicWrapper3 ) ;
            Wrappers.Add ( GenericAccess.CharacteristicResolution ,
                           CharacteristicWrapper4 ) ;
        }

        private ISubject < IEnumerable < byte > >          _subjectAppearanceChanged ;
        private ISubject < IEnumerable < byte > >          _subjectDeviceNameChanged ;
        private Func < ISubject < IEnumerable < byte > > > _subjectFactory ;
        private ISubject < IEnumerable < byte > >          _subjectParametersChanged ;
        private ISubject < IEnumerable < byte > >          _subjectResolutionChanged ;
    }
}