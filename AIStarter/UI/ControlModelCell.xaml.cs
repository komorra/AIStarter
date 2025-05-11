using AIStarter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AIStarter.UI
{
    /// <summary>
    /// Logika interakcji dla klasy ControlModelCell.xaml
    /// </summary>
    public partial class ControlModelCell : UserControl
    {
        public string ModelName { get; set; } = string.Empty;
        public string ModelFile { get; set; } = string.Empty;

        public string? ModelInputJSON { get; private set; }

        public ControlModelCell()
        {
            InitializeComponent();
            Loaded += ControlModelCell_Loaded;
        }

        private void ControlModelCell_Loaded(object sender, RoutedEventArgs e)
        {
            var modelData = File.ReadAllText(ModelFile);
            ModelInputJSON = ModelToUIParser.Populate(Input, modelData);
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            if(Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(this);
            }
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
