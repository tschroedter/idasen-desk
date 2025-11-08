namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IMainWindow
{
    /// <summary>
    ///     Event triggered when the visibility of the main window changes.
    /// </summary>
    IObservable < Visibility > VisibilityChanged { get ; }

    /// <summary>
    ///     Shows the main window.
    /// </summary>
    void ShowWindow ( ) ;

    /// <summary>
    ///     Closes the main window.
    /// </summary>
    void CloseWindow ( ) ;
}