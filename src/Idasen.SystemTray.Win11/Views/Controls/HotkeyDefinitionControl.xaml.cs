using System.Diagnostics.CodeAnalysis ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Controls ;

[ ExcludeFromCodeCoverage ]
public partial class HotkeyDefinitionControl
{
    public static readonly DependencyProperty SymbolProperty =
        DependencyProperty.Register ( nameof ( Symbol ) ,
                                      typeof ( SymbolRegular ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new PropertyMetadata ( SymbolRegular.ArrowCircleUp24 ) ) ;

    public static readonly DependencyProperty HotkeyNameProperty =
        DependencyProperty.Register ( nameof ( HotkeyName ) ,
                                      typeof ( string ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new PropertyMetadata ( "Stand" ) ) ;

    public static readonly DependencyProperty HotkeyTooltipProperty =
        DependencyProperty.Register ( nameof ( HotkeyTooltip ) ,
                                      typeof ( string ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new PropertyMetadata ( string.Empty ) ) ;

    public static readonly DependencyProperty AvailableKeysProperty =
        DependencyProperty.Register ( nameof ( AvailableKeys ) ,
                                      typeof ( IReadOnlyList < string > ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new PropertyMetadata ( null ) ) ;

    public static readonly DependencyProperty SelectedKeyProperty =
        DependencyProperty.Register ( nameof ( SelectedKey ) ,
                                      typeof ( string ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new FrameworkPropertyMetadata ( string.Empty ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public static readonly DependencyProperty KeyTooltipProperty =
        DependencyProperty.Register ( nameof ( KeyTooltip ) ,
                                      typeof ( string ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new PropertyMetadata ( "Select the key for this hotkey" ) ) ;

    public static readonly DependencyProperty IsControlCheckedProperty =
        DependencyProperty.Register ( nameof ( IsControlChecked ) ,
                                      typeof ( bool ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new FrameworkPropertyMetadata ( false ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public static readonly DependencyProperty IsAltCheckedProperty =
        DependencyProperty.Register ( nameof ( IsAltChecked ) ,
                                      typeof ( bool ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new FrameworkPropertyMetadata ( false ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public static readonly DependencyProperty IsShiftCheckedProperty =
        DependencyProperty.Register ( nameof ( IsShiftChecked ) ,
                                      typeof ( bool ) ,
                                      typeof ( HotkeyDefinitionControl ) ,
                                      new FrameworkPropertyMetadata ( false ,
                                                                      FrameworkPropertyMetadataOptions
                                                                         .BindsTwoWayByDefault ) ) ;

    public HotkeyDefinitionControl ( )
    {
        InitializeComponent ( ) ;
    }

    public SymbolRegular Symbol
    {
        get => ( SymbolRegular )GetValue ( SymbolProperty ) ;
        set => SetValue ( SymbolProperty ,
                          value ) ;
    }

    public string HotkeyName
    {
        get => ( string )GetValue ( HotkeyNameProperty ) ;
        set => SetValue ( HotkeyNameProperty ,
                          value ) ;
    }

    public string HotkeyTooltip
    {
        get => ( string )GetValue ( HotkeyTooltipProperty ) ;
        set => SetValue ( HotkeyTooltipProperty ,
                          value ) ;
    }

    public IReadOnlyList < string > AvailableKeys
    {
        get => ( IReadOnlyList < string > )GetValue ( AvailableKeysProperty ) ;
        set => SetValue ( AvailableKeysProperty ,
                          value ) ;
    }

    public string SelectedKey
    {
        get => ( string )GetValue ( SelectedKeyProperty ) ;
        set => SetValue ( SelectedKeyProperty ,
                          value ) ;
    }

    public string KeyTooltip
    {
        get => ( string )GetValue ( KeyTooltipProperty ) ;
        set => SetValue ( KeyTooltipProperty ,
                          value ) ;
    }

    public bool IsControlChecked
    {
        get => ( bool )GetValue ( IsControlCheckedProperty ) ;
        set => SetValue ( IsControlCheckedProperty ,
                          value ) ;
    }

    public bool IsAltChecked
    {
        get => ( bool )GetValue ( IsAltCheckedProperty ) ;
        set => SetValue ( IsAltCheckedProperty ,
                          value ) ;
    }

    public bool IsShiftChecked
    {
        get => ( bool )GetValue ( IsShiftCheckedProperty ) ;
        set => SetValue ( IsShiftCheckedProperty ,
                          value ) ;
    }
}
