using System.Diagnostics.CodeAnalysis ;
using System.Windows.Controls ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using System.Windows.Media ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Serilog ;
using Serilog.Events ;
using Wpf.Ui.Abstractions.Controls ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsPage : INavigableView < SettingsViewModel >
{
    private readonly ILogger _logger ;

    public SettingsPage ( SettingsViewModel viewModel , ILogger logger )
    {
        _logger     = logger ;
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;

        // Capture mouse wheel events even if already handled by child controls
        AddHandler ( PreviewMouseWheelEvent ,
                     new MouseWheelEventHandler ( OnPreviewMouseWheel ) ,
                     true ) ;
    }

    public SettingsViewModel ViewModel { get ; }

    private void OnPreviewMouseWheel ( object sender , MouseWheelEventArgs e )
    {
        // Check if the event originated from or over a ComboBox or its parts
        var elementUnderMouse = Mouse.DirectlyOver as DependencyObject ;
        var originalSource = e.OriginalSource as DependencyObject ;

        // Guard debug logging and associated popup lookups behind level check
        if ( _logger.IsEnabled ( LogEventLevel.Debug ) )
        {
            // Log element types for debugging
            _logger.Debug ( "Mouse wheel: ElementUnderMouse={ElementType}, OriginalSource={SourceType}" ,
                            elementUnderMouse?.GetType().Name ?? "null" ,
                            originalSource?.GetType().Name ?? "null" ) ;

            // Check for popup (only for logging purposes)
            var mousePopup = FindAssociatedPopup ( elementUnderMouse ) ;
            var sourcePopup = FindAssociatedPopup ( originalSource ) ;

            if ( mousePopup is not null )
            {
                _logger.Debug ( "Mouse popup found: PlacementTarget={TargetType}" ,
                                mousePopup.PlacementTarget?.GetType().Name ?? "null" ) ;
            }

            if ( sourcePopup is not null )
            {
                _logger.Debug ( "Source popup found: PlacementTarget={TargetType}" ,
                                sourcePopup.PlacementTarget?.GetType().Name ?? "null" ) ;
            }
        }

        // Check both the element under mouse and the original source
        var insideComboBox = IsInsideComboBox ( elementUnderMouse ) || IsInsideComboBox ( originalSource ) ;

        if ( _logger.IsEnabled ( LogEventLevel.Debug ) )
        {
            _logger.Debug ( "InsideComboBox={Inside}" , insideComboBox ) ;
        }

        if ( insideComboBox )
        {
            if ( _logger.IsEnabled ( LogEventLevel.Debug ) )
            {
                _logger.Debug ( "Returning early - inside ComboBox" ) ;
            }
            // Let the ComboBox handle its own scrolling
            return ;
        }

        // Try to scroll the nearest hosting ScrollViewer (e.g., DynamicScrollViewer in Navigation host)
        var hostScrollViewer = FindParentScrollViewer ( this ) ?? MainScrollViewer ;
        if ( hostScrollViewer is null ) return ;

        // Detect whether the wheel event originated over an expander control
        var source = e.OriginalSource as DependencyObject ;
        var overExpander = FindParentOfType < CardExpander > ( source ) is not null ||
                           FindParentOfType < Expander > ( source ) is not null ;

        // Increase scroll speed when over expander controls
        var steps = overExpander
                        ? 3
                        : 1 ;

        var delta = e.Delta ;
        if ( delta > 0 )
            for ( var i = 0 ; i < steps ; i ++ )
                hostScrollViewer.LineUp ( ) ;
        else if ( delta < 0 )
            for ( var i = 0 ; i < steps ; i ++ )
                hostScrollViewer.LineDown ( ) ;

        e.Handled = true ;
    }

    private static ScrollViewer ? FindParentScrollViewer ( DependencyObject start )
    {
        var current = start ;
        while ( current != null )
        {
            current = VisualTreeHelper.GetParent ( current ) ;
            if ( current is ScrollViewer sv ) return sv ;
        }

        return null ;
    }

    private static T ? FindParentOfType < T > ( DependencyObject ? start )
        where T : DependencyObject
    {
        var current = start ;
        while ( current != null )
        {
            if ( current is T t ) return t ;
            current = VisualTreeHelper.GetParent ( current ) ;
        }

        return null ;
    }

    private static Popup ? FindAssociatedPopup ( DependencyObject ? element )
    {
        if ( element is null ) return null ;

        // Try to find Popup in visual tree first
        var popup = FindParentOfType < Popup > ( element ) ;
        if ( popup is not null ) return popup ;

        // For elements inside PopupRoot (separate visual tree), we need to walk up to the root
        // then check the logical parent which should be the Popup
        var                current    = element ;
        DependencyObject ? visualRoot = null ;

        var maxDepth = 0 ; // Prevent infinite loops in case of malformed visual trees

        // Walk up the visual tree to find the root
        while ( maxDepth < 100 )
        {
            visualRoot = current ;
            var parent = VisualTreeHelper.GetParent ( current ) ;
            if ( parent is null )
            {
                // We've reached the visual root
                break ;
            }

            current = parent ;
            maxDepth ++ ;
        }

        // Check if the visual root has a Popup as its logical parent
        if ( visualRoot is FrameworkElement { Parent: Popup popupParent1 } ) return popupParent1 ;

        // Try LogicalTreeHelper
        var logicalParent = LogicalTreeHelper.GetParent ( visualRoot! ) ;

        if ( logicalParent is Popup popupParent2 )
            return popupParent2 ;

        return null ;
    }

    private static bool IsInsideComboBox ( DependencyObject ? element )
    {
        if ( element is null ) return false ;

        // Direct check if element is a ComboBox
        if ( element is ComboBox ) return true ;

        // Check if element is inside a ComboBox or ComboBoxItem in the main visual tree
        if ( FindParentOfType < ComboBox > ( element ) is not null ) return true ;
        if ( FindParentOfType < ComboBoxItem > ( element ) is not null ) return true ;

        // Check if we're inside a Popup that belongs to a ComboBox
        // ComboBox dropdowns are rendered in a Popup (separate visual tree)
        var popup = FindAssociatedPopup ( element ) ;
        if ( popup is not null )
        {
            // The PlacementTarget might be a Border or other element inside the ComboBox
            // So we need to check if the PlacementTarget itself is inside a ComboBox
            if ( popup.PlacementTarget is ComboBox ) return true ;
            if ( FindParentOfType < ComboBox > ( popup.PlacementTarget ) is not null ) return true ;
        }

        return false ;
    }
}
