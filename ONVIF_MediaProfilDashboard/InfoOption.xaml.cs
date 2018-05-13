using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ONVIF_MediaProfilDashboard.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ONVIF_MediaProfilDashboard
{

    /// <summary>
    /// Interaction logic for InfoOption.xaml
    /// </summary>
    public partial class InfoOption : Window
    {
        int selectedIndex = 0;
        JArray optionsArray;

        public InfoOption(string options)
        {
            InitializeComponent();
            options_txt.IsReadOnly = true;
            optionsArray = JArray.Parse(options);
            options_txt.Text = optionsArray[selectedIndex].ToString();
            LoadOptionsName();
        }

        private void LoadOptionsName()
        {
            list_options_name.Items.Clear();
            if (optionsArray != null)
                foreach (var option in optionsArray)
                {
                    list_options_name.Items.Add(option["Name"]);
                }
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void list_options_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = list_options_name.SelectedIndex;
            if (list_options_name.SelectedIndex >= 0)
            {
                options_txt.Text = optionsArray[selectedIndex].ToString();
            }
        }
    }
}
