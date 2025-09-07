using System.Reactive.Concurrency;
using System.Reflection;
using Idasen.BluetoothLE.Linak.Interfaces;
using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.TraySettings;
using Idasen.SystemTray.Win11.Utils;
using NSubstitute;
using Serilog;

namespace Idasen.SystemTray.Win11.Tests.Utils;

public class UiDeskManagerTests
{
    private static UiDeskManager CreateSut(
        out IDesk desk,
        out ISettingsManager settingsManager)
    {
        var logger         = Substitute.For<ILogger>();
        settingsManager    = Substitute.For<ISettingsManager>();
        var iconProvider   = Substitute.For<ITaskbarIconProvider>();
        var notifications  = Substitute.For<INotifications>();
        var scheduler      = Scheduler.Immediate;
        var deskProvider   = Substitute.For<IDeskProvider>();
        var providerFactory = new Func<IDeskProvider>(() => deskProvider);
        var errorManager   = Substitute.For<IErrorManager>();

        // Ensure CurrentSettings is available during SUT construction
        var settings = new Settings
        {
            HeightSettings = new HeightSettings()
        };
        settingsManager.CurrentSettings.Returns(settings);

        var sut = new UiDeskManager(
            logger,
            settingsManager,
            iconProvider,
            notifications,
            scheduler,
            providerFactory,
            errorManager);

        // Inject a connected desk via reflection to bypass connection flow
        desk = Substitute.For<IDesk>();
        var field = typeof(UiDeskManager).GetField("_desk", BindingFlags.Instance | BindingFlags.NonPublic);
        field!.SetValue(sut, desk);

        return sut;
    }

    [Fact]
    public async Task Custom1Async_MovesDeskToConfiguredCustom1Height() 
    {
        // Arrange
        var sut = CreateSut(out var desk, out var settingsManager);
        var settings = settingsManager.CurrentSettings as Settings;
        settings!.HeightSettings.Custom1HeightInCm = 111;
        settingsManager.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        await sut.Custom1Async();

        // Assert
        await settingsManager.Received(1).LoadAsync(Arg.Any<CancellationToken>());
        desk.Received(1).MoveTo(111u * 100u);
    }

    [Fact]
    public async Task EatingAsync_MovesDeskToConfiguredEatingHeight()
    {
        // Arrange
        var sut = CreateSut(out var desk, out var settingsManager);
        var settings = settingsManager.CurrentSettings as Settings;
        settings!.HeightSettings.Custom2HeightInCm = 66;
        settingsManager.LoadAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        await sut.EatingAsync();

        // Assert
        await settingsManager.Received(1).LoadAsync(Arg.Any<CancellationToken>());
        desk.Received(1).MoveTo(66u * 100u);
    }
}
