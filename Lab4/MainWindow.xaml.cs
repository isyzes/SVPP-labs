using System.Windows;

namespace Lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isGameRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameRunning)
            {
                GameControl.StartGame();
                StartPauseButton.Content = "Пауза";
                isGameRunning = true;
            }
            else
            {
                GameControl.PauseGame();
                StartPauseButton.Content = "Старт";
                isGameRunning = false;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            GameControl.ResetGame();
            StartPauseButton.Content = "Старт";
            isGameRunning = false;
            InfoText.Text = "Игра сброшена. Нажмите Старт для начала игры.";
        }

        private void DropBombButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGameRunning)
            {
                GameControl.DropBomb();
            }
        }

        private void GameControl_BombInfoRequested(string info)
        {
            InfoText.Text = info;
        }

        private void GameControl_AirplaneInfoRequested(string info)
        {
            InfoText.Text = info;
        }
    }
}