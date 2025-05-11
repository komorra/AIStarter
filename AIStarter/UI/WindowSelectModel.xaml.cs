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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace AIStarter.UI
{
    /// <summary>
    /// Logika interakcji dla klasy WindowSelectModel.xaml
    /// </summary>
    public partial class WindowSelectModel : Window
    {
        private class ModelItem
        {
            public string ModelName { get; set; }
            public string ModelFile { get; internal set; }
        }

        public string? SelectedModelFileName => ((ModelListView.SelectedItem as ListViewItem)?.Content as ModelItem)?.ModelFile;


        public WindowSelectModel()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            var modelFiles = Directory.GetFiles("Models", "*.txt", SearchOption.AllDirectories);
            
            foreach (var modelFileName in modelFiles)
            {                
                ModelListView.Items.Add(new ListViewItem 
                { 
                    Content = new ModelItem 
                    { 
                        ModelName = Path.GetFileNameWithoutExtension(modelFileName),
                        ModelFile = modelFileName,
                    } 
                });
            }

            base.OnInitialized(e);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Set the dialog result to true when the button is clicked
        }
    }
}
