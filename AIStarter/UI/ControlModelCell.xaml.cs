using AIStarter.Core;
using AIStarter.Interfaces;
using AIStarter.Utils;
using Newtonsoft.Json.Linq;
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

        private string modelData = string.Empty;

        public ControlModelCell()
        {
            InitializeComponent();
            Loaded += ControlModelCell_Loaded;
        }

        private void ControlModelCell_Loaded(object sender, RoutedEventArgs e)
        {
            modelData = File.ReadAllText(ModelFile);
            ModelInputJSON = ModelToUIParser.Populate(Input, modelData);
        }

        private void Discard_Click(object sender, RoutedEventArgs e)
        {
            if(Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(this);
            }
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            Run.IsEnabled = false;
            if (Input.Content is StackPanel panel)
            {
                var jsonInput = JObject.Parse(ModelInputJSON ?? string.Empty);  

                foreach (ISlot item in panel.Children)
                {
                    var token = jsonInput.SelectToken(item.ValueJSONPath ?? string.Empty);
                    token.Replace(item.Value ?? string.Empty);
                }

                var modelDataLines = modelData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var dockerCommand = modelDataLines.FirstOrDefault(line => line.StartsWith("docker run", StringComparison.OrdinalIgnoreCase))?.Trim() ?? string.Empty;
                dockerCommand = dockerCommand.Replace("docker run", "", StringComparison.OrdinalIgnoreCase);
                var prediction = modelDataLines.LastOrDefault()?.Trim() ?? string.Empty;

                var result = await Inference.Run(dockerCommand, jsonInput.ToString(), prediction);
                var outputFile = Inference.OutputDataToTempFile(result);
            }
            Run.IsEnabled = true;
        }
    }
}
