using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestMagnifier
{
    public class Magnifier : Canvas
    {
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            "ZoomFactor", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty ContentPanelProperty = DependencyProperty.Register(
            "ContentPanel", typeof(UIElement), typeof(Magnifier), new PropertyMetadata(default(UIElement)));

        public UIElement ContentPanel
        {
            get => (UIElement) GetValue(ContentPanelProperty);
            set => SetValue(ContentPanelProperty, value);
        }
        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }
        public double ZoomFactor
        {
            get => (double)GetValue(ZoomFactorProperty);
            set => SetValue(ZoomFactorProperty, value);
        }
        protected Rect ViewBox
        {
            get => MagnifierBrush.Viewbox;
            set => MagnifierBrush.Viewbox = value;
        }
        protected VisualBrush MagnifierBrush { get; set; }
        protected Ellipse MagnifierCircle { get; set; }
        protected Canvas MagnifierPanel { get; set; }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ContentPanelProperty)
            {
                MagnifierBrush = new VisualBrush(ContentPanel)
                {
                    ViewboxUnits = BrushMappingMode.Absolute
                };

                MagnifierCircle = new Ellipse
                {
                    Stroke = Brushes.Teal,
                    Width = 100,
                    Height = 100,
                    Visibility = Visibility.Hidden,
                    Fill = MagnifierBrush
                };

                MagnifierPanel = new Canvas
                {
                    IsHitTestVisible = false
                };
                MagnifierPanel.Children.Add(MagnifierCircle);

                var container = VisualTreeHelper.GetParent(ContentPanel) as Panel;
                container?.Children.Add(MagnifierPanel);

                ContentPanel.MouseEnter += delegate
                {
                    MagnifierCircle.Visibility = Visibility.Visible;
                };

                ContentPanel.MouseLeave += delegate
                {
                    MagnifierCircle.Visibility = Visibility.Hidden;
                };

                ContentPanel.MouseMove += ContentPanelOnMouseMove;
            }
        }

        protected void ContentPanelOnMouseMove(object sender, MouseEventArgs e)
        {
            var center = e.GetPosition(ContentPanel);
            var length = MagnifierCircle.ActualWidth * (1 / ZoomFactor);
            var radius = length / 2;
            ViewBox = new Rect(center.X - radius, center.Y - radius, length, length);
            MagnifierCircle.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }

        public Magnifier()
        {
            ZoomFactor = 3; // 3x
            Radius = 50;
        }
    }
}
