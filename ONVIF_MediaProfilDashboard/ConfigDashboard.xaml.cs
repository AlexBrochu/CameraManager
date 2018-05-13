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
        bool isCreateMode = true;

        public new bool DialogResult { get; private set; }

        // All config Screen
        ConfigVideoEncoder cve;
        ConfigVideoSource cvs;
        ConfigAudioSource cas;
        ConfigAudioEncoder cae;

        List<Button> allButtons = new List<Button>();
        List<bool> btnState = new List<bool>();
        string profileName;

        public ConfigDashboard(bool isCreateMode)
        {
            InitializeComponent();

            this.isCreateMode = isCreateMode;

            media_profile_rect.Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);

            save_btn.IsEnabled = false;

            // Fill list of btn
            allButtons.Add(video_source_btn);
            allButtons.Add(video_encode_btn);
            allButtons.Add(audio_src_btn);
            allButtons.Add(audio_encode_btn);
            allButtons.Add(ptz_btn);
            allButtons.Add(metadata_btn);
            // Disable all btn
            setBtnStateList();
            disableAllBtn();

            // Video is the first step of config
            video_source_btn.IsEnabled = true;
            video_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
            saveBtnState();
            if (profile_name.Text == "")
            {
                video_source_btn.IsEnabled = false;
            }
        }

        // Initialize state btn to false
        private void setBtnStateList()
        {
            foreach (Button btn in allButtons)
            {
                btnState.Add(false);
            }
        }

        private void disableAllBtn()
        {
            // Save state before disabling every btn
            saveBtnState();
            foreach (Button btn in allButtons)
            {
                btn.IsEnabled = false;
            }
        }

        private void saveBtnState()
        {
            int i = 0;
            foreach (Button btn in allButtons)
            {
                btnState[i] = btn.IsEnabled;
                i++;
            }
        }

        private void video_encode_btn_Click(object sender, RoutedEventArgs e)
        {
            cve = new ConfigVideoEncoder(media, this.profiles[0].token, profileName);
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

                save_btn.IsEnabled = true;
            }
        }

        private void video_source_btn_Click(object sender, RoutedEventArgs e)
        {
            if (this.profileToken != null)
            {
                cvs = new ConfigVideoSource(profileName);
                cvs.profileToken = this.profileToken;
            }
            else
            {
                cvs = new ConfigVideoSource(profileName);
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
            cae = new ConfigAudioEncoder(media, this.profiles[0].token, profileName);
            cae.ShowDialog();
            bool res = cae.DialogResult;

            // Done
            if (res)
            {
                this.media = cae.media;

                audio_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                // Next btn to active
                //ptz_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
                //ptz_btn.IsEnabled = true;
            }
        }

        private void audio_src_btn_Click(object sender, RoutedEventArgs e)
        {
            cas = new ConfigAudioSource(media, this.profiles[0].token, profileName);
            cas.ShowDialog();
            bool res = cas.DialogResult;

            // Done
            if (res)
            {
                this.media = cas.media;

                audio_src_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.LawnGreen);
                // Next btn to active
                audio_encode_btn.Background = new SolidColorBrush(System.Windows.Media.Colors.CadetBlue);
                audio_encode_btn.IsEnabled = true;
            }
        }

        private void ptz_btn_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void metadata_btn_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            // Delete token if mode is Create and token is defined
            if (profileToken != null && isCreateMode)
            {
                media.DeleteProfile(profileToken);
            }
            this.Close();
        }

        internal void setMedia(Media2Client media)
        {
            this.media = media;
            profiles = media.GetProfiles(null, null);
        }

        private void profile_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (profile_name.Text == "")
            {
                // Profile has to have a name all the time
                disableAllBtn();
            }
            else
            {
                this.profileName = profile_name.Text;
                reactiveBtn();
            }
        }

        private void reactiveBtn()
        {
            int i = 0;
            foreach (bool state in btnState)
            {
                allButtons[i].IsEnabled = state;
                i++;
            }
        }
    }
}
