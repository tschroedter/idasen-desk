using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Common
{
    [ TestClass ]
    public class AllGattCharacteristicsProviderTests
    {
        private const string UnknownDescription = "Unknown" ;
        private const string KnownDescription   = "Device Name" ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _unknownGuid = Guid.Empty ;
            _knownGuid   = Guid.Parse ( "00002a00-0000-1000-8000-00805f9b34fb" ) ;
        }

        [ TestMethod ]
        public void TryGetDescription_ForUnknownGuid_False ( )
        {
            CreateSut ( ).TryGetDescription ( _unknownGuid ,
                                              out _ )
                         .Should ( )
                         .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void TryGetDescription_ForUnknownGuid_Null ( )
        {
            CreateSut ( ).TryGetDescription ( _unknownGuid ,
                                              out var description ) ;

            description.Should ( )
                       .BeNull ( ) ;
        }

        [ TestMethod ]
        public void TryGetDescription_ForKnownGuid_True ( )
        {
            CreateSut ( ).TryGetDescription ( _knownGuid ,
                                              out _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryGetDescription_ForKnownGuid_Description ( )
        {
            CreateSut ( ).TryGetDescription ( _knownGuid ,
                                              out var description ) ;

            description.Should ( )
                       .Be ( KnownDescription ) ;
        }

        [ TestMethod ]
        public void TryGetUuid_ForUnknownDescription_False ( )
        {
            CreateSut ( ).TryGetUuid ( UnknownDescription ,
                                       out _ )
                         .Should ( )
                         .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void TryGetUuid_ForUnknownDescription_Empty ( )
        {
            CreateSut ( ).TryGetUuid ( UnknownDescription ,
                                       out var uuid ) ;

            uuid.Should ( )
                .BeEmpty ( ) ;
        }

        [ TestMethod ]
        public void TryGetUuid_ForKnownDescription_True ( )
        {
            CreateSut ( ).TryGetUuid ( KnownDescription ,
                                       out _ )
                         .Should ( )
                         .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void TryGetUuid_ForKnownDescription_Guid ( )
        {
            CreateSut ( ).TryGetUuid ( KnownDescription ,
                                       out var uuid ) ;

            uuid.Should ( )
                .Be ( _knownGuid ) ;
        }

        [ TestMethod ]
        public void OfficialGattCharacteristics_ForInvoked_ResourceFilename ( )
        {
            CreateSut ( ).OfficialGattCharacteristics
                         .Should ( )
                         .Be ( "Idasen.BluetoothLE.Characteristics.Common." +
                               AllGattCharacteristicsProvider.Filename ) ;
        }

        private AllGattCharacteristicsProvider CreateSut ( )
        {
            return new AllGattCharacteristicsProvider ( ) ;
        }

        private Guid _knownGuid ;
        private Guid _unknownGuid ;
    }
}