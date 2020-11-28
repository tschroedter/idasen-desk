using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Runtime.CompilerServices ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

[ assembly : InternalsVisibleTo ( "Idasen.BluetoothLE.Linak.Tests" ) ]

namespace Idasen.BluetoothLE.Linak.Control
{
    public class DeskMover
        : IDeskMover
    {
        public DeskMover ( [ NotNull ] ILogger                               logger ,
                           [ NotNull ] IScheduler                            scheduler ,
                           [ NotNull ] IInitialHeightAndSpeedProviderFactory providerFactory ,
                           [ NotNull ] IDeskCommandExecutor                  executor ,
                           [ NotNull ] IDeskHeightAndSpeed                   heightAndSpeed ,
                           [ NotNull ] IStoppingHeightCalculator             calculator ,
                           [ NotNull ] ISubject < uint >                     subjectFinished )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( providerFactory ,
                                    nameof ( providerFactory ) ) ;
            Guard.ArgumentNotNull ( executor ,
                                    nameof ( executor ) ) ;
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;
            Guard.ArgumentNotNull ( calculator ,
                                    nameof ( calculator ) ) ;
            Guard.ArgumentNotNull ( subjectFinished ,
                                    nameof ( subjectFinished ) ) ;

            _logger          = logger ;
            _scheduler       = scheduler ;
            _providerFactory = providerFactory ;
            _executor        = executor ;
            _heightAndSpeed  = heightAndSpeed ;
            _calculator      = calculator ;
            _subjectFinished = subjectFinished ;
        }

        public uint Height       { get ; private set ; }
        public int  Speed        { get ; private set ; }
        public uint TargetHeight { get ; set ; }

        public IObservable < uint > Finished => _subjectFinished ;

        public void Initialize ( )
        {
            _initialProvider?.Dispose ( ) ;
            _initialProvider = _providerFactory.Create ( _executor ,
                                                         _heightAndSpeed ) ; // todo maybe threading issue
            _initialProvider.Initialize ( ) ;

            _disposableProvider?.Dispose ( ) ;
            _disposableProvider = _initialProvider.Finished
                                                  .ObserveOn ( _scheduler )
                                                  .Subscribe ( OnFinished ) ;
        }

        public void Start ( )
        {
            _logger.Debug ( "Starting..." ) ;

            _disposalHeightAndSpeed?.Dispose ( ) ;
            _disposableTimer?.Dispose ( ) ;

            _initialProvider.Start ( ) ;
        }

        public async Task < bool > Up ( )
        {
            if ( IsAllowedToMove )
                return await _executor.Up ( ) ;

            return false ;
        }

        public async Task < bool > Down ( )
        {
            if ( IsAllowedToMove )
                return await _executor.Down ( ) ;

            return false ;
        }

        public async Task < bool > Stop ( )
        {
            _logger.Debug ( "Stopping..." ) ;

            IsAllowedToMove               = false ;
            _calculator.MoveIntoDirection = Direction.None ;

            _disposalHeightAndSpeed?.Dispose ( ) ;
            _disposableTimer?.Dispose ( ) ;

            var stop = await _executor.Stop ( ) ;

            _subjectFinished.OnNext ( Height ) ;

            return stop ;
        }

        public void Dispose ( )
        {
            _disposableProvider?.Dispose ( ) ;
            _disposalHeightAndSpeed?.Dispose ( ) ;
            _disposableTimer?.Dispose ( ) ;
        }

        public delegate IDeskMover Factory ( IDeskCommandExecutor executor ,
                                             IDeskHeightAndSpeed  heightAndSpeed ) ;

        public Direction StartMovingIntoDirection { get ; set ; }

        public bool IsAllowedToMove { get ; private set ; }

        private void StartAfterRefreshed ( )
        {
            Height = _heightAndSpeed.Height ;
            Speed  = _heightAndSpeed.Speed ;

            _calculator.Height                   = Height ;
            _calculator.Speed                    = Speed ;
            _calculator.StartMovingIntoDirection = Direction.None ;
            _calculator.TargetHeight             = TargetHeight ;

            _calculator.Calculate ( ) ;

            StartMovingIntoDirection = _calculator.MoveIntoDirection ;

            _disposalHeightAndSpeed = _heightAndSpeed.HeightAndSpeedChanged
                                                     .ObserveOn ( _scheduler )
                                                     .Subscribe ( OnHeightAndSpeedChanged ) ;

            _disposableTimer?.Dispose ( ) ;
            _disposableTimer = Observable.Interval ( TimerInterval )
                                         .SubscribeOn ( _scheduler )
                                         .SubscribeOn ( _scheduler )
                                         .Subscribe ( OnTimerElapsed ) ;

            IsAllowedToMove = true ;
        }

        internal void OnTimerElapsed ( long time )
        {
            if ( ! Move ( ).Wait ( TimerInterval * 10 ) )
                _logger.Warning ( "Calling Move() timed-out" ) ;
        }

        private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
        {
            Height = details.Height ;
            Speed  = details.Speed ;
        }

        private void OnFinished ( uint height )
        {
            Height = height ;

            StartAfterRefreshed ( ) ;
        }

        private async Task Move ( )
        {
            _logger.Debug ( "Moving..." ) ;

            if ( ! IsAllowedToMove )
            {
                _logger.Debug ( "Not allowed to move..." ) ;

                return ;
            }

            _calculator.Height                   = Height ;
            _calculator.Speed                    = Speed ;
            _calculator.TargetHeight             = TargetHeight ;
            _calculator.StartMovingIntoDirection = StartMovingIntoDirection ;
            _calculator.Calculate ( ) ;

            if ( _calculator.MoveIntoDirection == Direction.None ||
                 _calculator.HasReachedTargetHeight )
            {
                await Stop ( ) ;

                return ;
            }

            switch ( _calculator.MoveIntoDirection )
            {
                case Direction.Up :
                    await Up ( ) ;
                    break ;
                case Direction.Down :
                    await Down ( ) ;
                    break ;
                case Direction.None :
                    break ;
                default :
                    await Stop ( ) ;
                    break ;
            }
        }

        private readonly IStoppingHeightCalculator _calculator ;
        private readonly IDeskCommandExecutor      _executor ;
        private readonly IDeskHeightAndSpeed       _heightAndSpeed ;

        private readonly ILogger                               _logger ;
        private readonly IInitialHeightAndSpeedProviderFactory _providerFactory ;
        private readonly IScheduler                            _scheduler ;
        private readonly ISubject < uint >                     _subjectFinished ;

        public readonly TimeSpan    TimerInterval = TimeSpan.FromMilliseconds ( 100 ) ;
        private         IDisposable _disposableProvider ;

        private IDisposable _disposableTimer ;

        private IDisposable            _disposalHeightAndSpeed ;
        private IInitialHeightProvider _initialProvider ;
    }
}