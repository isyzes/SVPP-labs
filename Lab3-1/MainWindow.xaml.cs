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

namespace Lab3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Driver driver = new Driver();
        public MainWindow()
        {
            InitializeComponent();
            foreach (COLOREYES color in Enum.GetValues(typeof(COLOREYES)))
                comboBoxEyes.Items.Add(color);
            NewDriver();
            grid.DataContext = driver;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            driver.Name = textBoxName.Text;
            driver.Adress = textBoxAdress.Text;
            if (textBoxClass1.Text.Length == 1)
            {
                driver.Class1 = textBoxClass1.Text[0];
            }
            else
            {
                driver.Class1 = null;
            }
            if (textBoxNumber.Text.Length > 0)
            {
                int value;
                int.TryParse(textBoxNumber.Text, out value);
                driver.Number = value;
            }
            if (int.TryParse(textBoxNumber.Text, out int numberValue))
            {
                driver.Number = numberValue;
            }
            else
            {
                driver.Number = null;
            }
            driver.Hgt = sliderHGT.Value;
            driver.Donor = checkBoxDonor.IsChecked;
            driver.Dob = datePickerDOB.SelectedDate;
            driver.Iss = datePickerISS.SelectedDate;
            driver.Exp = datePickerEXP.SelectedDate;
            driver.Gender = GENDER.other;
            if (radioButtonM.IsChecked == true)
            {
                driver.Gender = GENDER.male;
            }
            if (radioButtonF.IsChecked == true)
            {
                driver.Gender = GENDER.female;
            }
            if (comboBoxEyes.SelectedIndex > -1)
            {
                driver.Coloreyes = (COLOREYES)comboBoxEyes.SelectedValue;
            }
            else
            {
                driver.Coloreyes = null;
            }

            MessageBox.Show(driver.ToString());
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            //openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == false) return;
            driver.UriImage = openFileDialog.FileName;
            image.Source = new BitmapImage(new Uri(driver.UriImage, UriKind.RelativeOrAbsolute));
        }

        void NewDriver()
        {
            Random random = new Random();
            string[] names = { "Иванов И.И.", "Петров П.П.", "Сидоров С.С.", "Александров А.А." };
            string[] adress = { "г. Минск", "г. Брест", "г. Пинск", "г. Слуцк" };
            driver.Name = names[random.Next(names.Length)];
            driver.Adress = adress[random.Next(adress.Length)];
            driver.Class1 = (char)((int)'A' + random.Next(5));
            driver.Number = random.Next(100000, 99999999);
            driver.Hgt = random.Next(1000, 2500) / 10.0;
            driver.Dob = DateTime.Now.AddDays(-random.Next(30000));
            driver.Iss = DateTime.Now.AddDays(-random.Next(1000));
            driver.Exp = driver.Iss?.AddYears(10);
            driver.Gender = (GENDER)random.Next(3);
            driver.Coloreyes = (COLOREYES)random.Next(Enum.GetValues(typeof(COLOREYES)).Length);
            driver.Donor = (bool)(random.Next(2) == 0);
            driver.UriImage = $"Images/{random.Next(1, 4)}.jpg";
            if (!string.IsNullOrEmpty(driver.UriImage))
            {
                image.Source = new BitmapImage(new Uri(driver.UriImage, UriKind.RelativeOrAbsolute));
            }

        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            NewDriver();

        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxName.Text = "";
            textBoxNumber.Text = "";
            textBoxAdress.Text = "";
            textBoxClass1.Text = "";
            datePickerDOB.SelectedDate = null;
            datePickerISS.SelectedDate = null;
            datePickerEXP.SelectedDate = null;
            radioButtonM.IsChecked = false;
            radioButtonF.IsChecked = false;
            radioButtonO.IsChecked = false;
            comboBoxEyes.SelectedIndex = -1;
            checkBoxDonor.IsChecked = false;
            sliderHGT.Value = sliderHGT.Minimum;
            image.Source = new BitmapImage(new Uri("/Images/placeholder.png", UriKind.Relative));
            driver = new Driver();
            grid.DataContext = driver;

            MessageBox.Show("Все данные очищены!");
        }

    }
}