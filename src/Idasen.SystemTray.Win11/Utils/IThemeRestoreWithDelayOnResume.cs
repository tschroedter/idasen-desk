namespace Idasen.SystemTray.Win11.Utils ;

public interface IThemeRestoreWithDelayOnResume
{
    Task ApplyWithDelayAsync ( CancellationToken token ) ;
}