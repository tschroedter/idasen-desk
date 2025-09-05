using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.ViewModels.Windows;
using NSubstitute;
using Serilog;
using Wpf.Ui.Tray.Controls;
using Xunit;
using System.Reactive.Concurrency;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Windows;

public class IdasenDeskWindowViewModelTests
{
    private sealed class TestSettingsChanges : IObserveSettingsChanges
    {
        public Subject<bool> AdvancedSubject { get; } = new();
        public Subject<bool> LockSubject { get; } = new();
        public IObservable<bool> AdvancedSettingsChanged => AdvancedSubject;
        public IObservable<bool> LockSettingsChanged => LockSubject;
    }

    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
    private readonly IUiDeskManager _uiDeskManager = Substitute.For<IUiDeskManager>();
    private readonly TestSettingsChanges _settingsChanges = new();
    private readonly IScheduler _scheduler = Scheduler.Immediate;

    private IdasenDeskWindowViewModel CreateSut() => new(
        _logger,
        _serviceProvider,
        _uiDeskManager,
        _settingsChanges,
        _scheduler);

    [Fact]
    public void Command_CanExecuteStanding_WhenInitializedAndConnected_ShouldBeTrue()
    {
        // Arrange
        var sut = CreateSut();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(true);

        // Act
        var canExecute = sut.StandingCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void Command_CanExecuteStanding_WhenNotConnected_ShouldBeFalse()
    {
        // Arrange
        var sut = CreateSut();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(false);

        // Act
        var canExecute = sut.StandingCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeFalse();
    }

    [Fact]
    public void Command_CanExecuteConnect_WhenInitializedAndNotConnected_ShouldBeTrue()
    {
        // Arrange
        var sut = CreateSut();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(false);

        // Act
        var canExecute = sut.ConnectCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void Command_CanExecuteDisconnect_WhenConnected_ShouldBeTrue()
    {
        // Arrange
        var sut = CreateSut();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(true);

        // Act
        var canExecute = sut.DisconnectCommand.CanExecute(null);

        // Assert
        canExecute.Should().BeTrue();
    }

    [Fact]
    public void Initialize_ShouldSetTrayMenuItemsAndInitializeDeskManager()
    {
        // Arrange
        var sut = CreateSut();
        var notifyIcon = new NotifyIcon();

        // Act
        sut.Initialize(notifyIcon);

        // Assert
        _uiDeskManager.Received(1).Initialize(notifyIcon);
        sut.TrayMenuItems.Should().NotBeNull();
        sut.TrayMenuItems.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task LockSettingsChanged_True_ShouldCallMoveLockAsync()
    {
        // Arrange
        var sut = CreateSut();
        var notifyIcon = new NotifyIcon();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(true);
        sut.Initialize(notifyIcon);

        // Act
        _settingsChanges.LockSubject.OnNext(true);
        await Task.Delay(10); // allow async subscription to run

        // Assert
        await _uiDeskManager.Received(1).MoveLockAsync();
    }

    [Fact]
    public async Task LockSettingsChanged_False_ShouldCallMoveUnlockAsync()
    {
        // Arrange
        var sut = CreateSut();
        var notifyIcon = new NotifyIcon();
        _uiDeskManager.IsInitialize.Returns(true);
        _uiDeskManager.IsConnected.Returns(true);
        sut.Initialize(notifyIcon);

        // Act
        _settingsChanges.LockSubject.OnNext(false);
        await Task.Delay(10); // allow async subscription to run

        // Assert
        await _uiDeskManager.Received(1).MoveUnlockAsync();
    }
}
