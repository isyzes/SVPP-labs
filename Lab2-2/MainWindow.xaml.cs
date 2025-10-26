using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuctionLot carrentLot;

        public MainWindow()
        {
            this.carrentLot = new AuctionLot();
            this.carrentLot.ImagePath = "/Images/placeholder.png";

            this.carrentLot.Title = "Title";
            this.carrentLot.Category = LotCategory.Antique;
            this.carrentLot.IsVerified = true;
            this.carrentLot.IsUrgent = true;
            this.carrentLot.Status = LotStatus.Draft;
            this.carrentLot.StartDate = new DateTime(2025, 09, 01);
            this.carrentLot.EndDate = new DateTime(2025, 09, 14);
            this.carrentLot.StartPrice = 1000;

            InitializeComponent();

            sldStartPrice.ValueChanged += (s, e) => tbPriceValue.Text = sldStartPrice.Value.ToString("C0");
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Генерируем уникальное имя файла
                    string originalFileName = Path.GetFileNameWithoutExtension(dialog.FileName);
                    string extension = Path.GetExtension(dialog.FileName);
                    string uniqueFileName = $"{originalFileName}_{Guid.NewGuid():N}{extension}";
                    string destinationPath = Path.Combine("Images", uniqueFileName);

                    // Копируем файл в папку Images
                    File.Copy(dialog.FileName, destinationPath, true);

                    // Загружаем изображение в интерфейс
                    lotImage.Source = new BitmapImage(new Uri(destinationPath, UriKind.RelativeOrAbsolute));
                    //_currentImagePath = destinationPath;

                    MessageBox.Show($"Изображение сохранено как: {uniqueFileName}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                }

                lotImage.Source = new BitmapImage(new System.Uri(dialog.FileName));
            }
        }

        private void Category_Checked(object sender, RoutedEventArgs e)
        {
            // Логика выбора категории
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            AuctionLot lot = new AuctionLot
            {
                Title = txtTitle.Text,
                //Category = rbArt.IsChecked == true ? "Искусство" :
                //          rbJewelry.IsChecked == true ? "Ювелирные изделия" : "Антиквариат",
                IsVerified = chkVerified.IsChecked == true,
                IsUrgent = chkUrgent.IsChecked == true,
                //Status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString(),
                StartDate = dpStartDate.SelectedDate,
                EndDate = dpEndDate.SelectedDate,
                StartPrice = sldStartPrice.Value
            };

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                DefaultExt = "json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(lot, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveDialog.FileName, json);
                MessageBox.Show("Данные сохранены!");
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json"
            };

            if (openDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openDialog.FileName);
                AuctionLot lot = JsonSerializer.Deserialize<AuctionLot>(json);

                // Заполнение полей формы
                txtTitle.Text = lot.Title;
                //switch (lot.Category)
                //{
                //    case "Искусство": rbArt.IsChecked = true; break;
                //    case "Ювелирные изделия": rbJewelry.IsChecked = true; break;
                //    case "Антиквариат": rbAntique.IsChecked = true; break;
                //}
                chkVerified.IsChecked = lot.IsVerified;
                chkUrgent.IsChecked = lot.IsUrgent;
                //cmbStatus.Text = lot.Status;
                dpStartDate.SelectedDate = lot.StartDate;
                dpEndDate.SelectedDate = lot.EndDate;
                sldStartPrice.Value = lot.StartPrice;
            }
        }
    }

    
}