using Newtonsoft.Json;
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
    /// Interaction logic for ConfigVideoEncoder.xaml
    /// </summary>
    public partial class ConfigVideoEncoder : Window
    {
        public Media2Client media;
        public string profileToken;
        public string configToken;
        string profileName;

        VideoEncoder2Configuration[] configs;
        int selectedIndex;
        InfoOption io;

        public new bool DialogResult { get; private set; }

        public ConfigVideoEncoder(Media2Client media, string token, string profileName)
        {
            InitializeComponent();
            this.media = media;
            this.profileToken = token;
            this.profileName = profileName;

            this.Closing += Window_Closing;

            configs = media.GetVideoEncoderConfigurations(null, profileToken);
            setComboxItem();
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            VideoEncoder2Configuration vec = JsonConvert.DeserializeObject<VideoEncoder2Configuration>(info_config.Text);
            media.SetVideoEncoderConfiguration(vec);

            ConfigurationRef[] config = { new ConfigurationRef() };
            config[0].Type = "VideoEncoder";
            config[0].Token = configs[selectedIndex].token;
            media.AddConfiguration(profileToken, profileName, config);

            this.DialogResult = true;
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void setComboxItem()
        {
            List<string> data = new List<string>();
            foreach (VideoEncoder2Configuration element in configs)
            {
                data.Add(element.Name);
            }
            configs_cbox.ItemsSource = data;
            configs_cbox.SelectedItem = configs_cbox.Items.GetItemAt(0);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = configs_cbox.SelectedIndex;
            info_config.Text = JsonConvert.SerializeObject(configs[selectedIndex], Formatting.Indented);
        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            string configsStr = JsonConvert.SerializeObject(configs, Formatting.Indented);
            io = new InfoOption(configsStr);
            io.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (io != null)
            {
                io.Close();
            }
        }
    }
}
