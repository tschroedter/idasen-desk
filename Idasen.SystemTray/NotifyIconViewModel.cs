using System.Windows ;
using System.Windows.Input ;
using Idasen.SystemTray.Interfaces ;
using JetBrains.Annotations ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Provides bindable properties and commands for the NotifyIcon. In this sample, the
    ///     view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    ///     in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel
    {
        /// <summary>
        ///     Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CanExecuteFunc = ( ) => SettingsWindow == null ,
                           CommandAction = async ( ) =>
                                           {
                                               var manager = new SettingsManager ( ) ;
                                               await manager.Load (  ) ;
                                               SettingsWindow = new SettingsWindow ( manager ) ;
                                               SettingsWindow?.Show ( ) ;
                                           }
                       } ;
            }
        }

        /// <summary>
        ///     Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                       {
                           CommandAction = ( ) =>
                                           {
                                               SettingsWindow?.Close ( ) ;
                                               SettingsWindow = null ;
                                           } ,
                           CanExecuteFunc = ( ) => SettingsWindow != null
                       } ;
            }
        }


        /// <summary>
        ///     Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get { return new DelegateCommand { CommandAction = ( ) => Application.Current.Shutdown ( ) } ; }
        }

        [ CanBeNull ]
        private ISettingsWindow SettingsWindow
        {
            get => Application.Current.MainWindow as ISettingsWindow ;
            set => Application.Current.MainWindow = value as Window ;
        }

        private readonly ISettingsManager _manager ;
    }
}