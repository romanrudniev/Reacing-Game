﻿using System.Diagnostics;
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
        private double carSpeed = 0;
        private double maxSpeed = 340;
        private double acceleration;
        private double deceleration;
        private bool gameStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += OnWindowSizeChanged;
            this.KeyDown += OnKeyDown;

            acceleration = maxSpeed / 0.25 / 50;
            deceleration = maxSpeed / 3.4 / 50;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustRoad();
            CenterCar();
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdjustRoad();
            CenterCar();
        }

        private void AdjustRoad()
        {
            double canvasWidth = MyCanvas.ActualWidth;
            double canvasHeight = MyCanvas.ActualHeight;

            road1.Width = canvasWidth * 0.5;
            road1.Height = canvasHeight;
            road2.Width = canvasWidth * 0.5;
            road2.Height = canvasHeight;

            double roadLeft = (canvasWidth - road1.Width) / 2;

            Canvas.SetLeft(road1, roadLeft);
            Canvas.SetLeft(road2, roadLeft);
            Canvas.SetTop(road1, 0);
            Canvas.SetTop(road2, -canvasHeight);
        }

        private void CenterCar()
        {
            double roadLeft = Canvas.GetLeft(road1);
            double roadWidth = road1.Width;
            double carWidth = roadWidth * 0.2;
            double carHeight = road1.Height * 0.15;

            car.Width = carWidth;
            car.Height = carHeight;

            Canvas.SetLeft(car, roadLeft + (roadWidth - carWidth) / 2);
            Canvas.SetTop(car, MyCanvas.ActualHeight - carHeight - 30);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!gameStarted) return;

            double carLeft = Canvas.GetLeft(car);
            double carMoveSpeed = carSpeed / maxSpeed * (road1.Width * 0.05);
            double roadLeft = Canvas.GetLeft(road1);
            double roadRight = roadLeft + road1.Width - car.Width;

            if ((e.Key == Key.Left || e.Key == Key.A) && carLeft > roadLeft)
            {
                Canvas.SetLeft(car, carLeft - carMoveSpeed);
            }
            else if ((e.Key == Key.Right || e.Key == Key.D) && carLeft < roadRight)
            {
                Canvas.SetLeft(car, carLeft + carMoveSpeed);
            }
            else if (e.Key == Key.Up || e.Key == Key.W)
            {
                carSpeed = Math.Min(carSpeed + acceleration, maxSpeed);
            }
            else if (e.Key == Key.Down || e.Key == Key.S)
            {
                carSpeed = Math.Max(carSpeed - deceleration, 0);
            }

            speedText.Text = $"Speed: {Math.Round(carSpeed)} km/h";
        }

        private void RoadTimer_Tick(object sender, EventArgs e)
        {
            MoveRoad(road1);
            MoveRoad(road2);
        }

        private void MoveRoad(Image road)
        {
            double roadSpeed = carSpeed / maxSpeed * 10;
            double newTop = Canvas.GetTop(road) + roadSpeed;

            if (newTop >= MyCanvas.ActualHeight)
            {
                newTop = -MyCanvas.ActualHeight;
            }

            Canvas.SetTop(road, newTop);
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (!gameStarted)
            {
                gameStarted = true;
                if (roadTimer == null)
                {
                    roadTimer = new DispatcherTimer();
                    roadTimer.Interval = TimeSpan.FromMilliseconds(20);
                    roadTimer.Tick += RoadTimer_Tick;
                }
                roadTimer.Start();
            }
        }

        private void StopGame(object sender, RoutedEventArgs e)
        {
            gameStarted = false;
            roadTimer?.Stop();
            carSpeed = 0;
            speedText.Text = "Speed: 0 km/h";
        }
    }
}