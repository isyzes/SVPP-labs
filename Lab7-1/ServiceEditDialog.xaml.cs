using System.Globalization;
using System.Windows;

namespace Lab7_1
{
    /// <summary>
    /// Логика взаимодействия для ServiceEditDialog.xaml
    /// </summary>
    public partial class ServiceEditDialog : Window
    {
        public Service Service { get; private set; }

        public ServiceEditDialog()
        {
            InitializeComponent();
            Service = new Service
            {
                RegistrationDate = DateTime.Now,
                IsActive = true
            };
            DataContext = Service;
        }

        public ServiceEditDialog(Service service)
        {
            InitializeComponent();
            Service = new Service
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Category = service.Category,
                Price = service.Price,
                Provider = service.Provider,
                Address = service.Address,
                Phone = service.Phone,
                RegistrationDate = service.RegistrationDate,
                IsActive = service.IsActive
            };
            DataContext = Service;
            Title = "Редактирование услуги";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            DialogResult = true;
            Close();
        }

        private bool ValidateInput()
        {
            // Проверка названия
            if (string.IsNullOrWhiteSpace(Service.Name))
            {
                MessageBox.Show("Введите название услуги", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return false;
            }

            // Проверка категории
            if (string.IsNullOrWhiteSpace(Service.Category))
            {
                MessageBox.Show("Введите категорию услуги", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                CategoryTextBox.Focus();
                return false;
            }

            // Проверка цены
            if (!decimal.TryParse(Service.Price.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PriceTextBox.Focus();
                return false;
            }
            Service.Price = price;

            // Проверка поставщика
            if (string.IsNullOrWhiteSpace(Service.Provider))
            {
                MessageBox.Show("Введите поставщика услуги", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ProviderTextBox.Focus();
                return false;
            }

            // Проверка телефона
            if (string.IsNullOrWhiteSpace(Service.Phone))
            {
                MessageBox.Show("Введите телефон поставщика", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PhoneTextBox.Focus();
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
