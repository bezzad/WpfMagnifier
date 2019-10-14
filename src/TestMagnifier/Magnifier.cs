using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestMagnifier
{
    public class Magnifier : Canvas
    {
        public Magnifier()
        {
            ZoomFactor = 3; // 3x
            Radius = 50;
            Stroke = Brushes.Teal;

            MagnifierPanel = new Canvas
            {
                IsHitTestVisible = false
            };
        }


        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            "ZoomFactor", typeof(double), typeof(Magnifier), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(Magnifier), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty ContentPanelProperty = DependencyProperty.Register(
            "ContentPanel", typeof(UIElement), typeof(Magnifier), new PropertyMetadata(default(UIElement)));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            "Stroke", typeof(SolidColorBrush), typeof(Magnifier), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush Stroke
        {
            get => (SolidColorBrush) GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        public UIElement ContentPanel
        {
            get => (UIElement)GetValue(ContentPanelProperty);
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
                if (VisualTreeHelper.GetParent(ContentPanel) is Panel container)
                {
                    MagnifierBrush = new VisualBrush(ContentPanel)
                    {
                        ViewboxUnits = BrushMappingMode.Absolute
                    };

                    MagnifierCircle = new Ellipse
                    {
                        Stroke = Stroke,
                        Width = 2 * Radius,
                        Height = 2 * Radius,
                        Visibility = Visibility.Hidden,
                        Fill = MagnifierBrush
                    };

                    MagnifierPanel.Children.Add(MagnifierCircle);
                    container.Children.Add(MagnifierPanel);
                    ContentPanel.MouseEnter += delegate { MagnifierCircle.Visibility = Visibility.Visible; };
                    ContentPanel.MouseLeave += delegate { MagnifierCircle.Visibility = Visibility.Hidden; };
                    ContentPanel.MouseMove += ContentPanelOnMouseMove;
                }
            }
            else if (e.Property == RadiusProperty)
            {
                if (MagnifierCircle != null)
                {
                    MagnifierCircle.Width = 2 * Radius;
                    MagnifierCircle.Height = 2 * Radius;
                }
            }
        }

        protected void ContentPanelOnMouseMove(object sender, MouseEventArgs e)
        {
            var center = e.GetPosition(ContentPanel);
            var length = MagnifierCircle.ActualWidth * (1 / ZoomFactor);
            var radius = length / 2;
            ViewBox = new Rect(center.X - radius, center.Y - radius, length, length);
            MagnifierCircle.SetValue(LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }
    }
}
