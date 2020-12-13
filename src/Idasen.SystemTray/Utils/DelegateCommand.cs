using System ;
using System.Windows.Input ;

namespace Idasen.SystemTray.Utils
{
    /// <summary>
    ///     Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public void Execute ( object parameter )
        {
            CommandAction ( ) ;
        }

        public bool CanExecute ( object parameter )
        {
            return CanExecuteFunc == null || CanExecuteFunc ( ) ;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value ;
            remove => CommandManager.RequerySuggested -= value ;
        }

        public Action        CommandAction  { get ; set ; }
        public Func < bool > CanExecuteFunc { get ; set ; }
    }
}