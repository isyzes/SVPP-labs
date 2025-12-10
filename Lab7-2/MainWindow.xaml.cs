using System.Configuration;
using System.Windows;
using System.Windows.Controls;


namespace Lab7_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private List<Service> _allServices;
        private List<string> _categories;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            _dbHelper = new DatabaseHelper(connectionString);
            LoadData();
            LoadCategories();
        }

        private void LoadData()
        {
            try
            {
                _allServices = _dbHelper.GetAllServices();
                ServicesDataGrid.ItemsSource = _allServices;

                // Обновляем статистику
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _dbHelper.GetAllCategories();
                _categories.Insert(0, "Все категории");
                CategoryFilterComboBox.ItemsSource = _categories;
                CategoryFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatistics()
        {
            var activeCount = _allServices.Count(s => s.IsActive);
            var totalPrice = _allServices.Sum(s => s.Price);
            var avgPrice = _allServices.Average(s => s.Price);

            this.Title = $"Бытовое обслуживание населения | Услуг: {_allServices.Count} | Активных: {activeCount}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            try
            {
                var searchText = SearchTextBox.Text;
                var selectedCategory = CategoryFilterComboBox.SelectedItem as string;

                if (string.IsNullOrWhiteSpace(searchText) &&
                    (selectedCategory == "Все категории" || string.IsNullOrEmpty(selectedCategory)))
                {
                    ServicesDataGrid.ItemsSource = _allServices;
                }
                else
                {
                    var filteredServices = _dbHelper.SearchServices(searchText, selectedCategory);
                    ServicesDataGrid.ItemsSource = filteredServices;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = string.Empty;
            CategoryFilterComboBox.SelectedIndex = 0;
            ServicesDataGrid.ItemsSource = _allServices;
        }

        private void ServicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = ServicesDataGrid.SelectedItem != null;
            DeleteButton.IsEnabled = ServicesDataGrid.SelectedItem != null;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ServiceEditDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    if (_dbHelper.AddService(dialog.Service))
                    {
                        LoadData();
                        MessageBox.Show("Услуга успешно добавлена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить услугу", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении услуги: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItem is Service selectedService)
            {
                var dialog = new ServiceEditDialog(selectedService);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        if (_dbHelper.UpdateService(dialog.Service))
                        {
                            LoadData();
                            MessageBox.Show("Услуга успешно обновлена!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить услугу", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении услуги: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItem is Service selectedService)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить услугу '{selectedService.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (_dbHelper.DeleteService(selectedService.Id))
                        {
                            LoadData();
                            MessageBox.Show("Услуга успешно удалена!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить услугу", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении услуги: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var services = _dbHelper.GetAllServices();

                var stats = new StatsWindow(services);
                stats.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ServicesDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Форматирование заголовков
            if (e.PropertyName == "Id")
                e.Column.Header = "ID";
            else if (e.PropertyName == "Name")
                e.Column.Header = "Название";
            else if (e.PropertyName == "Category")
                e.Column.Header = "Категория";
            else if (e.PropertyName == "Price")
            {
                e.Column.Header = "Цена (руб.)";
                if (e.Column is DataGridTextColumn textColumn)
                {
                    textColumn.Binding.StringFormat = "N2";
                }
            }
            else if (e.PropertyName == "Provider")
                e.Column.Header = "Поставщик";
            else if (e.PropertyName == "Phone")
                e.Column.Header = "Телефон";
            else if (e.PropertyName == "IsActive")
                e.Column.Header = "Активна";
            else
                e.Cancel = true; // Скрываем остальные столбцы
        }
    }

}