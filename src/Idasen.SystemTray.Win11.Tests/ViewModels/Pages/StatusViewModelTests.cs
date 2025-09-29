using System.Reactive.Subjects ;
using FluentAssertions ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Microsoft.Reactive.Testing ;
using NSubstitute ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Tests.ViewModels.Pages ;

public class StatusViewModelTests
    : IDisposable
{
    private readonly IUiDeskManager            _manager ;
    private readonly TestScheduler             _scheduler ;
    private readonly Subject < StatusBarInfo > _statusBarInfoSubject ;
    private readonly ITimer                    _timer ;

    public StatusViewModelTests ( )
    {
        _manager              = Substitute.For < IUiDeskManager > ( ) ;
        _statusBarInfoSubject = new Subject < StatusBarInfo > ( ) ;
        _manager.StatusBarInfoChanged.Returns ( _statusBarInfoSubject ) ;
        _manager.LastStatusBarInfo.Returns ( new StatusBarInfo ( "Initial Title" ,
                                                                 100 ,
                                                                 "Initial Message" ,
                                                                 InfoBarSeverity.Informational ) ) ;
        _manager.IsConnected.Returns ( true ) ;
        _scheduler = new TestScheduler ( ) ;
        _timer     = Substitute.For < ITimer > ( ) ;
    }

    [ Fact ]
    public void Constructor_ShouldInitializeProperties ( )
    {
        using var sut = CreateSut ( ) ;

        sut.Title.Should ( ).Be ( "Desk Status" ) ;
        sut.Message.Should ( ).Be ( "Initial Message" ) ;
        sut.Height.Should ( ).Be ( 100 ) ;
        sut.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public void OnStatusBarInfoChanged_ShouldUpdateProperties ( )
    {
        using var sut = CreateSut ( ) ;

        var newInfo = new StatusBarInfo ( "Updated Title" ,
                                          120 ,
                                          "Updated Message" ,
                                          InfoBarSeverity.Warning ) ;
        _statusBarInfoSubject.OnNext ( newInfo ) ;

        _scheduler.Start ( ) ; // Simulate time passing for the timer

        sut.Message.Should ( ).Be ( "Updated Message" ) ;
        sut.Height.Should ( ).Be ( 120 ) ;
        sut.Severity.Should ( ).Be ( InfoBarSeverity.Warning ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldSetMessageAndSeverity_WhenHeightIsZero ( )
    {
        _manager.IsConnected.Returns ( true ) ;

        using var sut = CreateSut ( ) ;

        sut.Height = 0 ;
        sut.DefaultInfoBar ( ) ;

        sut.Message.Should ( ).Be ( "Can't determine desk height." ) ;
        sut.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldSetMessageAndSeverity_WhenHeightIsNonZero ( )
    {
        _manager.IsConnected.Returns ( true ) ;

        using var sut = CreateSut ( ) ;

        sut.Height = 120 ;
        sut.DefaultInfoBar ( ) ;

        sut.Message.Should ( ).Be ( "Current desk height 120 cm" ) ;
        sut.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    [ Fact ]
    public void DefaultInfoBar_ShouldNotUpdate_WhenManagerIsNotConnected ( )
    {
        _manager.IsConnected.Returns ( false ) ;

        using var sut = CreateSut ( ) ;

        sut.DefaultInfoBar ( ) ;

        sut.Message.Should ( ).Be ( "Initial Message" ) ;
        sut.Severity.Should ( ).Be ( InfoBarSeverity.Informational ) ;
    }

    private StatusViewModel CreateSut ( )
    {
        return new StatusViewModel ( _manager ,
                                     _scheduler ,
                                     TimerFactory ) ;
    }

    private ITimer TimerFactory ( TimerCallback arg1 , object ? arg2 , TimeSpan arg3 , TimeSpan arg4 )
    {
        return _timer ;
    }

    public void Dispose ( )
    {
        _manager.Dispose ( ) ;
        _statusBarInfoSubject.Dispose ( ) ;
        _timer.Dispose ( ) ;

        GC.SuppressFinalize ( this );
    }
}