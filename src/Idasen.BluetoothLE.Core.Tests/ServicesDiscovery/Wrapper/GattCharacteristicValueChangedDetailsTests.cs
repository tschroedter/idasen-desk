using System ;
using System.Collections.Generic ;
using FluentAssertions ;
using Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using Selkie.AutoMocking ;

namespace Idasen.BluetoothLE.Core.Tests.ServicesDiscovery.Wrapper
{
    [ TestClass ]
    public class GattCharacteristicValueChangedDetailsTests
    {
        [ AutoDataTestMethod ]
        public void Constructor_ForInvoked_SetsUuid (
            GattCharacteristicValueChangedDetails sut ,
            [ Freeze ] Guid                       uuid )
        {
            sut.Uuid
               .Should ( )
               .Be ( uuid ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForInvoked_SetsValue (
            GattCharacteristicValueChangedDetails sut ,
            [ Freeze ] IEnumerable < byte >       value )
        {
            sut.Value
               .Should ( )
               .BeEquivalentTo ( value ) ;
        }

        [ AutoDataTestMethod ]
        public void Constructor_ForInvoked_SetsTimestamp (
            GattCharacteristicValueChangedDetails sut ,
            [ Freeze ] DateTimeOffset             timestamp )
        {
            sut.Timestamp
               .Should ( )
               .Be ( timestamp ) ;
        }
    }
}