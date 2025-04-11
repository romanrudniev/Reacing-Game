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
        private DispatcherTimer gameTimer;
        private double speed = 0;
        private double acceleration = 340 / 2.85 / 60;
        private const double maxSpeed = 340;
        private const double minSpeed = 0;
        private double roadWidth;
        private double roadLeft;
        private Random rand = new Random();
        private bool isGameOver = false;

        public MainWindow()
        {
            InitializeComponent();
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            this.KeyDown += OnKeyDown;
            this.SizeChanged += OnSizeChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitPositions();
        }

        private void InitPositions()
        {
            double canvasWidth = MyCanvas.ActualWidth;
            double canvasHeight = MyCanvas.ActualHeight;

            double roadImageWidth = canvasWidth * 0.55;
            double roadImageHeight = canvasHeight;

            roadLeft = (canvasWidth - roadImageWidth) / 2;
            roadWidth = roadImageWidth;

            road1.Width = road2.Width = roadImageWidth;
            road1.Height = road2.Height = roadImageHeight;
            Canvas.SetLeft(road1, roadLeft);
            Canvas.SetLeft(road2, roadLeft);
            Canvas.SetTop(road1, 0);
            Canvas.SetTop(road2, -roadImageHeight);

            double carWidth = roadImageWidth * 0.18;
            double carHeight = canvasHeight * 0.2;
            car.Width = carWidth;
            car.Height = carHeight;
            Canvas.SetLeft(car, roadLeft + (roadImageWidth - carWidth) / 2);
            Canvas.SetTop(car, canvasHeight - carHeight - 20);

            double obstacleWidth = roadWidth * 0.2;
            double obstacleHeight = canvasHeight * 0.1;
            obstacle.Width = obstacleWidth;
            obstacle.Height = obstacleHeight;
            Canvas.SetLeft(obstacle, roadLeft + rand.NextDouble() * (roadWidth - obstacleWidth));
            Canvas.SetTop(obstacle, -obstacleHeight);
            obstacle.Visibility = Visibility.Visible;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InitPositions();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (isGameOver) return;

            double carLeft = Canvas.GetLeft(car);
            double carTop = Canvas.GetTop(car);
            double step = roadWidth * 0.05;

            if ((e.Key == Key.Left || e.Key == Key.A) && carLeft > roadLeft)
                Canvas.SetLeft(car, carLeft - step);
            else if ((e.Key == Key.Right || e.Key == Key.D) && carLeft + car.Width < roadLeft + roadWidth)
                Canvas.SetLeft(car, carLeft + step);

            if (e.Key == Key.Up || e.Key == Key.W)
                speed = Math.Min(speed + acceleration, maxSpeed);
            else if (e.Key == Key.Down || e.Key == Key.S)
                speed = Math.Max(speed - acceleration * 1.5, minSpeed);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            MoveRoad();
            MoveObstacle();
            UpdateSpeedDisplay();
            CheckCollision();
        }

        private void MoveRoad()
        {
            double pixelsPerFrame = (speed / 340) * 25;

            double y1 = Canvas.GetTop(road1) + pixelsPerFrame;
            double y2 = Canvas.GetTop(road2) + pixelsPerFrame;
            double height = road1.Height;

            Canvas.SetTop(road1, y1 >= height ? y2 - height : y1);
            Canvas.SetTop(road2, y2 >= height ? y1 - height : y2);
        }

        private void MoveObstacle()
        {
            if (!obstacle.IsVisible) return;

            double y = Canvas.GetTop(obstacle);
            double pixelsPerFrame = (speed / 340) * 25;
            y += pixelsPerFrame;

            if (y > MyCanvas.ActualHeight)
            {
                ResetObstacle();
                return;
            }

            Canvas.SetTop(obstacle, y);
        }

        private void ResetObstacle()
        {
            double obstacleWidth = roadWidth * 0.2;
            double obstacleHeight = MyCanvas.ActualHeight * 0.1;
            obstacle.Width = obstacleWidth;
            obstacle.Height = obstacleHeight;
            Canvas.SetLeft(obstacle, roadLeft + rand.NextDouble() * (roadWidth - obstacleWidth));
            Canvas.SetTop(obstacle, -obstacleHeight);
        }

        private void CheckCollision()
        {
            Rect carRect = new Rect(Canvas.GetLeft(car), Canvas.GetTop(car), car.Width, car.Height);
            Rect obstacleRect = new Rect(Canvas.GetLeft(obstacle), Canvas.GetTop(obstacle), obstacle.Width, obstacle.Height);

            if (carRect.IntersectsWith(obstacleRect))
            {
                gameTimer.Stop();   
                isGameOver = true;
                speedText.Text = "ДТП! Гру завершено";
            }
        }

        private void UpdateSpeedDisplay()
        {
            speedText.Text = $"Швидкість: {Math.Round(speed)} км/год";
            Canvas.SetLeft(speedText, MyCanvas.ActualWidth - speedText.ActualWidth - 10);
            Canvas.SetTop(speedText, MyCanvas.ActualHeight - speedText.ActualHeight - 10);
        }
    }
}