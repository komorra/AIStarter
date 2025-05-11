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
using AIStarter.UI;
using Path = System.IO.Path;

namespace AIStarter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Models_Click(object sender, RoutedEventArgs e)
        {
            var selectModelWindow = new WindowSelectModel();
            selectModelWindow.Owner = this; // Set the owner of the new window to the main window          
            var result = selectModelWindow.ShowDialog();

            if (result == true)
            {
                var modelFileName = selectModelWindow.SelectedModelFileName;

                var controlModelCell = new ControlModelCell();
                controlModelCell.ModelName = Path.GetFileNameWithoutExtension(modelFileName) ?? string.Empty;
                controlModelCell.ModelFile = modelFileName ?? string.Empty;
                controlModelCell.DataContext = controlModelCell;
                controlModelCell.HorizontalAlignment = HorizontalAlignment.Stretch;

                ModelPanel.Children.Add(controlModelCell);

            }            
        }
    }
}