using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Xml;

namespace GettingOverItCheats
{
    /// <summary>
    /// Experimental
    /// </summary>
    public partial class WindowEditSave : Window
    {
        public string saveLocation;
        private XmlDocument xmlDoc = new XmlDocument();

        public WindowEditSave()
        {
            InitializeComponent();
        }

        private void ButtonModifySave_Click(object sender, RoutedEventArgs e)
        {
            XmlNode root = xmlDoc.DocumentElement;
            root.SelectSingleNode("timePlayed").InnerText = TextBoxTimePlayed.Text;
            root.SelectSingleNode("version").InnerText = TextBoxGameVersion.Text;
            root.SelectSingleNode("speedrun").InnerText = TextBoxSpeedRun.Text;
            xmlDoc.Save(saveLocation);

            DialogResult = true;
        }

        private void TitleBarButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void WindowEditSave_OnLoaded(object sender, RoutedEventArgs e)
        {
            xmlDoc.Load(saveLocation);
            XmlNode root = xmlDoc.DocumentElement;
            TextBoxTimePlayed.Text = root.SelectSingleNode("timePlayed").InnerText;
            TextBoxGameVersion.Text = root.SelectSingleNode("version").InnerText;
            TextBoxSpeedRun.Text = root.SelectSingleNode("speedrun").InnerText;
        }
    }
}
