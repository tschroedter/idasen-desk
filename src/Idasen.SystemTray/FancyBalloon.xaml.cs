using System ;
using System.Windows ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Interaction logic for FancyBalloon.xaml
    /// </summary>
    public partial class FancyBalloon
    {
        public FancyBalloon ( )
        {
            InitializeComponent ( ) ;
            TaskbarIcon.AddBalloonClosingHandler ( this ,
                                                   OnBalloonClosing ) ;
        }

        public static readonly DependencyProperty BalloonTitleProperty =
            DependencyProperty.Register ( "BalloonTitle" ,
                                          typeof ( string ) ,
                                          typeof ( FancyBalloon ) ,
                                          new FrameworkPropertyMetadata ( "" ) ) ;

        public static readonly DependencyProperty BalloonTextProperty =
            DependencyProperty.Register ( "BalloonText" ,
                                          typeof ( string ) ,
                                          typeof ( FancyBalloon ) ,
                                          new FrameworkPropertyMetadata ( "" ) ) ;

        public static readonly DependencyProperty VisibilityBulbGreenProperty =
            DependencyProperty.Register ( "VisibilityBulbGreen" ,
                                          typeof ( Visibility ) ,
                                          typeof ( FancyBalloon ) ,
                                          new FrameworkPropertyMetadata ( Visibility.Hidden ) ) ;

        public static readonly DependencyProperty VisibilityBulbYellowProperty =
            DependencyProperty.Register ( "VisibilityBulbYellow" ,
                                          typeof ( Visibility ) ,
                                          typeof ( FancyBalloon ) ,
                                          new FrameworkPropertyMetadata ( Visibility.Hidden ) ) ;

        public static readonly DependencyProperty VisibilityBulbRedProperty =
            DependencyProperty.Register ( "VisibilityBulbRed" ,
                                          typeof ( Visibility ) ,
                                          typeof ( FancyBalloon ) ,
                                          new FrameworkPropertyMetadata ( Visibility.Hidden ) ) ;

        public string BalloonTitle
        {
            get => ( string ) GetValue ( BalloonTitleProperty ) ;
            set => SetValue ( BalloonTitleProperty ,
                              value ) ;
        }

        public string BalloonText
        {
            get => ( string ) GetValue ( BalloonTextProperty ) ;
            set => SetValue ( BalloonTextProperty ,
                              value ) ;
        }

        public Visibility VisibilityBulbGreen
        {
            get => ( Visibility ) GetValue ( VisibilityBulbGreenProperty ) ;
            set => SetValue ( VisibilityBulbGreenProperty ,
                              value ) ;
        }

        public Visibility VisibilityBulbYellow
        {
            get => ( Visibility ) GetValue ( VisibilityBulbYellowProperty ) ;
            set => SetValue ( VisibilityBulbYellowProperty ,
                              value ) ;
        }

        public Visibility VisibilityBulbRed
        {
            get => ( Visibility ) GetValue ( VisibilityBulbRedProperty ) ;
            set => SetValue ( VisibilityBulbRedProperty ,
                              value ) ;
        }


        /// <summary>
        ///     By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent" />
        ///     and setting the "Handled" property to true, we suppress the popup
        ///     from being closed in order to display the custom fade-out animation.
        /// </summary>
        private void OnBalloonClosing ( object          sender ,
                                        RoutedEventArgs e )
        {
            e.Handled  = true ; //suppresses the popup from being closed immediately
            _isClosing = true ;
        }


        /// <summary>
        ///     Resolves the <see cref="TaskbarIcon" /> that displayed
        ///     the balloon and requests a close action.
        /// </summary>
        private void ImageClose_MouseDown ( object               sender ,
                                            MouseButtonEventArgs e )
        {
            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon ( this ) ;
            taskbarIcon.CloseBalloon ( ) ;
        }

        /// <summary>
        ///     If the users hovers over the balloon, we don't close it.
        /// </summary>
        private void grid_MouseEnter ( object         sender ,
                                       MouseEventArgs e )
        {
            //if we're already running the fade-out animation, do not interrupt anymore
            //(makes things too complicated for the sample)
            if ( _isClosing ) return ;

            //the tray icon assigned this attached property to simplify access
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon ( this ) ;
            taskbarIcon.ResetBalloonCloseTimer ( ) ;
        }


        /// <summary>
        ///     Closes the popup once the fade-out animation completed.
        ///     The animation was triggered in XAML through the attached
        ///     BalloonClosing event.
        /// </summary>
        private void OnFadeOutCompleted ( object    sender ,
                                          EventArgs e )
        {
            var pp = ( Popup ) Parent ;
            pp.IsOpen = false ;
        }

        private bool _isClosing ;
    }
}