using System.Reactive.Subjects;
using FluentAssertions;
using Idasen.BluetoothLE.Linak; // for HeightSpeedDetails
using Idasen.BluetoothLE.Linak.Control;
using Idasen.BluetoothLE.Linak.Interfaces;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;

namespace Idasen.SystemTray.Win11.Tests.BluetoothLE.Linak;

public class DeskMoverTests
{
    private sealed class FakeCalculator : IStoppingHeightCalculator
    {
        public uint MaxSpeedToStopMovement { get; set; }
        public int MaxSpeed { get; set; }
        public int Speed { get; set; }
        public float FudgeFactor { get; set; }
        public uint TargetHeight { get; set; }
        public uint Height { get; set; }
        public uint Delta => 0u;
        public uint StoppingHeight => Height;
        public int MovementUntilStop { get; private set; }
        public bool HasReachedTargetHeight { get; private set; }
        public Direction MoveIntoDirection { get; set; }
        public Direction StartMovingIntoDirection { get; set; }

        public IStoppingHeightCalculator Calculate()
        {
            if (TargetHeight == Height)
            {
                MoveIntoDirection = Direction.None;
                HasReachedTargetHeight = true;
                MovementUntilStop = 0;
                return this;
            }

            MoveIntoDirection = TargetHeight > Height ? Direction.Up : Direction.Down;
            HasReachedTargetHeight = false;
            // Keep movement small to avoid predictive crossing stop in tests
            MovementUntilStop = 2;
            return this;
        }
    }

    private sealed class FakeInitialHeightProvider : IInitialHeightProvider
    {
        private readonly Subject<uint> _finished = new();
        public IObservable<uint> Finished => _finished;
        public uint Height { get; set; }
        public bool HasReceivedHeightAndSpeed { get; set; }
        public void Initialize() { }
        public Task Start() => Task.CompletedTask;
        public void Dispose() { }

        public void Emit(uint height) => _finished.OnNext(height);
    }

    private sealed class FakeHeightAndSpeed : IDeskHeightAndSpeed
    {
        private readonly Subject<uint> _heightChanged = new();
        private readonly Subject<int> _speedChanged = new();
        private readonly Subject<HeightSpeedDetails> _heightSpeedChanged = new();

        public IObservable<uint> HeightChanged => _heightChanged;
        public IObservable<int> SpeedChanged => _speedChanged;
        public IObservable<HeightSpeedDetails> HeightAndSpeedChanged => _heightSpeedChanged;

        public uint Height { get; private set; }
        public int Speed { get; private set; }

        public void Set(uint height, int speed)
        {
            Height = height;
            Speed = speed;
            _heightChanged.OnNext(height);
            _speedChanged.OnNext(speed);
            _heightSpeedChanged.OnNext(new HeightSpeedDetails(DateTimeOffset.Now, height, speed));
        }

        public Task Refresh() => Task.CompletedTask;
        public IDeskHeightAndSpeed Initialize() => this;
        public void Dispose() { }
    }

    private static DeskMover CreateSut(
        TestScheduler scheduler,
        out IDeskCommandExecutor executor,
        out FakeHeightAndSpeed heightAndSpeed,
        out FakeInitialHeightProvider initialProvider,
        out Subject<uint> subjectFinished,
        out IDeskHeightMonitor heightMonitor,
        uint initialHeight,
        int initialSpeed)
    {
        var logger = Substitute.For<ILogger>();
        executor = Substitute.For<IDeskCommandExecutor>();
        executor.Up().Returns(Task.FromResult(true));
        executor.Down().Returns(Task.FromResult(true));
        executor.Stop().Returns(Task.FromResult(true));

        // Height/speed source
        heightAndSpeed = new FakeHeightAndSpeed();
        heightAndSpeed.Set(initialHeight, initialSpeed);

        // Height monitor - don't block with stall detection in unit tests
        heightMonitor = Substitute.For<IDeskHeightMonitor>();
        heightMonitor.IsHeightChanging().Returns(true);

        // Movement monitor factory - no-op
        var movementMonitor = Substitute.For<IDeskMovementMonitor>();
        var movementMonitorFactory = Substitute.For<IDeskMovementMonitorFactory>();
        movementMonitorFactory.Create(heightAndSpeed).Returns(movementMonitor);

        // Initial provider factory returning a controllable provider
        var initialProviderFactory = Substitute.For<IInitialHeightAndSpeedProviderFactory>();
        initialProvider = new FakeInitialHeightProvider { Height = initialHeight, HasReceivedHeightAndSpeed = true };
        initialProviderFactory.Create(executor, heightAndSpeed).Returns(initialProvider);

        // Calculator with simple deterministic behavior
        var calculator = new FakeCalculator();

        subjectFinished = new Subject<uint>();

        var mover = new DeskMover(
            logger,
            scheduler,
            initialProviderFactory,
            movementMonitorFactory,
            executor,
            heightAndSpeed,
            calculator,
            subjectFinished,
            heightMonitor
        );

        return mover;
    }

    [Fact]
    public void Move_ReissuesCommand_AsKeepAlive_OnEachTick()
    {
        var scheduler = new TestScheduler();

        // ReSharper disable UnusedVariable
        var sut = CreateSut( scheduler,
                             out var executor,
                             out var heightAndSpeed,
                             out var initialProvider,
                             out var subjectFinished,
                             out var heightMonitor,
                             initialHeight: 60000,
                             initialSpeed: 30);
        // ReSharper restore UnusedVariable

        sut.TargetHeight = 60500;
        sut.Initialize();
        sut.Start();

        // Trigger initial height received to start movement cycle
        initialProvider.Emit(60000);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1).Ticks);

        // Advance several ticks; expect multiple Up commands (keep-alive)
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(300).Ticks);

        var upCalls = executor.ReceivedCalls().Count(c => c.GetMethodInfo().Name == nameof(IDeskCommandExecutor.Up));
        upCalls.Should().BeGreaterThanOrEqualTo(2);

        executor.DidNotReceiveWithAnyArgs().Stop();
    }

    [Fact]
    public void Move_Stops_WhenWithinTolerance_AndPublishesFinished()
    {
        var scheduler = new TestScheduler();

        // ReSharper disable UnusedVariable
        var sut = CreateSut( scheduler,
                             out var executor,
                             out var heightAndSpeed,
                             out var initialProvider,
                             out var subjectFinished,
                             out var heightMonitor,
                             initialHeight: 60000,
                             initialSpeed: 10); // use non-zero speed to ensure a stop command is issued
        // ReSharper restore UnusedVariable

        sut.TargetHeight = 60003; // Within base tolerance (4)
        sut.Initialize();
        sut.Start();

        uint? finishedHeight = null;
        using var finishedSub = sut.Finished.Subscribe(h => finishedHeight = h);

        // Start movement evaluation by providing the initial height
        initialProvider.Emit(60000);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1).Ticks);

        // Advance virtual time long enough to guarantee at least one evaluation tick
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1000).Ticks);

        executor.Received(1).Stop();
        finishedHeight.Should().Be(60000u);
    }
}
