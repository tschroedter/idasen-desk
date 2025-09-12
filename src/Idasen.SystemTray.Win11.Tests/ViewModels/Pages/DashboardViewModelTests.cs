using FluentAssertions;
using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.ViewModels.Pages;
using NSubstitute;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Pages
{
    public class DashboardViewModelTests
    {
        [Fact]
        public void Constructor_ShouldSetTitleCorrectly()
        {
            // Arrange
            var versionProvider = Substitute.For<IVersionProvider>();
            versionProvider.GetVersion().Returns("1.0.0");

            // Act
            var viewModel = new DashboardViewModel(versionProvider);

            // Assert
            viewModel.Title.Should().Be("Idasen Desk Application 1.0.0");
        }

        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenVersionProviderIsNull()
        {
            // Arrange
            IVersionProvider versionProvider = null!;

            // Act
            var act = () => new DashboardViewModel(versionProvider);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("versionProvider");
        }
    }
}
