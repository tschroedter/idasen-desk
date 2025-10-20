using System.Diagnostics.CodeAnalysis ;
using Wpf.Ui.Controls ;
using System.Windows.Controls;
using System.Windows.Input;

namespace Idasen.SystemTray.Win11.Views.Controls ;

[ ExcludeFromCodeCoverage ]
public partial class DeskPositionControl
{
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register ( nameof ( Symbol ) ,
                                      typeof ( SymbolRegular ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new PropertyMetadata ( SymbolRegular.ArrowCircleUp24 ) ) ;

    public static readonly DependencyProperty IsSymbolVisibleProperty =
        DependencyProperty.Register(nameof(IsSymbolVisible),
                                    typeof(Visibility),
                                    typeof(DeskPositionControl),
                                    new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty PositionNameProperty =
        DependencyProperty.Register ( nameof ( PositionName ) ,
                                      typeof ( string ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new PropertyMetadata ( "Stand" ) ) ;

    public static readonly DependencyProperty IsPositionNameVisibleProperty =
        DependencyProperty.Register(nameof(IsPositionNameVisible),
                                    typeof(Visibility),
                                    typeof(DeskPositionControl),
                                    new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty PositionValueProperty =
        DependencyProperty.Register ( nameof ( PositionValue ) ,
                                      typeof ( double ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new FrameworkPropertyMetadata ( 0.0 ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public static readonly DependencyProperty IsVisibleInContextMenuProperty =
        DependencyProperty.Register ( nameof ( IsVisibleInContextMenu ) ,
                                      typeof ( bool ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new FrameworkPropertyMetadata ( false ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register ( nameof ( Minimum ) ,
                                      typeof ( double ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new PropertyMetadata ( 65.0 ) ) ;

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register ( nameof ( Maximum ) ,
                                      typeof ( double ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new PropertyMetadata ( 127.0 ) ) ;

    public static readonly DependencyProperty UnitsProperty =
        DependencyProperty.Register ( nameof ( Units ) ,
                                      typeof ( string ) ,
                                      typeof ( DeskPositionControl ) ,
                                      new PropertyMetadata ( "cm") ) ;

    public static readonly DependencyProperty IsShowInTrayVisibleProperty =
        DependencyProperty.Register( nameof(IsShowInTrayVisible),
                                     typeof(Visibility),
                                     typeof(DeskPositionControl),
                                     new PropertyMetadata(Visibility.Visible));

    public DeskPositionControl ( )
    {
        InitializeComponent ( ) ;
    }

    public SymbolRegular Symbol
    {
        get => ( SymbolRegular )GetValue ( SymbolProperty ) ;
        set => SetValue ( SymbolProperty ,
                          value ) ;
    }

    public Visibility IsSymbolVisible
    {
        get => ( Visibility )GetValue ( IsSymbolVisibleProperty ) ;
        set => SetValue ( IsSymbolVisibleProperty ,
                          value ) ;
    }

    public string PositionName
    {
        get => ( string )GetValue ( PositionNameProperty ) ;
        set => SetValue ( PositionNameProperty ,
                          value ) ;
    }

    public Visibility IsPositionNameVisible
    {
        get => (Visibility)GetValue(IsPositionNameVisibleProperty);
        set => SetValue(IsPositionNameVisibleProperty,
                        value);
    }

    public double PositionValue
    {
        get => ( double )GetValue ( PositionValueProperty ) ;
        set => SetValue ( PositionValueProperty ,
                          value ) ;
    }

    public bool IsVisibleInContextMenu
    {
        get => ( bool )GetValue ( IsVisibleInContextMenuProperty ) ;
        set => SetValue ( IsVisibleInContextMenuProperty ,
                          value ) ;
    }

    public double Minimum
    {
        get => ( double )GetValue ( MinimumProperty ) ;
        set => SetValue ( MinimumProperty ,
                          value ) ;
    }

    public double Maximum
    {
        get => ( double )GetValue ( MaximumProperty ) ;
        set => SetValue ( MaximumProperty ,
                          value ) ;
    }

    public string Units
    {
        get => (string)GetValue(UnitsProperty);
        set => SetValue(UnitsProperty,
                        value);
    }

    public Visibility IsShowInTrayVisible
    {
        get => ( Visibility )GetValue ( IsShowInTrayVisibleProperty ) ;
        set => SetValue ( IsShowInTrayVisibleProperty ,
                          value ) ;
    }

    private void OnIncreaseClicked ( object sender , RoutedEventArgs e )
    {
        if ( PositionValue < Maximum )
            PositionValue ++ ;
    }

    private void OnDecreaseClicked ( object sender , RoutedEventArgs e )
    {
        if ( PositionValue > Minimum )
            PositionValue -- ;
    }

    private void OnSliderPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is Slider slider)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (slider.Value < slider.Maximum)
                        slider.Value++;
                    break;
                case Key.Down:
                    if (slider.Value > slider.Minimum)
                        slider.Value--;
                    break;
            }

            // Update the PositionValue property to reflect the slider's value
            PositionValue = slider.Value;
        }
    }
}