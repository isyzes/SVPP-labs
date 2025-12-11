using Lab_8.Models;
using System.Configuration;
using System.Data.Entity;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;


namespace Lab8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServicesDbContext context;
        private List<Service> allServices;
        private List<string> categories;

        public MainWindow()
        {
            InitializeComponent();
            context = new ServicesDbContext();
            context.Services.Load();
            ServicesDataGrid.ItemsSource = context.Services.Local.ToBindingList();
            LoadData();
            LoadCategories(); 
        }

        private void LoadCategories()
        {
            try
            {
                categories = context.Services
                    .Select(s => s.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                categories.Insert(0, "Все категории");
                CategoryFilterComboBox.ItemsSource = categories;
                CategoryFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedServices = ServicesDataGrid.SelectedItems.Cast<Service>().ToList();

            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну услугу для удаления.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Создаем сообщение с перечислением выбранных услуг
            string serviceNames = string.Join("\n", selectedServices.Select(s => $"• {s.Name}"));
            string message;

            if (selectedServices.Count == 1)
            {
                message = $"Вы уверены, что хотите удалить услугу:\n\n{serviceNames}?";
            }
            else
            {
                message = $"Вы уверены, что хотите удалить {selectedServices.Count} услуг:\n\n{serviceNames}?";
            }

            var result = MessageBox.Show(message,
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем все выбранные услуги
                    context.Services.RemoveRange(selectedServices);
                    context.SaveChanges();

                    LoadData();

                    MessageBox.Show($"Успешно удалено {selectedServices.Count} услуг!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении услуг: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadData()
        {
            try
            {
                allServices = context.Services.OrderBy(s => s.Id).ToList();
                ServicesDataGrid.ItemsSource = allServices;
                UpdateTitle();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTitle()
        {
            if (allServices != null)
            {
                var activeCount = allServices.Count(s => s.IsActive);
                var totalPrice = allServices.Sum(s => s.Price);

                this.Title = $"Бытовое обслуживание населения | " +
                            $"Услуг: {allServices.Count} | Активных: {activeCount} | " +
                            $"Общая стоимость: {totalPrice:N2} руб.";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ServiceEditDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    context.Services.Add(dialog.Service);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Услуга успешно добавлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении услуги: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                var filteredServices = allServices.AsEnumerable();
                var searchText = SearchTextBox.Text.ToLower();

                // Применяем текстовый поиск
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    filteredServices = filteredServices.Where(s =>
                        s.Name.ToLower().Contains(searchText) ||
                        s.Category.ToLower().Contains(searchText) ||
                        s.Provider.ToLower().Contains(searchText) ||
                        s.Description.ToLower().Contains(searchText));
                }

                // Применяем фильтр по категории
                var selectedCategory = CategoryFilterComboBox.SelectedItem as string;
                if (selectedCategory != null && selectedCategory != "Все категории")
                {
                    filteredServices = filteredServices.Where(s => s.Category == selectedCategory);
                }

                ServicesDataGrid.ItemsSource = filteredServices.ToList();
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
            ServicesDataGrid.ItemsSource = allServices;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedServices = ServicesDataGrid.SelectedItems.Cast<Service>().ToList();

            // Проверяем количество выбранных записей
            if (selectedServices.Count == 0)
            {
                MessageBox.Show("Выберите услугу для редактирования.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selectedServices.Count > 1)
            {
                MessageBox.Show("Пожалуйста, выберите только одну услугу для редактирования.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Теперь мы точно знаем, что выбрана только одна услуга
            Service selectedService = selectedServices[0];

            var dialog = new ServiceEditDialog(selectedService);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    context.Entry(selectedService).CurrentValues.SetValues(dialog.Service);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Услуга успешно обновлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении услуги: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            context.Dispose();
        }
    }
}