﻿using System.Diagnostics.CodeAnalysis ;
using System.Windows.Controls ;
using System.Windows.Input ;
using System.Windows.Media ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsPage : INavigableView < SettingsViewModel >
{
    public SettingsPage ( SettingsViewModel viewModel )
    {
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
        // Try to scroll the nearest hosting ScrollViewer (e.g., DynamicScrollViewer in Navigation host)
        var hostScrollViewer = FindParentScrollViewer ( this ) ?? MainScrollViewer ;
        if ( hostScrollViewer is null ) return ;

        // Scroll 3 lines per wheel notch to match the standard Windows scroll speed
        // and the comfortable speed on window sides/other pages
        const int steps = 3 ;

        var delta = e.Delta ;
        if ( delta > 0 )
        {
            for ( var i = 0 ; i < steps ; i ++ ) hostScrollViewer.LineUp ( ) ;
        }
        else if ( delta < 0 )
        {
            for ( var i = 0 ; i < steps ; i ++ ) hostScrollViewer.LineDown ( ) ;
        }

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
}