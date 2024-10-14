using System.Windows.Input ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Simplistic delegate command for the demo.
/// </summary>
public class DelegateCommand (
    Action        commandAction ,
    Func < bool > canExecuteFunc )
    : ICommand
{
    public Action        CommandAction  { get ; set ; } = commandAction ;
    public Func < bool > CanExecuteFunc { get ; set ; } = canExecuteFunc ;

    public void Execute ( object ? parameter )
    {
        CommandAction ( ) ;
    }

    public bool CanExecute ( object ? parameter )
    {
        return CanExecuteFunc ( ) ;
    }

    public event EventHandler ? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value ;
        remove => CommandManager.RequerySuggested -= value ;
    }
}