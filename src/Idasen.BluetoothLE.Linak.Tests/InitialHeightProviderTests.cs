using System ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using FluentAssertions ;
using FluentAssertions.Execution ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Linak.Control ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Microsoft.Reactive.Testing ;
using Microsoft.VisualStudio.TestTools.UnitTesting ;
using NSubstitute ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Tests
{
    [ TestClass ]
    public class InitialHeightProviderTests
    {
        private const uint SomeHeight = 123u ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _scheduler       = new TestScheduler ( ) ;
            _logger          = Substitute.For < ILogger > ( ) ;
            _heightAndSpeed  = Substitute.For < IDeskHeightAndSpeed > ( ) ;
            _executor        = Substitute.For < IDeskCommandExecutor > ( ) ;
            _subjectFinished = new Subject < uint > ( ) ;

            _subjectHeightAndSpeed = new Subject < HeightSpeedDetails > ( ) ;

            _heightAndSpeed.HeightAndSpeedChanged
                           .Returns ( _subjectHeightAndSpeed ) ;
            _heightAndSpeed.Height
                           .Returns ( SomeHeight ) ;

            _details1 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 1u ,
                                                 2 ) ;

            _details2 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 11u ,
                                                 22 ) ;

            _detailsZeroHeight = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                          0u ,
                                                          22 ) ;
        }

        [ TestMethod ]
        public void Initialize_ForInvokedTwice_DisposesOldSubscription ( )
        {
            var disposable1    = Substitute.For < IDisposable > ( ) ;
            var disposable2    = Substitute.For < IDisposable > ( ) ;

            var heightAndSpeed = Substitute.For < IObservable < HeightSpeedDetails > > ( ) ;
            heightAndSpeed.Subscribe ( Arg.Any < IObserver < HeightSpeedDetails > > ( ) )
                          .Returns ( disposable1 ,
                                     disposable2 ) ;

            _heightAndSpeed.HeightAndSpeedChanged
                           .Returns ( heightAndSpeed ) ;

            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;
            sut.Initialize ( ) ;

            disposable1.Received ( )
                       .Dispose ( ) ;
            disposable2.DidNotReceive (  )
                       .Dispose (  );
        }

        [ TestMethod ]
        public void Initialize_ForInvoked_SetsHeight ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            sut.Height
               .Should ( )
               .Be ( SomeHeight ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTime_UpdatesHeight ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details2 ) ;

            _scheduler.Start ( ) ;

            sut.Height
               .Should ( )
               .Be ( _details2.Height ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTime_HasReceivedHeightAndSpeedIsTrue ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details2 ) ;

            _scheduler.Start ( ) ;

            sut.HasReceivedHeightAndSpeed
               .Should ( )
               .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTime_NotifiesFinished ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            var finished = false ;

            sut.Finished
               .ObserveOn ( _scheduler )
               .Subscribe ( x => finished = true ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;

            _scheduler.Start ( ) ;

            finished
               .Should ( )
               .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTimeWithHeightZero_UpdatesHeight ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _detailsZeroHeight ) ;

            _scheduler.Start ( ) ;

            sut.Height
               .Should ( )
               .Be ( _detailsZeroHeight.Height ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTimeWithHeightZero_HasReceivedHeightAndSpeedIsFalse ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _detailsZeroHeight ) ;

            _scheduler.Start ( ) ;

            sut.HasReceivedHeightAndSpeed
               .Should ( )
               .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedFirstTimeWithHeightZero_DoesNotNotifiesFinished ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            var finished = false ;

            sut.Finished
               .ObserveOn ( _scheduler )
               .Subscribe ( x => finished = true ) ;

            _subjectHeightAndSpeed.OnNext ( _detailsZeroHeight ) ;
            _scheduler.Start ( ) ;

            finished
               .Should ( )
               .BeFalse ( ) ;
        }


        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedSecondTime_UpdatesHeight ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _subjectHeightAndSpeed.OnNext ( _details2 ) ;

            _scheduler.Start ( ) ;

            sut.Height
               .Should ( )
               .Be ( _details2.Height ) ;
        }

        [ TestMethod ]
        public void OnHeightAndSpeedChanged_ForInvokedSecondTime_DoesNotNotifiesFinished ( )
        {
            using var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;
            _scheduler.Start ( ) ;

            var finished = false ;

            sut.Finished
               .ObserveOn ( _scheduler )
               .Subscribe ( x => finished = true ) ;

            _subjectHeightAndSpeed.OnNext ( _details2 ) ;
            _scheduler.Start ( ) ;

            finished
               .Should ( )
               .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void Start_ForNotInitialized_Throws ( )
        {
            var sut = CreateSut ( ) ;

            Func < Task > action = async ( ) => await sut.Start ( ) ;

            action.Should ( )
                  .Throw < NotInitializeException > ( ) ;
        }

        [ TestMethod ]
        public async Task Start_ForInitializedAndHeightGreaterZero_SetsHasReceivedHeightAndSpeedToTrue ( )
        {
            _heightAndSpeed.Height
                           .Returns ( 1u ) ;

            var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            await sut.Start ( ) ;

            sut.HasReceivedHeightAndSpeed
               .Should ( )
               .BeTrue ( ) ;
        }

        [ TestMethod ]
        public async Task Start_ForInitializedAndHeightGreaterZero_NotifiesFinished ( )
        {
            _heightAndSpeed.Height
                           .Returns ( 1u ) ;

            var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            var finished = false ;

            sut.Finished
               .ObserveOn ( _scheduler )
               .Subscribe ( x => finished = true ) ;

            await sut.Start ( ) ;

            _scheduler.Start ( ) ;

            finished
               .Should ( )
               .BeTrue ( ) ;
        }

        [ TestMethod ]
        public async Task Start_ForInitializedAndHeightIsZero_SetsHasReceivedHeightAndSpeedToFalse ( )
        {
            _heightAndSpeed.Height
                           .Returns ( 0u ) ;

            var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            await sut.Start ( ) ;

            sut.HasReceivedHeightAndSpeed
               .Should ( )
               .BeFalse ( ) ;
        }

        [ TestMethod ]
        public async Task Start_ForInitializedAndHeightIsZero_MoveDeskABit ( )
        {
            _heightAndSpeed.Height
                           .Returns ( 0u ) ;

            var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            await sut.Start ( ) ;

            using var scope = new AssertionScope ( ) ;

            await _executor.Received ( )
                           .Up ( ) ;

            await _executor.Received ( )
                           .Stop ( ) ;
        }

        [ TestMethod ]
        public void HasReceivedHeightAndSpeed_ForInvoked_False ( )
        {
            var sut = CreateSut ( ) ;

            sut.HasReceivedHeightAndSpeed
               .Should ( )
               .BeFalse ( ) ;
        }

        private InitialHeightProvider CreateSut ( )
        {
            var deviceMonitor = new InitialHeightProvider ( _logger ,
                                                            _scheduler ,
                                                            _heightAndSpeed ,
                                                            _executor ,
                                                            _subjectFinished ) ;

            return deviceMonitor ;
        }

        private HeightSpeedDetails _details1 ;
        private HeightSpeedDetails _details2 ;
        private HeightSpeedDetails _detailsZeroHeight ;

        private IDeskCommandExecutor           _executor ;
        private IDeskHeightAndSpeed            _heightAndSpeed ;
        private ILogger                        _logger ;
        private TestScheduler                  _scheduler ;
        private Subject < uint >               _subjectFinished ;
        private Subject < HeightSpeedDetails > _subjectHeightAndSpeed ;
    }
}