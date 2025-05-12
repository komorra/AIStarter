using AIStarter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
    /// Logika interakcji dla klasy ControlInputSlot.xaml
    /// </summary>
    public partial class ControlInputSlot : UserControl, ISlot
    {
        private string value = string.Empty;

        public string InputName { get; set; } = string.Empty;
        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                Visualisation.Child = null;
                TryVisualise(value);
                this.value = value;
                if (UrlTextBox.Text != value)
                {
                    UrlTextBox.Text = value;
                }
            }
        }        

        private async Task TryVisualise(string resourcePath)
        {            
            if (File.Exists(resourcePath))
            {
                if(resourcePath.EndsWith(".png") || resourcePath.EndsWith(".jpg") || resourcePath.EndsWith(".jpeg"))
                {
                    var image = new BitmapImage(new Uri(resourcePath));
                    var imageControl = new Image
                    {
                        Source = image,
                        Stretch = Stretch.Uniform,
                        Width = 200,
                        Height = 200
                    };
                    imageControl.Source = image;
                    Visualisation.Child = imageControl;
                }                
            }
            else if (resourcePath.StartsWith("http"))
            {
                var content = await new HttpClient().GetAsync(resourcePath);

                if (content.StatusCode == HttpStatusCode.OK)
                {
                    var image = new BitmapImage(new Uri(resourcePath));
                    var imageControl = new Image
                    {
                        Source = image,
                        Stretch = Stretch.Uniform,
                        Width = 200,
                        Height = 200
                    };
                    imageControl.Source = image;
                    Visualisation.Child = imageControl;
                }                
            }
        }

        public string ValueJSONPath { get; set; } = string.Empty;

        public bool ShowInputBar { get; set; } = true;

        public ControlInputSlot()
        {
            InitializeComponent();
            Loaded += ControlInputSlot_Loaded;
        }

        private void ControlInputSlot_Loaded(object sender, RoutedEventArgs e)
        {
            InputBar.Visibility = ShowInputBar ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                Value = filePath;                
                UrlTextBox.Text = filePath;

                e.Handled = true;
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = UrlTextBox.Text;
        }
    }
}
