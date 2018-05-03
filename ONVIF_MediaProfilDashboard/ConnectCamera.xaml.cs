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
    public class ConfigData
    {
        private string v_encode = "test";

        public string VEncode { get => v_encode; set => v_encode = value; }
    }

    /// <summary>
    /// Interaction logic for ConnectCamera.xaml
    /// </summary>
    public partial class ConnectCamera : Window
    {
        Media2Client media;
        UriBuilder deviceUri;
        MediaProfile[] profiles;
        ConfigDashboard cd = new ConfigDashboard();
        ConfigData cdata = new ConfigData();
        String[] prms = { };

        public ConnectCamera()
        {
            InitializeComponent();
            this.DataContext = cdata;
            Console.WriteLine(cdata.VEncode);
            Console.WriteLine(this.DataContext.ToString());

            address.GotFocus += InitTextbox;
            user.GotFocus += InitTextbox;
            password.GotFocus += InitTextbox;
            button.Click += OnConnect;
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {

            /*if (cd != null && cd.IsActive)
            {
                return;
            }
            cd.DataContext = this.DataContext;
            cd.Show();*/

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

                    // Take first token 
                    VideoEncoder2Configuration[] videoEncode = media.GetVideoEncoderConfigurations(null, profiles[0].token);
                    AudioEncoder2Configuration[] audioEncode = media.GetAudioEncoderConfigurations(null, profiles[0].token);

                    VideoEncoder2ConfigurationOptions[]  options = media.GetVideoEncoderConfigurationOptions(null, profiles[0].token);

                    videoEncode[0].Quality = 1;
                    videoEncode[0].Resolution.Width = options[0].ResolutionsAvailable[14].Width;
                    videoEncode[0].Resolution.Height = options[0].ResolutionsAvailable[14].Height;
                    media.SetVideoEncoderConfiguration(videoEncode[0]);
                    
                    //profiles[0].Configurations.VideoEncoder = videoEncode[0];
                    //profiles[0].Configurations.VideoEncoder.Quality = 1;
                    //profiles[0].Configurations.VideoEncoder.Resolution.Height= 50;
                    //profiles[0].Configurations.VideoEncoder.Resolution.Width = 50;
                    //profiles[0].Configurations.VideoEncoder.RateControl.FrameRateLimit = 1;

                    //ConfigurationRef config = profiles[0].Configurations;

                    ConfigurationRef[] config = { new ConfigurationRef() };
                    config[0].Token = videoEncode[0].token;

                    media.AddConfiguration(profiles[0].token, "profile_1_h264", null);


                    profiles = media.GetProfiles(null, null);

                    // Make sure that the list is empty before adding new items
                    listBox.Items.Clear();
                    listBox.Items.Add(profiles[0].Name);
                    /*if (profiles != null)
                        foreach (MediaProfile p in profiles)
                        {
                            if (p.Configurations!=null)
                            {
                                
                                Console.WriteLine("Video Source :" + p.Configurations.VideoSource.ToString());
                                Console.WriteLine("Video Encoder :" + p.Configurations.VideoEncoder.ToString());
                                Console.WriteLine("Audio Source :" + p.Configurations.AudioSource.ToString());
                                Console.WriteLine("Audio Encoder :" + p.Configurations.AudioEncoder.ToString());
                            }
                            
                            listBox.Items.Add(p.Name);
                        }*/
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
            if (inError)
            {
                textBox.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void InitTextbox(object sender, RoutedEventArgs e)
        {
            if (((sender as Control).Foreground as SolidColorBrush)?.Color == Colors.DarkGray)
            {
                if (sender is TextBox)
                {
                    (sender as TextBox).Text = "";
                }
                else if (sender is PasswordBox)
                {
                    (sender as PasswordBox).Password = "";
                }
                (sender as Control).Foreground = new SolidColorBrush(Colors.Black);
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
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"d:\Programs File\VLC\"));
            else
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"D:\temp\src\onvifex\Vlc.DotNet-develop\lib\x64"));
        }


        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (profiles != null && listBox.SelectedIndex >= 0)
            {
                StreamVideoOnVLC(prms);
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
    }
}
