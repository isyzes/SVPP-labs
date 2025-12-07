using System.Windows;
using System.Windows.Media;
 
namespace Lab5
{
    /// <summary>
    /// Логика взаимодействия для WindowShape.xaml
    /// </summary>
    public partial class WindowShape : Window
    {
        Shape shape;
        public WindowShape()
        {
            InitializeComponent();
            shape = new Shape(1, Colors.White, Colors.Black, 100, 100);
            grid.DataContext = shape;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        public Shape getShape() { return shape; }
    }
}
