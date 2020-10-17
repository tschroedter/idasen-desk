using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Idasen.BluetoothLE.Core;
using Idasen.BluetoothLE.Linak.Interfaces;
using JetBrains.Annotations;
using Serilog;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class DeskMover
        : IDeskMover
    {
        public delegate IDeskMover Factory(IDeskCommandExecutor executor,
                                           IDeskHeightAndSpeed  heightAndSpeed);

        private readonly IStoppingHeightCalculator _calculator;
        private readonly  IDeskCommandExecutor _executor;
        private readonly IDeskHeightAndSpeed  _heightAndSpeed;

        private readonly  ILogger        _logger;
        private readonly IScheduler     _scheduler;
        private readonly  ISubject<uint> _subjectFinished;

        public readonly TimeSpan TimerInterval = TimeSpan.FromMilliseconds(100);

        private IDisposable _disposableTimer;

        private IDisposable _disposalHeightAndSpeed;

        private bool _isAllowedToMove;

        public DeskMover([NotNull] ILogger                   logger,
                         [NotNull] IScheduler                scheduler,
                         [NotNull] IDeskCommandExecutor      executor,
                         [NotNull] IDeskHeightAndSpeed       heightAndSpeed,
                         [NotNull] IStoppingHeightCalculator calculator,
                         [NotNull] ISubject<uint>            subjectFinished)
        {
            Guard.ArgumentNotNull(logger,
                                  nameof(logger));
            Guard.ArgumentNotNull(scheduler,
                                  nameof(scheduler));
            Guard.ArgumentNotNull(executor,
                                  nameof(executor));
            Guard.ArgumentNotNull(heightAndSpeed,
                                  nameof(heightAndSpeed));
            Guard.ArgumentNotNull(calculator,
                                  nameof(calculator));
            Guard.ArgumentNotNull(subjectFinished,
                                  nameof(subjectFinished));

            _logger          = logger;
            _scheduler  = scheduler;
            _executor        = executor;
            _heightAndSpeed  = heightAndSpeed;
            _calculator      = calculator;
            _subjectFinished = subjectFinished;
        }

        public uint Height       { get; private set; }
        public int  Speed        { get; private set; }
        public uint TargetHeight { get; set; }

        public IObservable<uint> Finished => _subjectFinished;

        public void Start()
        {
            _logger.Information("Starting...");

            _disposalHeightAndSpeed?.Dispose();
            _disposableTimer?.Dispose();

            _heightAndSpeed.Refresh()
                           .ContinueWith(StartAfterRefreshed)
                           .ConfigureAwait(false);
        }

        private void StartAfterRefreshed(Task obj)
        {
            Height = _heightAndSpeed.Height;
            Speed  = _heightAndSpeed.Speed;

            _calculator.Height                   = Height;
            _calculator.Speed                    = Speed;
            _calculator.StartMovingIntoDirection = Direction.None;
            _calculator.TargetHeight             = TargetHeight;

            _calculator.Calculate();

            StartMovingIntoDirection = _calculator.MoveIntoDirection;

            _disposalHeightAndSpeed = _heightAndSpeed.HeightAndSpeedChanged
                                                     .SubscribeOn(_scheduler)
                                                     .Subscribe(OnHeightAndSpeedChanged);

            _disposableTimer = Observable.Interval(TimerInterval)
                                         .Subscribe(OnTimerElapsed);

            _isAllowedToMove = true;
        }

        public Direction StartMovingIntoDirection { get; set; }

        public async Task<bool> Up()
        {
            _logger.Information("Moving up...");

            if (_isAllowedToMove)
                return await _executor.Up();

            _logger.Information("Moving 'Up' was canceled");

            return false;
        }

        public async Task<bool> Down()
        {
            _logger.Information("Moving down...");

            if (_isAllowedToMove)
                return await _executor.Down();

            _logger.Information("Moving 'Down' was canceled");

            return false;
        }

        public async Task<bool> Stop()
        {
            _logger.Information("Stopping...");

            _isAllowedToMove              = false;
            _calculator.MoveIntoDirection = Direction.None;

            _disposalHeightAndSpeed?.Dispose();
            _disposableTimer?.Dispose();

            var stop = await _executor.Stop();

            _subjectFinished.OnNext(Height);

            return stop;
        }

        public void Dispose()
        {
            _disposalHeightAndSpeed?.Dispose();
            _disposableTimer?.Dispose();
        }

        private void OnTimerElapsed(long time)
        {
            if(!Move().Wait(TimerInterval * 10))
                _logger.Warning("Calling Move() timed-out");
        }

        private void OnHeightAndSpeedChanged(HeightSpeedDetails details)
        {
            Height = details.Height;
            Speed  = details.Speed;
        }

        private async Task Move()
        {
            _logger.Information("Moving...");

            if (!_isAllowedToMove)
                _logger.Information("Not allowed to move...");

            _calculator.Height                   = Height;
            _calculator.Speed                    = Speed;
            _calculator.TargetHeight             = TargetHeight;
            _calculator.StartMovingIntoDirection = StartMovingIntoDirection;
            _calculator.Calculate();

            if (_calculator.MoveIntoDirection == Direction.None ||
                _calculator.HasReachedTargetHeight)
            {
                await Stop();

                return;
            }

            switch (_calculator.MoveIntoDirection)
            {
                case Direction.Up:
                    await Up();
                    break;
                case Direction.Down:
                    await Down();
                    break;
                case Direction.None:
                    break;
                default:
                    await Stop();
                    break;
            }
        }
    }
}