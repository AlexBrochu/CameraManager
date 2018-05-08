using Newtonsoft.Json;
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

namespace ONVIF_MediaProfilDashboard
{

    /// <summary>
    /// Interaction logic for ConfigDashboard.xaml
    /// </summary>
    public partial class ConfigDashboard : Window
    {
        VideoParam vp = null;

        public ConfigDashboard()
        {
            InitializeComponent();
        }

        public ConfigDashboard(VideoParam video) : this()
        {
            this.vp = video;
            quality.Text = vp.Quality;
        }

    }
}
