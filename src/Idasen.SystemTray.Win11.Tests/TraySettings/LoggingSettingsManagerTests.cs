using FluentAssertions;
using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.TraySettings;
using Idasen.TestLogger;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog.Events;

#pragma warning disable CA2012 // Use ValueTasks correctly - disabled for test mocking

namespace Idasen.SystemTray.Win11.Tests.TraySettings;

public sealed class LoggingSettingsManagerTests : IDisposable
{
    private readonly InMemoryLogger _logger;
    private readonly LoggingSettingsManager _manager;
    private readonly ISettingsManager _settingsManager;

    public LoggingSettingsManagerTests()
    {
        _logger = new InMemoryLogger();
        _settingsManager = Substitute.For<ISettingsManager>();
        _manager = new LoggingSettingsManager(_logger,
                                                _settingsManager);
    }

    public void Dispose()
    {
        _logger.Dispose();
    }

    [Fact]
    public async Task SaveAsync_ShouldLogDebugAndCallSaveAsync()
    {
        await _manager.SaveAsync(CancellationToken.None);

        _logger.ContainsLevel(LogEventLevel.Debug)
               .Should()
               .BeTrue();

        await _settingsManager.Received(1)
                              .SaveAsync(CancellationToken.None);
    }

    [Fact]
    public async Task LoadAsync_ShouldLogDebugAndCallLoadAsync()
    {
        await _manager.LoadAsync(CancellationToken.None);

        _logger.ContainsLevel(LogEventLevel.Debug)
               .Should()
               .BeTrue();

        await _settingsManager.Received(1)
                              .LoadAsync(CancellationToken.None);
    }

    [Fact]
    public async Task UpgradeSettingsAsync_ShouldLogDebugAndReturnSuccess()
    {
        _settingsManager.UpgradeSettingsAsync(CancellationToken.None).Returns(true);

        var result = await _manager.UpgradeSettingsAsync(CancellationToken.None);

        result.Should()
              .BeTrue();

        _logger.ContainsLevel(LogEventLevel.Debug)
               .Should()
               .BeTrue();
    }

    [Fact]
    public async Task UpgradeSettingsAsync_ShouldLogErrorAndReturnFalseOnFailure()
    {
        _settingsManager.UpgradeSettingsAsync(CancellationToken.None).Returns(false);

        var result = await _manager.UpgradeSettingsAsync(CancellationToken.None);

        result.Should().BeFalse();

        _logger.ContainsLevel(LogEventLevel.Error)
               .Should()
               .BeTrue();
    }

    [Fact]
    public async Task SetLastKnownDeskHeight_ShouldLogDebugAndCallSetLastKnownDeskHeight()
    {
        const uint heightInCm = 100;

        await _manager.SetLastKnownDeskHeight(heightInCm,
                                                CancellationToken.None);

        _logger.ContainsLevel(LogEventLevel.Debug)
               .Should()
               .BeTrue();

        await _settingsManager.Received(1)
                              .SetLastKnownDeskHeight(heightInCm,
                                                        CancellationToken.None);
    }

    [Fact]
    public async Task SaveAsync_ShouldLogErrorAndThrowInvalidOperationException_WhenSaveFails()
    {
        // Arrange
        var exception = new ArgumentException("Test exception");

        _settingsManager.SaveAsync(Arg.Any<CancellationToken>()).Throws(exception);

        // Act
        var act = async () => await _manager.SaveAsync(CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage($"Failed to save settings in file {_settingsManager.SettingsFileName}");

        _logger.Contains("Failed to save settings in file")
               .Should()
               .BeTrue();

        _logger.ContainsLevel(LogEventLevel.Error)
               .Should()
               .BeTrue();
    }

    [Fact]
    public async Task LoadAsync_WhenExceptionThrown_ShouldLogErrorAndThrowInvalidOperationException()
    {
        // Arrange
        var token = CancellationToken.None;

        _settingsManager.LoadAsync(token).Throws(new ArgumentException("Test exception"));

        // Act
        var act = async () => await _manager.LoadAsync(token);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage($"Failed to load settings from file {_settingsManager.SettingsFileName}");

        _logger.Contains("Failed to load settings")
               .Should()
               .BeTrue();

        _logger.ContainsLevel(LogEventLevel.Error)
               .Should()
               .BeTrue();
    }

    [Fact]
    public void Properties_ShouldReturnValuesFromSettingsManager()
    {
        var settings = Substitute.For<ISettings>();
        var settingsSaved = Substitute.For<IObservable<ISettings>>();

        _settingsManager.CurrentSettings.Returns(settings);
        _settingsManager.SettingsFileName.Returns("someFile");
        _settingsManager.SettingsSaved.Returns(settingsSaved);

        _manager.CurrentSettings.Should().Be(settings);
        _manager.SettingsFileName.Should().Be("someFile");
        _manager.SettingsSaved.Should().Be(settingsSaved);
    }

    [Fact]
    public async Task UpgradeSettingsAsync_WhenThrows_ShouldReturnFalseAndLogError()
    {
        _settingsManager.UpgradeSettingsAsync(Arg.Any<CancellationToken>())
                         .Returns(ValueTask.FromException<bool>(new InvalidOperationException("boom")));

        var result = await _manager.UpgradeSettingsAsync(CancellationToken.None);

        result.Should().BeFalse();

        _logger.Contains("Failed to upgrade settings")
               .Should()
               .BeTrue();

        _logger.ContainsLevel(LogEventLevel.Error)
               .Should()
               .BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WhenPreviousCancellationPending_LogsWarningAndCallsSave()
    {
        // Arrange
        _settingsManager.SaveAsync(Arg.Any<CancellationToken>())
                        .Returns(new ValueTask());

        // Act
        await _manager.SaveAsync(CancellationToken.None);

        // Assert
        await _settingsManager.Received(1)
                              .SaveAsync(CancellationToken.None);

        _logger.ContainsLevel(LogEventLevel.Warning)
               .Should()
               .BeTrue();
    }
}
