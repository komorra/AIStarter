using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy ControlInputSlot.xaml
    /// </summary>
    public partial class ControlInputSlot : UserControl
    {
        public string InputName { get; set; } = string.Empty;
        public string InputValue { get; set; } = string.Empty;
        public string ValueJSONPath { get; set; } = string.Empty;

        public ControlInputSlot()
        {
            InitializeComponent();
        }
    }
}
