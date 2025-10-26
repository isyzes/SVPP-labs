using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace Lab2_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            foreach (LotStatus status in Enum.GetValues(typeof(LotStatus)))
            {
                cmbStatus.Items.Add(status);
            }

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


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            AuctionLot lot = new AuctionLot
            {
                Title = txtTitle.Text,
                Category = rbArt.IsChecked == true ? LotCategory.Art :
                          rbJewelry.IsChecked == true ? LotCategory.Jewelry : LotCategory.Antique,
                IsVerified = chkVerified.IsChecked == true,
                IsUrgent = chkUrgent.IsChecked == true,
                Status = (LotStatus)cmbStatus.SelectedValue,
                StartDate = dpStartDate.SelectedDate,
                EndDate = dpEndDate.SelectedDate,
                ImagePath = lotImage.Source.ToString(),
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
                switch (lot.Category)
                {
                    case LotCategory.Art: rbArt.IsChecked = true; break;
                    case LotCategory.Jewelry: rbJewelry.IsChecked = true; break;
                    case LotCategory.Antique: rbAntique.IsChecked = true; break;
                }
                chkVerified.IsChecked = lot.IsVerified;
                chkUrgent.IsChecked = lot.IsUrgent;
                cmbStatus.SelectedItem = lot.Status;
                dpStartDate.SelectedDate = lot.StartDate;
                dpEndDate.SelectedDate = lot.EndDate;
                sldStartPrice.Value = lot.StartPrice;
                lotImage.Source = new BitmapImage(new Uri(lot.ImagePath, UriKind.RelativeOrAbsolute));
            }
        }
    }
}