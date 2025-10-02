using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Idasen.SystemTray.Win11.Views.Controls
{
    public partial class DeskPositionControl : UserControl
    {
        public DeskPositionControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(SymbolRegular), typeof(DeskPositionControl), new PropertyMetadata(SymbolRegular.ArrowCircleUp24));

        public SymbolRegular Symbol
        {
            get => (SymbolRegular)GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        public static readonly DependencyProperty PositionNameProperty =
            DependencyProperty.Register(nameof(PositionName), typeof(string), typeof(DeskPositionControl), new PropertyMetadata("Stand"));

        public string PositionName
        {
            get => (string)GetValue(PositionNameProperty);
            set => SetValue(PositionNameProperty, value);
        }

        public static readonly DependencyProperty PositionValueProperty =
            DependencyProperty.Register(nameof(PositionValue), typeof(double), typeof(DeskPositionControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PositionValue
        {
            get => (double)GetValue(PositionValueProperty);
            set => SetValue(PositionValueProperty, value);
        }

        public static readonly DependencyProperty IsVisibleInContextMenuProperty =
            DependencyProperty.Register(nameof(IsVisibleInContextMenu), typeof(bool), typeof(DeskPositionControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsVisibleInContextMenu
        {
            get => (bool)GetValue(IsVisibleInContextMenuProperty);
            set => SetValue(IsVisibleInContextMenuProperty, value);
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(DeskPositionControl), new PropertyMetadata(65.0));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(DeskPositionControl), new PropertyMetadata(127.0));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private void OnIncreaseClicked(object sender, RoutedEventArgs e)
        {
            if (PositionValue < Maximum)
                PositionValue++;
        }

        private void OnDecreaseClicked(object sender, RoutedEventArgs e)
        {
            if (PositionValue > Minimum)
                PositionValue--;
        }
    }
}