using AIStarter.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AIStarter.Utils
{
    internal static class ModelToUIParser
    {
        public static void Populate(ContentControl slot, string modelData)
        {
            var match = Regex.Match(modelData, @"\$'(.*?)'", RegexOptions.Singleline);

            if (match.Success)
            {
                var slotPanel = new StackPanel();
                slotPanel.Orientation = Orientation.Vertical;

                string json = match.Groups[1].Value;
                
                var jsonData = JsonConvert.DeserializeObject<dynamic>(json);
                var input = jsonData.input;
                foreach(var item in input)
                {
                    var name = item.Name;
                    var value = item.Value;

                    var inputSlot = new ControlInputSlot();
                    inputSlot.InputName = name.ToString();
                    inputSlot.InputValue = value.ToString();
                    inputSlot.DataContext = inputSlot;

                    slotPanel.Children.Add(inputSlot);
                }

                slot.Content = slotPanel;
            }            
        }
    }
}
