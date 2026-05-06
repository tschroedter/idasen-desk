using System.Diagnostics ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Services ;
using Microsoft.Extensions.Configuration ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Tests.Services ;

public class DonateServiceTests
{
    private const string ValidConfiguredUrl = "https://configured.example.com/donate" ;
    private const string DefaultUrl         = "https://www.paypal.com/donate/?hosted_button_id=KAWJDNVJTD7SJ" ;

    private readonly IConfiguration  _configuration  = Substitute.For < IConfiguration > ( ) ;
    private readonly ILogger         _logger         = Substitute.For < ILogger > ( ) ;
    private readonly IProcessLauncher _processLauncher = Substitute.For < IProcessLauncher > ( ) ;

    private DonateService CreateSut ( )
    {
        return new DonateService ( _configuration , _logger , _processLauncher ) ;
    }

    // ── URL resolution ────────────────────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_WhenDonateUrlNotConfigured_LogsDebugAndUsesDefaultUrl ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ( string? ) null ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Debug ( "DonateUrl is not configured. Using built-in default donate URL." ) ;
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.FileName == DefaultUrl ) ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenDonateUrlIsEmpty_LogsDebugAndUsesDefaultUrl ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( string.Empty ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Debug ( "DonateUrl is not configured. Using built-in default donate URL." ) ;
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.FileName == DefaultUrl ) ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenDonateUrlIsNotHttpOrHttps_LogsWarningAndUsesDefaultUrl ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( "ftp://example.com/donate" ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Warning ( "DonateUrl is invalid or not an absolute HTTP/HTTPS URI, using default URL" ) ;
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.FileName == DefaultUrl ) ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenDonateUrlIsRelative_LogsWarningAndUsesDefaultUrl ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( "/relative/path" ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Warning ( "DonateUrl is invalid or not an absolute HTTP/HTTPS URI, using default URL" ) ;
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.FileName == DefaultUrl ) ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenDonateUrlIsValid_UsesConfiguredUrl ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.DidNotReceive ( ).Debug ( Arg.Any < string > ( ) ) ;
        _logger.DidNotReceive ( ).Warning ( Arg.Any < string > ( ) ) ;
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.FileName == ValidConfiguredUrl ) ) ;
    }

    // ── Primary launch path ───────────────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_WhenPrimaryLaunchSucceeds_LogsSuccessAndDoesNotCallFallback ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Information ( "Successfully opened URL in browser" ) ;
        _processLauncher.DidNotReceive ( ).StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenPrimaryLaunchReturnsFalse_LogsWarningAndTriesFallback ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Warning ( "Process.Start returned null, trying fallback method" ) ;
        _processLauncher.Received ( 1 ).StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , 5000 ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenPrimaryLaunchThrowsWin32Exception_LogsWarningAndTriesFallback ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        var win32Ex = new System.ComponentModel.Win32Exception ( ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Throws ( win32Ex ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Warning ( win32Ex , "Primary method failed with Win32Exception, trying fallback" ) ;
        _processLauncher.Received ( 1 ).StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , 5000 ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenPrimaryLaunchThrows_LogsWarningAndTriesFallback ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        var ex = new InvalidOperationException ( "boom" ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Throws ( ex ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Warning ( ex , "Primary method failed, trying fallback" ) ;
        _processLauncher.Received ( 1 ).StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , 5000 ) ;
    }

    // ── Fallback launch path ──────────────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackLaunchReturnsNotStarted_LogsError ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( false , ( int? ) null ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Error ( "Both primary and fallback methods failed to open URL" ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackLaunchTimesOut_LogsError ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) null ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Error ( "Fallback method did not complete within the expected time" ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackLaunchSucceeds_LogsSuccess ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Information ( "Successfully opened URL using fallback method" ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackLaunchReturnsNonZeroExitCode_LogsError ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 1 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Error ( "Fallback method failed with exit code {ExitCode}" , ( int? ) 1 ) ;
    }

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackLaunchThrows_LogsError ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        var ex = new InvalidOperationException ( "fallback boom" ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) ).Throws ( ex ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Error ( ex , "Fallback method also failed" ) ;
    }

    // ── Outer exception handling ──────────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_WhenUnexpectedExceptionOccurs_LogsError ( )
    {
        // Arrange
        var ex = new InvalidOperationException ( "unexpected" ) ;
        _configuration [ "DonateUrl" ].Throws ( ex ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _logger.Received ( 1 ).Error ( ex , "An error occurred while trying to open the donate URL." ) ;
    }

    // ── Fallback uses correct timeout ─────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_WhenFallbackIsTriggered_PassesCorrectTimeoutToStartAndWait ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _processLauncher.Received ( 1 ).StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , 5000 ) ;
    }

    // ── Primary uses UseShellExecute ─────────────────────────────────────────

    [ Fact ]
    public void OpenDonateUrl_PrimaryLaunch_UsesShellExecuteTrue ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( true ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _processLauncher.Received ( 1 ).TryStart (
            Arg.Is < ProcessStartInfo > ( psi => psi.UseShellExecute ) ) ;
    }

    // ── Fallback uses cmd.exe with correct arguments ──────────────────────────

    [ Fact ]
    public void OpenDonateUrl_FallbackLaunch_UsesCmdWithUrlInArguments ( )
    {
        // Arrange
        _configuration [ "DonateUrl" ].Returns ( ValidConfiguredUrl ) ;
        _processLauncher.TryStart ( Arg.Any < ProcessStartInfo > ( ) ).Returns ( false ) ;
        _processLauncher.StartAndWait ( Arg.Any < ProcessStartInfo > ( ) , Arg.Any < int > ( ) )
                        .Returns ( ( true , ( int? ) 0 ) ) ;

        var sut = CreateSut ( ) ;

        // Act
        sut.OpenDonateUrl ( ) ;

        // Assert
        _processLauncher.Received ( 1 ).StartAndWait (
            Arg.Is < ProcessStartInfo > ( psi =>
                psi.FileName.EndsWith ( "cmd.exe" , StringComparison.OrdinalIgnoreCase ) &&
                psi.Arguments.Contains ( ValidConfiguredUrl ) &&
                ! psi.UseShellExecute &&
                psi.CreateNoWindow ) ,
            5000 ) ;
    }
}
