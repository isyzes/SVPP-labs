using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double a = 0;
        private double b = 1;
        private int n = 1000;

        public MainWindow()
        {
            InitializeComponent();
            UpdateParamsInfo();
        }

        // === Dispatcher реализация ===
        private void Button_Dispatcher_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            SetButtonsEnabled(false);

            Thread calculationThread = new Thread(new ThreadStart(CalculateWithDispatcher));
            calculationThread.Start();
        }

        // === BackgroundWorker реализация ===
        private void Button_BackgroundWorker_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            SetButtonsEnabled(false);

            BackgroundWorker worker = (BackgroundWorker)this.FindResource("MyBackgroundWorker");
            worker.RunWorkerAsync();
        }

        // === Асинхронный стрим ===
        private async void Button_AsyncStream_Click(object sender, RoutedEventArgs e)
        {
            ClearResults();
            SetButtonsEnabled(false);

            try
            {
                var calculator = new IntegralCalculator(a, b, n);
                IAsyncEnumerable<(double x, double S, double progress)> data = calculator.CalculateAsync();

                double finalResult = 0;

                await foreach (var (x, currentIntegral, progress) in data)
                {
                    ProgressBar.Value = progress * 100;

                    if ((int)(progress * n) % 10 == 0)
                    {
                        ResultsListBox.Items.Add($"Async: x={x:F2}, S={currentIntegral:F6}, progress={progress:P0}");
                        ResultsListBox.ScrollIntoView(ResultsListBox.Items[ResultsListBox.Items.Count - 1]);
                    }

                    finalResult = currentIntegral;
                }

                ResultTextBlock.Text = $"Результат (Async Stream): {finalResult:F6}";
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
            }
            finally
            {
                SetButtonsEnabled(true);
            }
        }

        private void ButtonParams_Click(object sender, RoutedEventArgs e)
        {
            var paramsWindow = new ParametersWindow(a, b, n);
            if (paramsWindow.ShowDialog() == true)
            {
                a = paramsWindow.A;
                b = paramsWindow.B;
                n = paramsWindow.N;
                UpdateParamsInfo();
            }
        }

        private void UpdateParamsInfo()
        {
            ParamsInfoText.Text = $"a={a}, b={b}, N={n}";
        }


        private void ClearResults()
        {
            ResultsListBox.Items.Clear();
            ProgressBar.Value = 0;
            ResultTextBlock.Text = "Вычисление...";
        }

        private void SetButtonsEnabled(bool enabled)
        {
            Button_Dispatcher.IsEnabled = enabled;
            Button_BackgroundWorker.IsEnabled = enabled;
            Button_AsyncStream.IsEnabled = enabled;
            ButtonParams.IsEnabled = enabled;
        }

        private void CalculateWithDispatcher()
        {
            double h = (b - a) / n;
            double sum = 0.0;

            for (int i = 0; i <= n; i++)
            {
                double x = a + h * i;
                if (i < n) sum += Function(x);

                double currentIntegral = sum * h;
                double progress = (double)i / n;

                // Обновляем UI через Dispatcher
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProgressBar.Value = progress * 100;
                    if (i % 10 == 0) // Чтобы не засорять ListBox, обновляем каждые 10 итераций
                    {
                        ResultsListBox.Items.Add($"Dispatcher: x={x:F2}, S={currentIntegral:F6}, progress={progress:P0}");
                        ResultsListBox.ScrollIntoView(ResultsListBox.Items[ResultsListBox.Items.Count - 1]);
                    }
                }), DispatcherPriority.Normal);
            }

            double result = sum * h;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ResultTextBlock.Text = $"Результат (Dispatcher): {result:F6}";
                SetButtonsEnabled(true);
            }), DispatcherPriority.Normal);
        }


        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            double h = (b - a) / n;
            double sum = 0.0;

            for (int i = 0; i <= n; i++)
            {
                double x = a + h * i;
                if (i < n) sum += Function(x);

                double currentIntegral = sum * h;
                double progress = (double)i / n;

                // Отправляем прогресс и промежуточные данные
                worker.ReportProgress((int)(progress * 100), new { x, currentIntegral, progress });
            }
            e.Result = sum * h;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;

            // Получаем промежуточные данные
            var data = e.UserState as dynamic;
            if (data != null && (int)(data.progress * n) % 10 == 0)
            {
                ResultsListBox.Items.Add($"BgWorker: x={data.x:F2}, S={data.currentIntegral:F6}, progress={data.progress:P0}");
                ResultsListBox.ScrollIntoView(ResultsListBox.Items[ResultsListBox.Items.Count - 1]);
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ResultTextBlock.Text = $"Ошибка: {e.Error.Message}";
            }
            else
            {
                double result = (double)e.Result;
                ResultTextBlock.Text = $"Результат (BgWorker): {result:F6}";
            }
            SetButtonsEnabled(true);
        }

        private double Function(double x)
        {
            return x*x*x;
        }
    }
}