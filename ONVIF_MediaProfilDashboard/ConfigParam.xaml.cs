using Newtonsoft.Json;
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
    /// Interaction logic for ConfigDashboard.xaml
    /// </summary>
    public partial class ConfigParam : Window
    {
        public VideoParam vp = new VideoParam();
        public new bool DialogResult { get; private set; }

        public ConfigParam()
        {
            InitializeComponent();
            quality.KeyDown += ValidNumberOnly;
            width.KeyDown += ValidNumberOnly;
            height.KeyDown += ValidNumberOnly;
        }

        private void ValidNumberOnly(object sender, KeyEventArgs e)
        {
            int isNumber = 0;
            string key = e.Key.ToString();
            char k = key[key.Length - 1];
            e.Handled = !int.TryParse(k.ToString(), out isNumber);
        }

        public void MapValue(VideoParam vp)
        {
            quality.Text = vp.Quality.ToString();
            width.Text = vp.Width.ToString();
            height.Text = vp.Height.ToString();
        }

        public ConfigParam(VideoParam video) : this()
        {
            this.vp = video;
            quality.Text = vp.Quality.ToString();
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void quality_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (quality.Text.Trim() != "")
            {
                this.vp.Quality = int.Parse(quality.Text);
            }
         
        }

        private void width_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (width.Text.Trim() != "")
            {
                this.vp.Width = int.Parse(width.Text);
            }
        }

        private void height_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (height.Text.Trim() != "")
            {
                this.vp.Height = int.Parse(height.Text);
            }
        }
    }
}
