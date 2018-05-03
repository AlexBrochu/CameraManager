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

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class VideoEncoderConf
    {
        public string text { get; set; }
        public string value { get; set; }
    }

    public class RootObject
    {
        public List<VideoEncoderConf> VideoEncoderConfs { get; set; }
    }

    /// <summary>
    /// Interaction logic for ConfigDashboard.xaml
    /// </summary>
    public partial class ConfigDashboard : Window
    {
        private string projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        public ConfigDashboard()
        {
            InitializeComponent();
            //LoadComboItems(this.VideoEncoderConf_CBox, "\\assets\\properties\\videoEncoderConfiguration.json");
        }

        private void LoadComboItems(ComboBox cb, string file)
        {
            Console.WriteLine(projectFolder);
            var str = File.ReadAllText(projectFolder + file);
            var x = JsonConvert.DeserializeObject<RootObject>(str);
            foreach (var config in x.VideoEncoderConfs)
            {
                ComboboxItem item = new ComboboxItem();
                item.Text = config.text;
                item.Value= config.value;

                cb.Items.Add(item);
            }

        }

        private void VideoEncoderConf_CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string value = VideoEncoderConf_CBox.SelectedItem.ToString();
            int index = VideoEncoderConf_CBox.SelectedIndex;

            /*MessageBox.Show("Selected Item Text: " + value + "\n" +
                    "Index: " + index);*/
        }
    }
}
