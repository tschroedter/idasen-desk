using System.Diagnostics ;
using Idasen.SystemTray.Win11.Services ;

namespace Idasen.SystemTray.Win11.Tests.Services ;

public sealed class ProcessLauncherTests
{
    [ Theory ]
    [ InlineData ( - 2 ) ]
    [ InlineData ( int.MinValue ) ]
    public void StartAndWait_ThrowsArgumentOutOfRangeException_WhenTimeoutIsLessThanInfinite ( int timeoutMs )
    {
        var sut       = new ProcessLauncher ( ) ;
        var startInfo = new ProcessStartInfo ( "process-that-does-not-exist" ) ;

        var exception = Assert.Throws < ArgumentOutOfRangeException > ( ( ) => sut.StartAndWait ( startInfo ,
                                                                                                  timeoutMs ) ) ;

        Assert.Equal ( "timeoutMs" ,
                       exception.ParamName ) ;
    }
}
