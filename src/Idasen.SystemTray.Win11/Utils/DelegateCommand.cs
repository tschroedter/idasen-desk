using System.Windows.Input ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Simplistic delegate command for the demo.
/// </summary>
public class DelegateCommand ( Action          execute ,
                               Func < bool > ? canExecute = null ) : ICommand
{
    private readonly Func < bool > _canExecute = canExecute ?? ( ( ) => true ) ;
    private readonly Action        _execute    = execute    ?? throw new ArgumentNullException ( nameof ( execute ) ) ;

    public void Execute ( object ? parameter )
    {
        _execute ( ) ;
    }

    public bool CanExecute ( object ? parameter )
    {
        return _canExecute ( ) ;
    }

    public event EventHandler ? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value ;
        remove => CommandManager.RequerySuggested -= value ;
    }
}