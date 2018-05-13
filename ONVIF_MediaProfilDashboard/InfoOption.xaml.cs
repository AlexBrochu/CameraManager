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
        JArray optionsArray;
        JObject options;

        public InfoOption(string options)
        {
            InitializeComponent();
            options_txt.IsReadOnly = true;

            // Converting string to json object
            try
            {
                this.options = JObject.Parse(options);
            }
            // if fails try json array
            catch (Exception ex)
            {
                this.options = null;
                this.optionsArray = JArray.Parse(options);
            }
                        
            options_txt.Text = options;
            LoadOptionsName();
        }

        private void LoadOptionsName()
        {
            list_options_name.Items.Clear();
            if (options != null)
            {
                list_options_name.Items.Add("option 1");
            }

            if (optionsArray != null)
            {
                int index = 1;
                foreach (var option in optionsArray)
                {
                    list_options_name.Items.Add("option " + index);
                    index++;
                }
            }
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void list_options_name_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = list_options_name.SelectedIndex;
            if (list_options_name.SelectedIndex >= 0 && optionsArray != null)
            {
                options_txt.Text = optionsArray[selectedIndex].ToString();
            }
        }
    }
}
