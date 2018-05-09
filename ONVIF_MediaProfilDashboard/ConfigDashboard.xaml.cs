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
    public partial class ConfigDashboard : Window
    {
        public VideoParam vp = new VideoParam();
        public new bool DialogResult { get; private set; }

        public ConfigDashboard()
        {
            InitializeComponent();
            quality.Text = vp.Quality;
            width.Text = vp.Width;
            Height.Text = vp.Height;
        }

        public ConfigDashboard(VideoParam video) : this()
        {
            this.vp = video;
            quality.Text = vp.Quality;
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void quality_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (quality.Text != "")
            {
                this.vp.Quality = quality.Text;
            }
         
        }
    }
}
