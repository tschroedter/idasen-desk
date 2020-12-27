using System ;
using FluentAssertions ;
using Idasen.BluetoothLE.Characteristics.Characteristics ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;

namespace Idasen.BluetoothLE.Characteristics.Tests.Characteristics
{
    [ TestClass ]
    public class RawValueChangedDetailsTests
    {
        private const string Description = "Description" ;

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsDescription ( )
        {
            CreateSut ( ).Description
                         .Should ( )
                         .Be ( Description ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsValue ( )
        {
            CreateSut ( ).Value
                         .Should ( )
                         .BeEquivalentTo ( _value ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsTimestamp ( )
        {
            CreateSut ( ).Timestamp
                         .Should ( )
                         .Be ( _timestamp ) ;
        }

        [ TestMethod ]
        public void Constructor_ForInvoked_SetsUuid ( )
        {
            CreateSut ( ).Uuid
                         .Should ( )
                         .Be ( _uuid ) ;
        }

        [ TestMethod ]
        public void ToString_ForInvoked_Instance ( )
        {
            var expected = "Description = Description, "                     +
                           "Value =  01-02-03, "                             +
                           "Timestamp = 2007-10-02T13:02:03.0000000-07:30, " +
                           "Uuid = 11111111-1111-1111-1111-111111111111" ;

            CreateSut ( ).ToString ( )
                         .Should ( )
                         .Be ( expected ) ;
        }

        private RawValueChangedDetails CreateSut ( )
        {
            return new RawValueChangedDetails ( Description ,
                                                _value ,
                                                _timestamp ,
                                                _uuid ) ;
        }

        private readonly DateTimeOffset _timestamp = DateTimeOffset.Parse ( "2007-10-02T13:02:03.0000000-07:30" ) ;
        private readonly Guid           _uuid      = Guid.Parse ( "11111111-1111-1111-1111-111111111111" ) ;

        private readonly byte [ ] _value = { 1 , 2 , 3 } ;
    }
}