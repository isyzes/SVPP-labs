using System;
using System.Windows;
using System.Windows.Controls;

namespace Lab1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            FromUnit.SelectedIndex = 0;
            ToUnit.SelectedIndex = 1;

            ConvertValue(); 
        }

        private void ConvertValue()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(InputValue.Text))
                {
                    ResultText.Text = "0";
                    return;
                }
                

                double inputValue = double.Parse(InputValue.Text);

                string fromUnit = ((ComboBoxItem)FromUnit.SelectedItem).Content.ToString();
                string toUnit = ((ComboBoxItem)ToUnit.SelectedItem).Content.ToString();

                double valueInGrams = ConvertToGrams(inputValue, fromUnit);
                double result = ConvertFromGrams(valueInGrams, toUnit);

                ResultText.Text = result.ToString();
            }
            catch (FormatException)
            {
                ResultText.Text = "Ошибка ввода";
            }
            catch (Exception ex)
            {
                ResultText.Text = "Ошибка";
            }
        }

        private double ConvertToGrams(double value, string fromUnit)
        {
            switch (fromUnit.ToLower())
            {
                case "миллиграмм": return value / 1000;
                case "грамм": return value;
                case "килограмм": return value * 1000;
                case "центнер": return value * 100000;
                case "тонн": return value * 1000000;
                default: return value;
            }
        }

        private double ConvertFromGrams(double value, string toUnit)
        {
            switch (toUnit.ToLower())
            {
                case "миллиграмм": return value * 1000;
                case "грамм": return value;
                case "килограмм": return value / 1000;
                case "центнер": return value / 100000;
                case "тонн": return value / 1000000;
                default: return value;
            }
        }

        private void InputValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConvertValue();
        }

        private void FromUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConvertValue();
        }
    }
}