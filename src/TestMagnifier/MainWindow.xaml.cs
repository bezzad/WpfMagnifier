using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestMagnifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            "ZoomFactor", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty RadiusProperty = DependencyProperty.Register(
            "Radius", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

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



        public MainWindow()
        {
            InitializeComponent();

            ZoomFactor = 2; // 3x
            Radius = 50;

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

            Loaded += delegate
            {
                var container = VisualTreeHelper.GetParent(ContentPanel) as Panel;
                container?.Children.Add(MagnifierPanel);
            };
        }



        private void ContentPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var center = e.GetPosition(ContentPanel);
            var length = MagnifierCircle.ActualWidth * (1 / ZoomFactor);
            var radius = length / 2;
            ViewBox = new Rect(center.X - radius, center.Y - radius, length, length);
            MagnifierCircle.SetValue(Canvas.LeftProperty, center.X - MagnifierCircle.ActualWidth / 2);
            MagnifierCircle.SetValue(Canvas.TopProperty, center.Y - MagnifierCircle.ActualHeight / 2);
        }

        private void ContentPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            MagnifierCircle.Visibility = Visibility.Visible;
        }

        private void ContentPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            MagnifierCircle.Visibility = Visibility.Hidden;
        }
    }
}
