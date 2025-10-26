using Microsoft.Win32;
using System.Drawing;
using System.Net;
using System.Reflection;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab2_1
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

            datePickerDOB.SelectedDate = DateTime.Now;
            datePickerISS.SelectedDate = DateTime.Now;
            datePickerEXP.SelectedDate = DateTime.Now;

            foreach (COLOREYES color in Enum.GetValues(typeof(COLOREYES))) { 
                comboBoxEyes.Items.Add(color);
            }

            comboBoxEyes.SelectedIndex = 0;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            driver.Name = textBoxName.Text;
            driver.Number = Int32.Parse(textBoxNumber.Text);
            driver.Address= textBoxAddress.Text;
            driver.Clazz = textBoxClass.Text.Length > 0 ? textBoxClass.Text[0] : '0';
            driver.Dob = (DateTime)datePickerDOB.SelectedDate;
            driver.Iss = (DateTime)datePickerISS.SelectedDate;
            driver.Exp = (DateTime)datePickerEXP.SelectedDate;

            if (radioButtonMale.IsChecked == true) {
                driver.Gender = GENDER.male;
            } else if (radioButtonFemale.IsChecked == true)
            {
                driver.Gender = GENDER.female;
            } else if(radioButtonVariant.IsChecked == true)
            {
                driver.Gender = GENDER.variant;
            }

            driver.Donor = checkBoxDonor.IsChecked == true;
            driver.Hgt = sliderHgt.Value;

            driver.Eyes = (COLOREYES)comboBoxEyes.SelectedValue;

            MessageBox.Show(driver.ToString());
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Фвйлы (jpg)|*.jpg|Все файлы|*.*.";
            if (dialog.ShowDialog() == false) return;

            driver.UriImage = dialog.FileName;
            image.Source = new BitmapImage(new Uri(dialog.FileName, UriKind.RelativeOrAbsolute));

        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            driver.Number = 123456789;
            driver.Clazz = 'B';
            driver.Hgt = 175;
            driver.Name = "Иванов Иван Иванович";
            driver.Address = "г. Москва, ул. Пушкина, д. 10";
            driver.Gender = GENDER.male;
            driver.Eyes = COLOREYES.blue;
            driver.Dob = new DateTime(1990, 5, 15);
            driver.Iss = new DateTime(2020, 8, 21);
            driver.Exp = new DateTime(2030, 8, 21);
            driver.Donor = true;
            driver.UriImage = "/Images/2.jpg";

            textBoxName.Text = driver.Name;
            textBoxClass.Text = driver.Clazz.ToString();
            textBoxAddress.Text = driver.Address;
            textBoxNumber.Text = driver.Number.ToString();
            sliderHgt.Value = driver.Hgt;
            switch (driver.Gender)
            {
                case GENDER.male: radioButtonMale.IsChecked = true; break;
                case GENDER.female: radioButtonFemale.IsChecked = true; break;
                case GENDER.variant : radioButtonVariant.IsChecked = true; break;
            }

            comboBoxEyes.SelectedItem = driver.Eyes;
            datePickerDOB.SelectedDate = driver.Dob;
            datePickerISS.SelectedDate = driver.Iss;
            datePickerEXP.SelectedDate = driver.Exp;
            checkBoxDonor.IsChecked = driver.Donor;

            image.Source = new BitmapImage(new Uri(driver.UriImage, UriKind.RelativeOrAbsolute));
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            driver = new Driver();

            textBoxName.Clear();
            textBoxClass.Clear();
            textBoxAddress.Clear();
            textBoxNumber.Clear();
            sliderHgt.Value = sliderHgt.Minimum;
            radioButtonMale.IsChecked = false;
            radioButtonFemale.IsChecked = false;
            radioButtonVariant.IsChecked = false;
            comboBoxEyes.SelectedIndex = 0;
            datePickerDOB.SelectedDate = DateTime.Now;
            datePickerISS.SelectedDate = DateTime.Now;
            datePickerEXP.SelectedDate = DateTime.Now;
            checkBoxDonor.IsChecked = false;
            image.Source = new BitmapImage(new Uri("/Images/placeholder.png", UriKind.RelativeOrAbsolute));
        }
    }
}