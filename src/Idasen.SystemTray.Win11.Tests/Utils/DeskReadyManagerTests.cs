using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using Wpf.Ui.Controls ;

#pragma warning disable CA2012    // ValueTask verification in tests
#pragma warning disable xUnit1051 // CancellationToken matching is behavior under test here

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class DeskReadyManagerTests
{
    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        var settingsManager     = Substitute.For < ISettingsManager > ( ) ;
        var iconProvider        = Substitute.For < ITaskbarIconProvider > ( ) ;
        var notificationManager = Substitute.For < IDeskNotificationManager > ( ) ;
        var connectionManager   = Substitute.For < IDeskConnectionManager > ( ) ;

        var act = ( ) => new DeskReadyManager ( null ! ,
                                                settingsManager ,
                                                iconProvider ,
                                                notificationManager ,
                                                null ,
                                                connectionManager ) ;

        act.Should ( )
           .Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public async Task OnDeskReady_InitializesIconProvider_AndShowsConnectedNotification ( )
    {
        using var context = new TestHarness ( ) ;
        context.Desk.DeviceName.Returns ( "Desk A" ) ;

        context.Sut.OnDeskReady ( context.Desk ) ;

        context.IconProvider.Received ( 1 )
               .Initialize ( context.Logger ,
                             context.Desk ,
                             null ) ;

        context.NotificationManager.Received ( 1 )
               .ShowStatusUpdate ( 0 ,
                                   "Connected" ,
                                   "Connected successfully to 'Desk A'." ,
                                   InfoBarSeverity.Success ) ;

        await Task.CompletedTask ;
    }

    [ Fact ]
    public async Task OnDeskReady_FinishedChanged_ShowsFinishedHeight_PersistsHeight_AndResetsActivityTimer ( )
    {
        using var context = new TestHarness ( ) ;
        context.Sut.OnDeskReady ( context.Desk ) ;
        context.ClearReceivedCalls ( ) ;

        context.FinishedChanged.OnNext ( 7550 ) ;

        await EventuallyAsync ( async ( ) =>
                                {
                                    context.ConnectionMonitor.Received ( 1 )
                                           .ResetActivityTimer ( ) ;

                                    context.NotificationManager.Received ( 1 )
                                           .ShowStatusUpdate ( 76 ,
                                                               "Finished" ,
                                                               "Current Height: 76 cm" ,
                                                               InfoBarSeverity.Success ) ;

                                    await context.SettingsManager.Received ( 1 )
                                                 .SetLastKnownDeskHeight ( 76 ,
                                                                           Arg.Any < CancellationToken > ( ) ) ;
                                } ) ;
    }

    [ Fact ]
    public async Task OnDeskReady_HeightChanged_ShowsThrottledHeight_PersistsHeight_AndResetsActivityTimer ( )
    {
        using var context = new TestHarness ( ) ;
        context.Sut.OnDeskReady ( context.Desk ) ;
        context.ClearReceivedCalls ( ) ;

        context.HeightChanged.OnNext ( 8250 ) ;

        await Task.Delay ( TimeSpan.FromMilliseconds ( 1200 ) ,
                           TestContext.Current.CancellationToken ) ;

        await EventuallyAsync ( async ( ) =>
                                {
                                    context.ConnectionMonitor.Received ( 1 )
                                           .ResetActivityTimer ( ) ;

                                    context.NotificationManager.Received ( 1 )
                                           .ShowStatusUpdate ( 83 ,
                                                               "Height Changed" ,
                                                               "Current Height: 83 cm" ,
                                                               InfoBarSeverity.Warning ) ;

                                    await context.SettingsManager.Received ( 1 )
                                                 .SetLastKnownDeskHeight ( 83 ,
                                                                           Arg.Any < CancellationToken > ( ) ) ;
                                } ) ;
    }

    [ Fact ]
    public async Task OnDeskReady_WhenCalledAgain_DisposesPreviousDeskSubscriptions ( )
    {
        using var originalContext    = new TestHarness ( ) ;
        using var replacementContext = new TestHarness ( ) ;

        originalContext.Sut.OnDeskReady ( originalContext.Desk ) ;
        originalContext.Sut.OnDeskReady ( replacementContext.Desk ) ;
        originalContext.ClearReceivedCalls ( ) ;

        originalContext.FinishedChanged.OnNext ( 7500 ) ;
        originalContext.HeightChanged.OnNext ( 8200 ) ;

        await Task.Delay ( TimeSpan.FromMilliseconds ( 1200 ) ,
                           TestContext.Current.CancellationToken ) ;

        originalContext.ConnectionMonitor.DidNotReceive ( )
                       .ResetActivityTimer ( ) ;
        originalContext.NotificationManager.DidNotReceiveWithAnyArgs ( )
                       .ShowStatusUpdate ( 0 ,
                                           string.Empty ,
                                           string.Empty ,
                                           default ) ;
        await originalContext.SettingsManager.DidNotReceiveWithAnyArgs ( )
                             .SetLastKnownDeskHeight ( 0 ,
                                                       Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task Dispose_DisposesSubscriptions ( )
    {
        using var context = new TestHarness ( ) ;
        context.Sut.OnDeskReady ( context.Desk ) ;
        context.Sut.Dispose ( ) ;
        context.ClearReceivedCalls ( ) ;

        context.FinishedChanged.OnNext ( 7600 ) ;
        context.HeightChanged.OnNext ( 8100 ) ;

        await Task.Delay ( TimeSpan.FromMilliseconds ( 1200 ) ,
                           TestContext.Current.CancellationToken ) ;

        context.ConnectionMonitor.DidNotReceive ( )
               .ResetActivityTimer ( ) ;
        context.NotificationManager.DidNotReceiveWithAnyArgs ( )
               .ShowStatusUpdate ( 0 ,
                                   string.Empty ,
                                   string.Empty ,
                                   default ) ;
        await context.SettingsManager.DidNotReceiveWithAnyArgs ( )
                     .SetLastKnownDeskHeight ( 0 ,
                                               Arg.Any < CancellationToken > ( ) ) ;
    }

    private static async Task EventuallyAsync ( Func < Task > assertion ,
                                                int           timeoutMs = 2000 ,
                                                int           pollMs    = 25 )
    {
        var         timeout       = DateTime.UtcNow.AddMilliseconds ( timeoutMs ) ;
        Exception ? lastException = null ;

        while ( DateTime.UtcNow < timeout )
            try
            {
                await assertion ( ) ;
                return ;
            }
            catch ( Exception ex )
            {
                lastException = ex ;
                await Task.Delay ( pollMs ,
                                   TestContext.Current.CancellationToken ) ;
            }

        if ( lastException != null )
            throw lastException ;

        throw new InvalidOperationException ( "Assertion did not succeed before timeout." ) ;
    }

    private sealed class TestHarness : IDisposable
    {
        public TestHarness ( )
        {
            Desk.FinishedChanged.Returns ( FinishedChanged ) ;
            Desk.HeightChanged.Returns ( HeightChanged ) ;
            ConnectionManager.ConnectionMonitor.Returns ( ConnectionMonitor ) ;

            Sut = new DeskReadyManager ( Logger ,
                                         SettingsManager ,
                                         IconProvider ,
                                         NotificationManager ,
                                         null ,
                                         ConnectionManager ) ;
        }

        public InMemoryLogger       Logger          { get ; } = new( ) ;
        public ISettingsManager     SettingsManager { get ; } = Substitute.For < ISettingsManager > ( ) ;
        public ITaskbarIconProvider IconProvider    { get ; } = Substitute.For < ITaskbarIconProvider > ( ) ;

        public IDeskNotificationManager NotificationManager { get ; } =
            Substitute.For < IDeskNotificationManager > ( ) ;

        public IDeskConnectionManager ConnectionManager { get ; } = Substitute.For < IDeskConnectionManager > ( ) ;

        public IBluetoothConnectionMonitor ConnectionMonitor { get ; } =
            Substitute.For < IBluetoothConnectionMonitor > ( ) ;

        public IDesk            Desk            { get ; } = Substitute.For < IDesk > ( ) ;
        public Subject < uint > FinishedChanged { get ; } = new( ) ;
        public Subject < uint > HeightChanged   { get ; } = new( ) ;
        public DeskReadyManager Sut             { get ; }

        public void Dispose ( )
        {
            Sut.Dispose ( ) ;
            FinishedChanged.Dispose ( ) ;
            HeightChanged.Dispose ( ) ;
        }

        public void ClearReceivedCalls ( )
        {
            ConnectionMonitor.ClearReceivedCalls ( ) ;
            NotificationManager.ClearReceivedCalls ( ) ;
            SettingsManager.ClearReceivedCalls ( ) ;
            IconProvider.ClearReceivedCalls ( ) ;
        }
    }
}
