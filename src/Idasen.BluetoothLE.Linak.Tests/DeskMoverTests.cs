using System ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
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
    public class DeskMoverTests
    {
        private const uint InitialHeight       = 100u ;
        private const uint DefaultTargetHeight = 1500 ;
        private const uint DefaultHeight       = 1000 ;
        private const int  DefaultSpeed        = 200 ;

        [ TestInitialize ]
        public void Initialize ( )
        {
            _logger                = Substitute.For < ILogger > ( ) ;
            _scheduler             = new TestScheduler ( ) ;
            _providerFactory       = Substitute.For < IInitialHeightAndSpeedProviderFactory > ( ) ;
            _monitorFactory        = Substitute.For < IDeskMovementMonitorFactory > ( ) ;
            _executor              = Substitute.For < IDeskCommandExecutor > ( ) ;
            _heightAndSpeed        = Substitute.For < IDeskHeightAndSpeed > ( ) ;
            _calculator            = Substitute.For < IStoppingHeightCalculator > ( ) ;
            _subjectHeightAndSpeed = new Subject < HeightSpeedDetails > ( ) ;
            _subjectFinished       = new Subject < uint > ( ) ;
            _provider              = Substitute.For < IInitialHeightProvider > ( ) ;
            _heightMonitor         = Substitute.For < IDeskHeightMonitor > ( ) ;

            _providerFactory.Create ( Arg.Any < IDeskCommandExecutor > ( ) ,
                                      Arg.Any < IDeskHeightAndSpeed > ( ) )
                            .Returns ( _provider ) ;

            _provider.Finished
                     .Returns ( _subjectFinished ) ;

            _heightAndSpeed.HeightAndSpeedChanged
                           .Returns ( _subjectHeightAndSpeed ) ;
            _heightAndSpeed.Height
                           .Returns ( DefaultHeight ) ;
            _heightAndSpeed.Speed
                           .Returns ( DefaultSpeed ) ;

            _details1 = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                 123u ,
                                                 321 ) ;

            _heightMonitor.IsHeightChanging ( )
                          .Returns ( true ) ;

            _monitor = Substitute.For < IDeskMovementMonitor > ( ) ;

            _monitorFactory.Create ( _heightAndSpeed )
                           .Returns ( _monitor ) ;

            _finished = Substitute.For < ISubject < uint > > ( ) ;

            _disposable = Substitute.For < IDisposable > ( ) ;

            _finished.Subscribe ( Arg.Any < IObserver < uint > > ( ) )
                     .Returns ( _disposable ) ;

            _disposableProvider = Substitute.For < IInitialHeightProvider > ( ) ;
            _disposableProvider.Finished
                               .Returns ( _finished ) ;
        }

        private DeskMover CreateSut ( )
        {
            return new DeskMover ( _logger ,
                                   _scheduler ,
                                   _providerFactory ,
                                   _monitorFactory ,
                                   _executor ,
                                   _heightAndSpeed ,
                                   _calculator ,
                                   _subjectFinished ,
                                   _heightMonitor ) ;
        }

        private DeskMover CreateSutInitialized ( )
        {
            var sut = CreateSut ( ) ;

            sut.Initialize ( ) ;

            return sut ;
        }

        private DeskMover CreateSutWithTargetHeight ( )
        {
            var sut = CreateSutInitialized ( ) ;

            sut.TargetHeight = DefaultTargetHeight ;

            return sut ;
        }

        private DeskMover CreateSutWithIsAllowedToMoveIsTrue ( )
        {
            var sut = CreateSutWithTargetHeight ( ) ;

            _subjectFinished.OnNext ( InitialHeight ) ;

            _scheduler.Start ( ) ;

            return sut ;
        }

        [ TestMethod ]
        public void StartAfterRefreshed_ForInvoked_SetsHeight ( )
        {
            CreateSutWithIsAllowedToMoveIsTrue ( ).Height
                                                  .Should ( )
                                                  .Be ( DefaultHeight ) ;
        }

        [ TestMethod ]
        public void StartAfterRefreshed_ForInvoked_SetsSpeed ( )
        {
            CreateSutWithIsAllowedToMoveIsTrue ( ).Speed
                                                  .Should ( )
                                                  .Be ( DefaultSpeed ) ;
        }

        [ TestMethod ]
        public void StartAfterRefreshed_ForInvoked_CallsCalculator ( )
        {
            using var _ = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            using var scope = new AssertionScope ( ) ;

            _calculator.Height
                       .Should ( )
                       .Be ( DefaultHeight ,
                             "Height" ) ;
            _calculator.Speed
                       .Should ( )
                       .Be ( DefaultSpeed ,
                             "Speed" ) ;

            _calculator.StartMovingIntoDirection
                       .Should ( )
                       .Be ( Direction.None ,
                             "StartMovingIntoDirection" ) ;

            _calculator.TargetHeight
                       .Should ( )
                       .Be ( DefaultTargetHeight ,
                             "TargetHeight" ) ;

            _calculator.Received ( )
                       .Calculate ( ) ;
        }

        [ TestMethod ]
        public void StartAfterRefreshed_ForInvoked_SetsIsAllowedToMoveToTrue ( )
        {
            CreateSutWithIsAllowedToMoveIsTrue ( ).IsAllowedToMove
                                                  .Should ( )
                                                  .BeTrue ( ) ;
        }

        [ TestMethod ]
        public void StartAfterRefreshed_ForIsAllowedToMoveIsTrueAndSuccess_SetsStartMovingIntoDirection ( )
        {
            CreateSutWithIsAllowedToMoveIsTrue ( ).StartMovingIntoDirection
                                                  .Should ( )
                                                  .Be ( _calculator.MoveIntoDirection ) ;
        }

        [TestMethod]
        public void Start_Invoked_CallsHeightMonitorResets()
        {
            _executor.Up()
                     .Returns(true);

            using var sut = CreateSutWithIsAllowedToMoveIsTrue();

            _heightMonitor.Received ( )
                          .Reset ( ) ;
        }

        [ TestMethod ]
        public async Task Up_ForIsAllowedToMoveIsTrueAndSuccess_ReturnsTrue ( )
        {
            _executor.Up ( )
                     .Returns ( true ) ;

            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            var actual = await sut.Up ( ) ;

            actual.Should ( )
                  .BeTrue ( ) ;
        }

        [ TestMethod ]
        public async Task Up_ForIsAllowedToMoveIsTrueAndFailed_ReturnsFalse ( )
        {
            _executor.Up ( )
                     .Returns ( false ) ;

            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            var actual = await sut.Up ( ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void Height_ForIsAllowedToMoveIsTrueAndSuccessAndNotified_UpdatesHeight ( )
        {
            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;

            _scheduler.Start ( ) ;

            sut.Height
               .Should ( )
               .Be ( _details1.Height ) ;
        }

        [ TestMethod ]
        public void Speed_ForIsAllowedToMoveIsTrueAndSuccessAndNotified_UpdatesSpeed ( )
        {
            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            _subjectHeightAndSpeed.OnNext ( _details1 ) ;

            _scheduler.Start ( ) ;

            sut.Speed
               .Should ( )
               .Be ( _details1.Speed ) ;
        }

        [ TestMethod ]
        public void OnTimerElapsed_ForMoveIntoDirectionUp_MovesUp ( )
        {
            _calculator.MoveIntoDirection
                       .Returns ( Direction.Up ) ;

            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            sut.OnTimerElapsed ( 1 ) ;

            _executor.Received ( )
                     .Up ( ) ;
        }

        [ TestMethod ]
        public void OnTimerElapsed_ForMoveIntoDirectionDown_MovesDown ( )
        {
            _calculator.MoveIntoDirection
                       .Returns ( Direction.Down ) ;

            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            sut.OnTimerElapsed ( 1 ) ;

            _executor.Received ( )
                     .Down ( ) ;
        }

        [ TestMethod ]
        public void OnTimerElapsed_ForMoveIntoDirectionNone_MoveStop ( )
        {
            _calculator.MoveIntoDirection
                       .Returns ( Direction.None ) ;

            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            sut.OnTimerElapsed ( 1 ) ;

            using var scope = new AssertionScope ( ) ;

            _executor.Received ( )
                     .Stop ( ) ;
        }

        [ TestMethod ]
        public void OnTimerElapsed_ForIsAllowedToMoveIsFalse_DoesNotMove ( )
        {
            _calculator.MoveIntoDirection
                       .Returns ( Direction.None ) ;

            using var sut = CreateSutInitialized ( ) ;

            sut.OnTimerElapsed ( 1 ) ;

            using var scope = new AssertionScope ( ) ;

            _executor.DidNotReceive ( )
                     .Up ( ) ;
            _executor.DidNotReceive ( )
                     .Down ( ) ;
            _executor.DidNotReceive ( )
                     .Stop ( ) ;
        }


        [ TestMethod ]
        public async Task Finished_ForMoveFinished_Notifies ( )
        {
            using var sut = CreateSutWithIsAllowedToMoveIsTrue ( ) ;

            var wasNotified = false ;

            sut.Finished.ObserveOn ( _scheduler )
               .Subscribe ( x => wasNotified = true ) ;

            await sut.Stop ( ) ;

            _scheduler.Start ( ) ;

            wasNotified.Should ( )
                       .BeTrue ( ) ;
        }

        [ TestMethod ]
        public async Task Up_ForIsAllowedToMoveIsTrue_DoesNotMoveUp ( )
        {
            using var sut = CreateSutInitialized ( ) ;

            await sut.Up ( ) ;

            _ = _executor.DidNotReceive ( )
                         .Up ( ) ;
        }

        [ TestMethod ]
        public async Task Up_ForIsAllowedToMoveIsTrue_ReturnsFalse ( )
        {
            using var sut = CreateSutInitialized ( ) ;

            var actual = await sut.Up ( ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ TestMethod ]
        public async Task Down_ForIsAllowedToMoveIsTrue_DoesNotMoveDown ( )
        {
            using var sut = CreateSutInitialized ( ) ;

            await sut.Down ( ) ;

            _ = _executor.DidNotReceive ( )
                         .Down ( ) ;
        }

        [ TestMethod ]
        public async Task Down_ForIsAllowedToMoveIsTrue_ReturnsFalse ( )
        {
            using var sut = CreateSutInitialized ( ) ;

            var actual = await sut.Down ( ) ;

            actual.Should ( )
                  .BeFalse ( ) ;
        }

        [ TestMethod ]
        public void Start_ForInvoked_CallsProvider ( )
        {
            using var sut = CreateSutInitialized ( ) ;

            sut.Start ( ) ;

            _provider.Received ( )
                     .Start ( ) ;
        }

        [ TestMethod ]
        public void Dispose_ForInvoked_DisposesMonitor ( )
        {
            var sut = CreateSutInitialized ( ) ;

            sut.Dispose ( ) ;

            _monitor.Received ( )
                    .Dispose ( ) ;
        }

        [ TestMethod ]
        public void Dispose_ForInvoked_DisposesDisposableProvider ( )
        {

            _providerFactory.Create(_executor,
                                    _heightAndSpeed)
                            .Returns(_disposableProvider);

            var sut = CreateSutInitialized ( ) ;

            sut.Dispose ( ) ;

            _disposable.Received ( )
                       .Dispose ( ) ;
        }

        [TestMethod]
        public void Dispose_ForInvoked_DisposalHeightAndSpeed()
        {
            IDisposable disposable = Substitute.For<IDisposable>();

            var subject = Substitute.For < ISubject < HeightSpeedDetails > > ( ) ;

            subject.Subscribe ( Arg.Any < IObserver < HeightSpeedDetails > > ( ) )
                   .Returns ( disposable ) ;

            _heightAndSpeed.HeightAndSpeedChanged
                           .Returns(subject);

            var sut = CreateSutWithIsAllowedToMoveIsTrue();

            sut.Dispose();

            disposable.Received()
                      .Dispose();
        }

        private IStoppingHeightCalculator _calculator ;
        private HeightSpeedDetails        _details1 ;
        private IDeskCommandExecutor      _executor ;
        private IDeskHeightAndSpeed       _heightAndSpeed ;

        private ILogger                               _logger ;
        private IDeskMovementMonitorFactory           _monitorFactory ;
        private IInitialHeightProvider                _provider ;
        private IInitialHeightAndSpeedProviderFactory _providerFactory ;
        private TestScheduler                         _scheduler ;
        private Subject < uint >                      _subjectFinished ;
        private Subject < HeightSpeedDetails >        _subjectHeightAndSpeed ;
        private IDeskHeightMonitor                    _heightMonitor ;
        private IDeskMovementMonitor                  _monitor ;
        private IInitialHeightProvider                _disposableProvider ;
        private IObservable < uint >                  _finished ;
        private IDisposable                           _disposable ;
    }
}