using System ;
using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests.Control
{
    [ TestClass ]
    public class DeskMovementMonitorTests
    {
        private const int DefaultCapacity = 3 ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger                = Substitute.For < ILogger > ( ) ;
            _scheduler             = new TestScheduler ( ) ;
            _heightAndSpeed        = Substitute.For < IDeskHeightAndSpeed > ( ) ;
            _subjectHeightAndSpeed = new Subject < HeightSpeedDetails > ( ) ;


            _heightAndSpeed.HeightAndSpeedChanged
                           .Returns ( _subjectHeightAndSpeed ) ;

            _details1 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 1u ,
                                                 2 ) ;
            _details2 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 3u ,
                                                 4 ) ;
            _details3 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 5u ,
                                                 6 ) ;

            _details4SameHeightAsDetails1 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                                     1u ,
                                                                     22 ) ;
            _details5SameHeightAsDetails1 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                                     1u ,
                                                                     33 ) ;

            _details6WithSpeedZero = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                              1u ,
                                                              0 ) ;
            _details7WithSpeedZero = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                              3u ,
                                                              0 ) ;
            _details8WithSpeedZero = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                              5u ,
                                                              0 ) ;
        }

        private DeskMovementMonitor CreateSut ( )
        {
            var sut = new DeskMovementMonitor ( _logger ,
                                                _scheduler ,
                                                _heightAndSpeed ) ;

            sut.Initialize ( DefaultCapacity ) ;

            return sut ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForThreeEventsWithDifferentHeightAndSpeed_DoesNotThrow ( )
        {
            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details2 ) ;
            _subjectHeightAndSpeed.OnNext ( _details3 ) ;

            Action action = ( ) => _scheduler.Start ( ) ;

            action.Should ( )
                  .NotThrow < ApplicationException > ( ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForThreeEventsWithSameHeight_Throws ( )
        {
            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details4SameHeightAsDetails1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details5SameHeightAsDetails1 ) ;

            Action action = ( ) => _scheduler.Start ( ) ;

            action.Should ( )
                  .Throw < ApplicationException > ( )
                  .WithMessage ( DeskMovementMonitor.HeightDidNotChange ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForThreeEventsWithSpeedZero_Throws ( )
        {
            using var sut = CreateSut ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details6WithSpeedZero ) ;
            _subjectHeightAndSpeed.OnNext ( _details7WithSpeedZero ) ;
            _subjectHeightAndSpeed.OnNext ( _details8WithSpeedZero ) ;

            Action action = ( ) => _scheduler.Start ( ) ;

            action.Should ( )
                  .Throw < ApplicationException > ( )
                  .WithMessage ( DeskMovementMonitor.SpeedWasZero ) ;
        }

        private HeightSpeedDetails  _details1 ;
        private HeightSpeedDetails  _details2 ;
        private HeightSpeedDetails  _details3 ;
        private HeightSpeedDetails  _details4SameHeightAsDetails1 ;
        private HeightSpeedDetails  _details5SameHeightAsDetails1 ;
        private HeightSpeedDetails  _details6WithSpeedZero ;
        private HeightSpeedDetails  _details7WithSpeedZero ;
        private HeightSpeedDetails  _details8WithSpeedZero ;
        private IDeskHeightAndSpeed _heightAndSpeed ;

        private ILogger                        _logger ;
        private TestScheduler                  _scheduler ;
        private Subject < HeightSpeedDetails > _subjectHeightAndSpeed ;
    }
}