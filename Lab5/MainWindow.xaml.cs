using System.Windows;
using System.Windows.Input;

namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Shape shape;
        public MainWindow()
        {
            InitializeComponent();
            CommandBinding commandBindingHelp = new CommandBinding();
            commandBindingHelp.Command = ApplicationCommands.Help;
            commandBindingHelp.Executed += help;
            menuItemHelp.CommandBindings.Add(commandBindingHelp);
            buttonHelp.CommandBindings.Add(commandBindingHelp);

            CommandBinding commandBindingSave = new CommandBinding();
            commandBindingSave.Command = ApplicationCommands.Save;
            commandBindingSave.Executed += save;
            menuItemSave.CommandBindings.Add(commandBindingSave);
            buttonSave.CommandBindings.Add(commandBindingSave);
            commandBindingSave.CanExecute += canExecute;

            CommandBinding commandBindingOpen = new CommandBinding();
            commandBindingOpen.Command = ApplicationCommands.Open;
            commandBindingOpen.Executed += open;
            menuItemOpen.CommandBindings.Add(commandBindingOpen);
            buttonOpen.CommandBindings.Add(commandBindingOpen);

            CommandBinding commandBindingClose = new CommandBinding();
            commandBindingClose.Command = ApplicationCommands.Close;
            commandBindingClose.Executed += close;
            menuItemExit.CommandBindings.Add(commandBindingClose);
            buttonExit.CommandBindings.Add(commandBindingClose);

        }

        private void canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = shape != null;
        }

        private void open(object sender, ExecutedRoutedEventArgs e)
        {
            shape = Shape.load();
        }

        private void save(object sender, ExecutedRoutedEventArgs e)
        {
            if (shape == null) return;
            shape.save();
        }

        private void help(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("справка по приложению");
        }

        private void close(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItemShape_Click(object sender, RoutedEventArgs e)
        {
            WindowShape windowShape = new WindowShape();
            if (windowShape.ShowDialog() == false) return;
            shape = windowShape.getShape();
            //MessageBox.Show(shape.ToString());
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (shape == null) return;
            shape.draw(canvas, e.GetPosition(canvas));
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            textBlockCursorPosition.Text = $"X = {e.GetPosition(canvas).X}, Y = {e.GetPosition(canvas).Y}";
        }
    }
}