using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ReacingGameDemo1
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer roadTimer;
        private bool isRoad1Visible = true;
        private double carSpeed = 20;

        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(OnWindowSizeChanged);
            this.KeyDown += new KeyEventHandler(OnKeyDown);

            roadTimer = new DispatcherTimer();
            roadTimer.Interval = TimeSpan.FromSeconds(0.2);
            roadTimer.Tick += RoadTimer_Tick;
            roadTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CenterCar();
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterCar();
        }

        private void CenterCar()
        {
            double canvasWidth = MyCanvas.ActualWidth;
            double canvasHeight = MyCanvas.ActualHeight;

            double carWidth = car.ActualWidth;
            double carHeight = car.ActualHeight;

            Canvas.SetLeft(car, (canvasWidth - carWidth) / 2);
            Canvas.SetTop(car, canvasHeight - carHeight - 30);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            double carLeft = Canvas.GetLeft(car);

            if (e.Key == Key.Left && carLeft > 0)
            {
                Canvas.SetLeft(car, carLeft - carSpeed);
            }
            else if (e.Key == Key.Right && carLeft + car.ActualWidth < MyCanvas.ActualWidth)
            {
                Canvas.SetLeft(car, carLeft + carSpeed);
            }
            else if (e.Key == Key.A && carLeft > 0)
            {
                Canvas.SetLeft(car, carLeft - carSpeed);
            }
            else if (e.Key == Key.D && carLeft + car.ActualWidth < MyCanvas.ActualWidth)
            {
                Canvas.SetLeft(car, carLeft + carSpeed);
            }
        }

        private void RoadTimer_Tick(object sender, EventArgs e)
        {
            isRoad1Visible = !isRoad1Visible;
            road1.Visibility = isRoad1Visible ? Visibility.Visible : Visibility.Hidden;
            road2.Visibility = isRoad1Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}