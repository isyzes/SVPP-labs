using System.Windows;

namespace Lab7_2
{
    /// <summary>
    /// Логика взаимодействия для StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow(List<Service> services)
        {
            InitializeComponent();
            LoadStatistics(services);
        }

        private void LoadStatistics(List<Service> services)
        {
            var stats = new List<KeyValuePair<string, string>>();

            // Основная статистика
            stats.Add(new KeyValuePair<string, string>("Общее количество услуг", services.Count.ToString()));
            stats.Add(new KeyValuePair<string, string>("Активных услуг", services.Count(s => s.IsActive).ToString()));
            stats.Add(new KeyValuePair<string, string>("Неактивных услуг", services.Count(s => !s.IsActive).ToString()));
            stats.Add(new KeyValuePair<string, string>("Общая стоимость всех услуг", services.Sum(s => s.Price).ToString("N2") + " руб."));
            stats.Add(new KeyValuePair<string, string>("Средняя стоимость услуги", services.Average(s => s.Price).ToString("N2") + " руб."));
            stats.Add(new KeyValuePair<string, string>("Минимальная цена", services.Min(s => s.Price).ToString("N2") + " руб."));
            stats.Add(new KeyValuePair<string, string>("Максимальная цена", services.Max(s => s.Price).ToString("N2") + " руб."));

            // Статистика по категориям
            var categories = services.GroupBy(s => s.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count(),
                    AvgPrice = g.Average(s => s.Price)
                })
                .OrderByDescending(x => x.Count);

            stats.Add(new KeyValuePair<string, string>("", "")); // Пустая строка как разделитель
            stats.Add(new KeyValuePair<string, string>("Статистика по категориям:", ""));

            foreach (var category in categories)
            {
                stats.Add(new KeyValuePair<string, string>(
                    $"  • {category.Category}",
                    $"{category.Count} услуг, средняя цена: {category.AvgPrice:N2} руб."));
            }

            // Статистика по активности
            var byMonth = services.GroupBy(s => s.RegistrationDate.ToString("yyyy-MM"))
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month);

            stats.Add(new KeyValuePair<string, string>("", ""));
            stats.Add(new KeyValuePair<string, string>("Регистрация по месяцам:", ""));

            foreach (var month in byMonth)
            {
                stats.Add(new KeyValuePair<string, string>(
                    $"  • {month.Month}",
                    $"{month.Count} услуг"));
            }

            StatsListView.ItemsSource = stats;
        }
    }
}
