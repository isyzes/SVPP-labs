using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lab4
{
    public partial class BombingGameControl : UserControl
    {
        private Ellipse airplane;
        private List<Ellipse> bombs = new List<Ellipse>();
        private List<BombData> bombData = new List<BombData>();
        private System.Windows.Threading.DispatcherTimer gameTimer;
        private Random random = new Random();
        private const double AIRPLANE_SPEED = 2;
        private const double GRAVITY = 0.5;
        private bool isExploding = false;
        private int explosionCounter = 0;

        public event Action<string> AirplaneInfoRequested;
        public event Action<string> BombInfoRequested;

        public BombingGameControl()
        {
            InitializeComponent();
            InitializeGame();

            this.Loaded += BombingGameControl_Loaded;
        }

        private void BombingGameControl_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(airplane, 50);
            Canvas.SetTop(airplane, 50);
            // Устанавливаем случайную позицию цели при загрузке
            PlaceTargetRandomly();
        }

        private void InitializeGame()
        {
            airplane = new Ellipse
            {
                Width = 40,
                Height = 20,
                Fill = Brushes.Gray,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            GameCanvas.Children.Add(airplane);

            gameTimer = new System.Windows.Threading.DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
        }

        private void PlaceTargetRandomly()
        {
            if (GameCanvas.ActualWidth == 0 || GameCanvas.ActualHeight == 0)
                return;

            // Минимальные и максимальные координаты для цели
            double minX = 50; // Отступ от левого края
            double maxX = GameCanvas.ActualWidth - Target.Width - 50; // Отступ от правого края
            double minY = GameCanvas.ActualHeight - 100; // Выше земли
            double maxY = GameCanvas.ActualHeight - 40; // Уровень земли

            // Генерация случайных координат
            double randomX = random.Next((int)minX, (int)maxX);
            double randomY = random.Next((int)minY, (int)maxY);

            // Установка позиции цели
            Canvas.SetLeft(Target, randomX);
            Canvas.SetTop(Target, randomY);

            Target.Width = random.Next(20, 50); // Ширина от 20 до 50 пикселей
            Target.Height = random.Next(8, 15); // Высота от 8 до 15 пикселей

            byte r = (byte)random.Next(150, 256);
            byte g = (byte)random.Next(50, 150);
            byte b = (byte)random.Next(50, 150);
            Target.Fill = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (!isExploding)
            {
                MoveAirplane();
                MoveBombs();
                CheckCollisions();
            }
            else
            {
                HandleExplosion();
            }
        }

        private void MoveAirplane()
        {
            double left = Canvas.GetLeft(airplane);
            left += AIRPLANE_SPEED;

            if (left > GameCanvas.ActualWidth)
            {
                left = -airplane.Width;
            }

            Canvas.SetLeft(airplane, left);
        }

        public void DropBomb()
        {
            Ellipse bomb = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,
                Stroke = Brushes.DarkGray,
                StrokeThickness = 1
            };

            double airplaneLeft = Canvas.GetLeft(airplane);
            double airplaneTop = Canvas.GetTop(airplane);

            Canvas.SetLeft(bomb, airplaneLeft + airplane.Width / 2 - bomb.Width / 2);
            Canvas.SetTop(bomb, airplaneTop + airplane.Height);

            GameCanvas.Children.Add(bomb);
            bombs.Add(bomb);
            bombData.Add(new BombData
            {
                VelocityX = AIRPLANE_SPEED,
                VelocityY = 0
            });
        }

        private void MoveBombs()
        {
            for (int i = bombs.Count - 1; i >= 0; i--)
            {
                var bomb = bombs[i];
                var data = bombData[i];

                data.VelocityY += GRAVITY;

                double left = Canvas.GetLeft(bomb);
                double top = Canvas.GetTop(bomb);

                Canvas.SetLeft(bomb, left + data.VelocityX);
                Canvas.SetTop(bomb, top + data.VelocityY);

                if (top > GameCanvas.ActualHeight)
                {
                    GameCanvas.Children.Remove(bomb);
                    bombs.RemoveAt(i);
                    bombData.RemoveAt(i);
                }
            }
        }

        private void CheckCollisions()
        {
            Rect targetRect = new Rect(
                Canvas.GetLeft(Target),
                Canvas.GetTop(Target),
                Target.Width,
                Target.Height);

            for (int i = bombs.Count - 1; i >= 0; i--)
            {
                var bomb = bombs[i];
                Rect bombRect = new Rect(
                    Canvas.GetLeft(bomb),
                    Canvas.GetTop(bomb),
                    bomb.Width,
                    bomb.Height);

                if (targetRect.IntersectsWith(bombRect))
                {
                    StartExplosion(bombRect);
                    GameCanvas.Children.Remove(bomb);
                    bombs.RemoveAt(i);
                    bombData.RemoveAt(i);
                    Target.Visibility = Visibility.Hidden;
                    break;
                }
            }
        }

        private void StartExplosion(Rect bombRect)
        {
            isExploding = true;
            explosionCounter = 0;
            Canvas.SetLeft(Explosion, bombRect.Left - 25);
            Canvas.SetTop(Explosion, bombRect.Top - 25);
        }

        private void HandleExplosion()
        {
            explosionCounter++;

            if (explosionCounter < 30)
            {
                double size = 50 + explosionCounter * 2;
                Explosion.Width = size;
                Explosion.Height = size;
                Explosion.Opacity = 1 - (explosionCounter / 30.0);
            }
            else
            {
                isExploding = false;
                Explosion.Opacity = 0;
                ResetTarget();
            }
        }

        private void ResetTarget()
        {
            // Размещаем цель в новом случайном месте
            PlaceTargetRandomly();
            Target.Visibility = Visibility.Visible;
        }

        private void GameCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var position = e.GetPosition(GameCanvas);

            foreach (var bomb in bombs)
            {
                Rect bombRect = new Rect(
                    Canvas.GetLeft(bomb),
                    Canvas.GetTop(bomb),
                    bomb.Width,
                    bomb.Height);

                if (bombRect.Contains(position))
                {
                    int index = bombs.IndexOf(bomb);
                    var data = bombData[index];
                    double speed = Math.Sqrt(data.VelocityX * data.VelocityX + data.VelocityY * data.VelocityY);

                    BombInfoRequested?.Invoke($"Скорость бомбы: {speed:F1}");
                    return;
                }
            }

            Rect airplaneRect = new Rect(
                Canvas.GetLeft(airplane),
                Canvas.GetTop(airplane),
                airplane.Width,
                airplane.Height);

            if (airplaneRect.Contains(position))
            {
                AirplaneInfoRequested?.Invoke($"Позиция самолета: X={Canvas.GetLeft(airplane):F1}, Y={Canvas.GetTop(airplane):F1}");
            }
        }

        public void StartGame()
        {
            gameTimer.Start();
        }

        public void PauseGame()
        {
            gameTimer.Stop();
        }

        public void ResetGame()
        {
            gameTimer.Stop();

            Canvas.SetLeft(airplane, 50);
            Canvas.SetTop(airplane, 50);

            foreach (var bomb in bombs)
            {
                GameCanvas.Children.Remove(bomb);
            }
            bombs.Clear();
            bombData.Clear();

            ResetTarget();
            Explosion.Opacity = 0;
            isExploding = false;
        }

        private class BombData
        {
            public double VelocityX { get; set; }
            public double VelocityY { get; set; }
        }
    }
}