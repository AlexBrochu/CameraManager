using Newtonsoft.Json;
using ONVIF_MediaProfilDashboard.Device;
using ONVIF_MediaProfilDashboard.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ONVIF_MediaProfilDashboard
{

    /// <summary>
    /// Interaction logic for ConnectCamera.xaml
    /// </summary>
    public partial class ConnectCamera : Window
    {

        // TO CHANGE In the properties resources of the project
        // Onvifex path
        string vlcPath = System.IO.Path.Combine(System.IO.Path.GetFullPath(@"..\..\"), "Vlc");
        string onvifexPath = Properties.Resources.ONVIFEXPath;

        int previousSavedConnIndex = -1;
        List<CameraConnexion> cameras = new List<CameraConnexion>();
        CameraConnexion selectedCam = null;
        String path_to_connexion_file = AppDomain.CurrentDomain.BaseDirectory + @"\login_info.json";

        VideoParam vp = new VideoParam();

        Media2Client media;
        UriBuilder deviceUri;
        MediaProfile[] profiles;
        ConfigDashboard cd;
        String[] prms = { };

        public ConnectCamera()
        {
            InitializeComponent();

            delete_cam_btn.IsEnabled = false;
        
            // Manage profiles btn
            create_profile_btn.IsEnabled = false;
            modify_profile_btn.IsEnabled = false;
            delete_profile_btn.IsEnabled = false;

            listBox.SelectionMode = SelectionMode.Single;
            listBox_saved.SelectionMode = SelectionMode.Single;

            button.Click += OnConnect;
            save_btn.Click += OnSave;
            listBox_saved.SelectionChanged += OnSavedSelectionChanged;
            LoadConnexion();
        }

        private void OnSavedSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cameras != null && listBox_saved.SelectedIndex >= 0)
            {
                selectedCam = cameras.ElementAt(listBox_saved.SelectedIndex);
                address.Text = selectedCam.Address;
                password.Password = selectedCam.Password;
                user.Text = selectedCam.User;
                camera_name.Text = selectedCam.CameraName;
                // Can be Deleted
                delete_cam_btn.IsEnabled = true;

                if (previousSavedConnIndex == listBox_saved.SelectedIndex)
                {
                    ConnectCam();
                }
                previousSavedConnIndex = listBox_saved.SelectedIndex;
            }
            listBox_saved.SelectedIndex = -1;
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {

            if (camera_name.Text == "")
            {
                camera_name.Background = new SolidColorBrush(Colors.Red);
            }
            else if(address.Text == "")
            {
                address.Background = new SolidColorBrush(Colors.Red);
            }
            else if (user.Text == "")
            {
                user.Background = new SolidColorBrush(Colors.Red);
            }
            else if (password.Password == "")
            {
                password.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                CameraConnexion cc = new CameraConnexion();
                cc.Address = address.Text;
                cc.CameraName = camera_name.Text;
                cc.Password = password.Password;
                cc.User = user.Text;

                // write login info in json file
                if (cameras == null)
                {
                    cameras = new List<CameraConnexion>();
                }
                // override camera if is already saved with same name
                int index = cameras.FindIndex(element => element.CameraName.Equals(cc.CameraName));
                if (index == -1)
                {
                    cameras.Add(cc);
                }
                else
                {
                    cameras[index] = cc;
                }
                
                string json = JsonConvert.SerializeObject(cameras);
                if (!File.Exists(path_to_connexion_file))
                {
                    File.CreateText(path_to_connexion_file).Close();
                }
                using (StreamWriter file = new StreamWriter(path_to_connexion_file, false))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, cameras);
                }
                LoadConnexion();

            }
            
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            ConnectCam();
        }

        private void ConnectCam()
        {
            bool inError = false;
            deviceUri = new UriBuilder("http:/onvif/device_service");

            string[] addr = address.Text.Split(':');
            deviceUri.Host = addr[0];
            if (addr.Length == 2)
            {
                deviceUri.Port = Convert.ToInt16(addr[1]);
            }

            System.ServiceModel.Channels.Binding binding;
            HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
            httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
            binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), httpTransport);

            try
            {
                DeviceClient device = new DeviceClient(binding, new EndpointAddress(deviceUri.ToString()));
                Service[] services = device.GetServices(false);
                Service xmedia2 = services.FirstOrDefault(s => s.Namespace == "http://www.onvif.org/ver20/media/wsdl");

                if (xmedia2 != null)
                {
                    media = new Media2Client(binding, new EndpointAddress(deviceUri.ToString()));
                    media.ClientCredentials.HttpDigest.ClientCredential.UserName = user.Text;
                    media.ClientCredentials.HttpDigest.ClientCredential.Password = password.Password;
                    media.ClientCredentials.HttpDigest.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                    profiles = media.GetProfiles(null, null);

                    // Make sure that the list is empty before adding new items
                    listBox.Items.Clear();
                    if (profiles != null)
                        foreach (MediaProfile p in profiles)
                        {                            
                            listBox.Items.Add(p.Name);
                        }

                    // Enable Manage Profile btn
                    create_profile_btn.IsEnabled = true;
                }
                listBox.SelectionChanged += OnSelectionChanged;
                video.MediaPlayer.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
                video.MediaPlayer.Log += MediaPlayer_Log;
                video.MediaPlayer.EndInit();
            }
            catch (Exception ex)
            {
                textBox.Text = ex.Message;
                inError = true;

            }
            changeErrorLogColor(inError);
        }

        private void changeErrorLogColor(bool inError)
        {
            if (inError)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Red);
                textBox.FontSize = 16;
            }
            else
            {
                textBox.Text = "";
                textBox.FontSize = 12;
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void LoadConnexion()
        {
            listBox_saved.Items.Clear();
            if (!File.Exists(path_to_connexion_file))
            {
                File.CreateText(path_to_connexion_file).Close();
            }
            using (StreamReader file = File.OpenText(path_to_connexion_file))
            {
                JsonSerializer serializer = new JsonSerializer();
                cameras = (List<CameraConnexion>)serializer.Deserialize(file, typeof(List<CameraConnexion>));
                if (cameras != null)
                {
                    foreach (CameraConnexion cam in cameras)
                    {
                        listBox_saved.Items.Add(cam.CameraName);
                    }
                }
            }
        }

        private void MediaPlayer_Log(object sender, Vlc.DotNet.Core.VlcMediaPlayerLogEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("libVlc : {0} {1} @ {2}", e.Level, e.Message, e.Module));
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            if (IntPtr.Size == 4)
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, vlcPath));
            else
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, onvifexPath));
        }


        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (profiles != null && listBox.SelectedIndex >= 0)
            {
                bool inError = false;
                try
                {
                    StreamVideoOnVLC(prms);
                }
                catch (Exception ex)
                {
                    textBox.Text = ex.Message;
                    inError = true;
                }
                changeErrorLogColor(inError);


                // Enable Modify and delete profiles
                //modify_profile_btn.IsEnabled = true;
                delete_profile_btn.IsEnabled = true;
            }
            else
            {
                // Disable Modify and delete profiles
                modify_profile_btn.IsEnabled = false;
                delete_profile_btn.IsEnabled = false;
            }
        }

        System.ServiceModel.Channels.Binding WsdlBinding
        {
            get
            {
                HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
                httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
                return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), httpTransport);
            }
        }

        private void StreamVideoOnVLC(String[] recordParams)
        {
            UriBuilder uri = new UriBuilder(media.GetStreamUri("RtspOverHttp", profiles[listBox.SelectedIndex].token));
            uri.Host = deviceUri.Host;
            uri.Port = deviceUri.Port;
            uri.Scheme = "rtsp";

            textBox.Text = uri.Path;

            List<string> options = new List<string>();
            options.Add(":rtsp-http");
            options.Add(":rtsp-http-port=" + uri.Port);
            options.Add(":rtsp-user=" + user.Text);
            options.Add(":rtsp-pwd=" + password.Password);

            if (recordParams.Length != 0)
            {
                foreach (string param in recordParams)
                {
                    options.Add(param);
                }
            }
            
            video.MediaPlayer.Play(uri.Uri, options.ToArray());
        }

        private void delete_cam_btn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCam == null || cameras == null)
            {
                delete_cam_btn.IsEnabled = false;
                return;
            }
            // Remove Camera login info
            int index = cameras.FindIndex(element => element.CameraName.Equals(selectedCam.CameraName));
            if (index >= 0)
            {
                cameras.RemoveAt(index);
            }

            string json = JsonConvert.SerializeObject(cameras);
            if (!File.Exists(path_to_connexion_file))
            {
                File.CreateText(path_to_connexion_file).Close();
            }
            using (StreamWriter file = new StreamWriter(path_to_connexion_file, false))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, cameras);
            }
            LoadConnexion();
            delete_cam_btn.IsEnabled = false;
        }

        private void create_profile_btn_Click(object sender, RoutedEventArgs e)
        {
            bool createMode = true;
            cd = new ConfigDashboard(createMode);
            cd.setMedia(media);
            cd.ShowDialog();
            bool res = cd.DialogResult;
            if (res)
            {
                this.media = cd.media;
                profiles = media.GetProfiles(null, null);

                // Make sure that the list is empty before adding new items
                listBox.Items.Clear();
                if (profiles != null)
                    foreach (MediaProfile p in profiles)
                    {
                        listBox.Items.Add(p.Name);
                    }
            }
        }

        private void modify_profile_btn_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void delete_profile_btn_Click(object sender, RoutedEventArgs e)
        {
            // Remove selected profile
            if (listBox.SelectedIndex >= 0)
            {
                media.DeleteProfile(profiles[listBox.SelectedIndex].token);
                profiles = media.GetProfiles(null, null);
                refreshProfileList();
                textBox.Text = "";
            }
        }

        private void refreshProfileList()
        {
            listBox.Items.Clear();
            if (profiles != null)
                foreach (MediaProfile p in profiles)
                {
                    listBox.Items.Add(p.Name);
                }
        }
    }
}
