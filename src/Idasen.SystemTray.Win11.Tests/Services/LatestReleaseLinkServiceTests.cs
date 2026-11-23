using System.Diagnostics ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Services ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.Services ;

public sealed class LatestReleaseLinkServiceTests
{
    [ Theory ]
    [ InlineData ( true ) ]
    [ InlineData ( false ) ]
    public void OpenLatestRelease_ReturnsProcessLauncherResult ( bool launched )
    {
        var processLauncher = Substitute.For < IProcessLauncher > ( ) ;
        processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( launched ) ;
        var sut = new LatestReleaseLinkService ( processLauncher ) ;

        var result = sut.OpenLatestRelease ( ) ;

        result.Should ( ).Be ( launched ) ;
        processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( startInfo =>
                startInfo.FileName == "https://github.com/tschroedter/idasen-desk/releases/latest" &&
                startInfo.UseShellExecute ) ) ;
    }
}
