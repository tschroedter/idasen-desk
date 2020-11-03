using System ;
using System.Linq ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class RawValueChangedDetailsCollectorTests
    {
        private const string Timestamp1 = "2020-11-03T11:39:45.5123366+10:30" ;
        private const string Timestamp2 = "2020-11-03T11:39:45.5123366+10:31" ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger    = Substitute.For < ILogger > ( ) ;
            _scheduler = new TestScheduler ( ) ;
            _desk      = Substitute.For < IDesk > ( ) ;

            _subjectHeightAndSpeed = new Subject < HeightSpeedDetails > ( ) ;

            _desk.HeightAndSpeedChanged
                 .Returns ( _subjectHeightAndSpeed ) ;

            _details1 = new HeightSpeedDetails ( DateTimeOffset.Parse ( Timestamp1 ) ,
                                                 1u ,
                                                 11 ) ;

            _details2 = new HeightSpeedDetails ( DateTimeOffset.Parse ( Timestamp2 ) ,
                                                 2 ,
                                                 22 ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedWithDetails1_AddsDetails1 ( )
        {
            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;

            _scheduler.Start ( ) ;

            sut.Details
               .Should ( )
               .Contain ( _details1 ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedWithDetails1And2_AddsDetails1And2 ( )
        {
            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details2 ) ;

            _scheduler.Start ( ) ;

            using var scope = new AssertionScope ( ) ;

            sut.Details
               .Should ( )
               .Contain ( _details1 ) ;

            sut.Details
               .Should ( )
               .Contain ( _details2 ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedMultipleTimes_DetailsWontGetBiggerThenMax ( )
        {
            using var sut = CreateSut ( ) ;

            for ( var i = 0 ; i <= RawValueChangedDetailsCollector.MaxItems + 1 ; i ++ )
            {
                _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            }

            _scheduler.Start ( ) ;

            using var scope = new AssertionScope ( ) ;

            sut.Details
               .Count ( )
               .Should ( )
               .Be ( RawValueChangedDetailsCollector.MaxItems ) ;
        }

        [ TestMethod ]
        public void ToString_ForInvokedAndDetailsAvailable_Instance ( )
        {
            var expected = "Timestamp = " + Timestamp1 + ", Height = 1, Speed = 11|" +
                           "Timestamp = " + Timestamp2 + ", Height = 2, Speed = 22" ;

            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details2 ) ;

            _scheduler.Start ( ) ;

            using var scope = new AssertionScope ( ) ;

            sut.ToString ( )
               .Should ( )
               .Be ( expected ) ;
        }

        private RawValueChangedDetailsCollector CreateSut ( )
        {
            return new RawValueChangedDetailsCollector ( _logger ,
                                                         _scheduler ,
                                                         _desk ) ;
        }

        private IDesk              _desk ;
        private HeightSpeedDetails _details1 ;
        private HeightSpeedDetails _details2 ;
        private ILogger            _logger ;
        private TestScheduler      _scheduler ;

        private Subject < HeightSpeedDetails > _subjectHeightAndSpeed ;
    }
}