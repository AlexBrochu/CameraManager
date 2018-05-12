using Newtonsoft.Json;
using ONVIF_MediaProfilDashboard.Media;
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
        public Media2Client media;
        MediaProfile[] profiles;
        string profileToken;
        

        public new bool DialogResult { get; private set; }

        // All config Screen
        ConfigVideoEncoder cve;
        ConfigVideoSource cvs;

        List<Button> allButtons = new List<Button>();

        public ConfigDashboard()
        {
            InitializeComponent();

            media_profile_rect.Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);

            allButtons.Add(video_source_btn);
            allButtons.Add(video_encode_btn);
            allButtons.Add(audio_src_btn);
            allButtons.Add(audio_encode_btn);
            allButtons.Add(ptz_btn);
            allButtons.Add(metadata_btn);
            disableAllBtn();

            // Video is the first step of config
            video_source_btn.IsEnabled = true;
            video_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
        }

        private void disableAllBtn()
        {
            foreach (Button btn in allButtons)
            {
                btn.IsEnabled = false;
            }
        }

        private void video_encode_btn_Click(object sender, RoutedEventArgs e)
        {
            cve = new ConfigVideoEncoder(media, this.profiles[0].token);
            cve.ShowDialog();
            bool res = cve.DialogResult;

            // Done
            if (res)
            {
                this.media = cve.media;

                video_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                // Next btn to active
                audio_src_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
                audio_src_btn.IsEnabled = true;
            }
        }

        private void video_source_btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.profileToken != null)
            {
                cvs = new ConfigVideoSource();
                cvs.profileToken = this.profileToken;
            }
            else
            {
                cvs = new ConfigVideoSource();
            }

            cvs.setMedia(media);
            cvs.ShowDialog();
            bool res = cvs.DialogResult;

            // Done
            if (res)
            {
                this.media = cvs.media;
                this.profileToken = this.cvs.profileToken;
                profiles = media.GetProfiles(this.profileToken, new string[]{ "All" });

                video_source_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                // Next btn to active
                video_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
                video_encode_btn.IsEnabled = true;
            }
        }

        private void audio_encode_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void audio_src_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ptz_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void metadata_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        internal void setMedia(Media2Client media)
        {
            this.media = media;
            profiles = media.GetProfiles(null, null);
        }
    }
}
