using System.Diagnostics.CodeAnalysis ;
using System.Windows.Input ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Simplistic delegate command for the demo.
/// </summary>
public partial class DelegateCommand (
    Action < object ? >        execute ,
    Func < object ? , bool > ? canExecute = null ) : ICommand
{
    private readonly Func < object ? , bool > _canExecute = canExecute ?? ( _ => true ) ;
    private readonly Action < object ? > _execute = execute ?? throw new ArgumentNullException ( nameof ( execute ) ) ;

    public void Execute ( object ? parameter )
    {
        _execute.Invoke ( parameter ) ;
    }

    public bool CanExecute ( object ? parameter )
    {
        return _canExecute ( parameter ) ;
    }

    [ ExcludeFromCodeCoverage ]
    public event EventHandler ? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value ;
        remove => CommandManager.RequerySuggested -= value ;
    }
}