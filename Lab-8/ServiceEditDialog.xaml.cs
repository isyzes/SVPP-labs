using Lab_8.Models;
using System.Globalization;
using System.Windows;

namespace Lab8
{
    /// <summary>
    /// Логика взаимодействия для ServiceEditDialog.xaml
    /// </summary>
    public partial class ServiceEditDialog : Window
    {
        public Service Service { get; private set; }
        private bool _isEditMode;

        public ServiceEditDialog()
        {
            InitializeComponent();
            Service = new Service
            {
                RegistrationDate = DateTime.Now,
                IsActive = true
            };
            _isEditMode = false;
            DataContext = Service;
        }

        public ServiceEditDialog(Service service)
        {
            InitializeComponent();
            Service = new Service();
            CopyService(service, Service);
            _isEditMode = true;
            DataContext = Service;
            Title = "Редактирование услуги";
        }

        private void CopyService(Service source, Service target)
        {
            target.Id = source.Id;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Category = source.Category;
            target.Price = source.Price;
            target.Provider = source.Provider;
            target.Address = source.Address;
            target.Phone = source.Phone;
            target.RegistrationDate = source.RegistrationDate;
            target.IsActive = source.IsActive;
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
            if (!decimal.TryParse(PriceTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                PriceTextBox.Focus();
                return false;
            }
            Service.Price = price;

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
