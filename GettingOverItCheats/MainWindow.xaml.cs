using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using GettingOverItCheats.core;
using GettingOverItCheats.core.hacks;
using GettingOverItCheats.core.hacks.amountofwins;
using GettingOverItCheats.core.patches.gravityhack;
using Microsoft.Win32;

namespace GettingOverItCheats
{
    public partial class MainWindow : Window
    {
        private string targetAssembly;
        private string targetExe;
        private int newNumberOfWins = 0;
        private int saveCounter = 1;
        private AmountOfWins amountOfWins;
        private RegistryEditor registryEditor;

        public MainWindow()
        {
            InitializeComponent();
            amountOfWins = new AmountOfWins();
            registryEditor = new RegistryEditor(Constants.gameRegistryPath);

            // form settings
            newNumberOfWins = amountOfWins.Get();
            TextBoxNumberOfWins.Text = newNumberOfWins.ToString();
            ComboBoxGravity.IsEnabled = false;
            ButtonApplyGravityHack.IsEnabled = false;
            UpdateListBoxSavesList();
        }

        #region Layout

        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TitleBarButtonMinimize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TitleBarButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Program
        private void ButtonBrowseToGameLocation_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executables|*.exe";
            ofd.Title = "Select the getting over it executable";
            ofd.RestoreDirectory = true;
            
            if (ofd.ShowDialog() == true)
            {
                TextBoxGameLocation.Text = ofd.FileName;
                targetExe = ofd.FileName;
                targetAssembly = targetExe.ToLower().Replace("gettingoverit.exe", "GettingOverIt_Data\\Managed\\Assembly-CSharp.dll");
            }

            ComboBoxGravity.IsEnabled = true;
            ButtonApplyGravityHack.IsEnabled = true;
        }

        private void ButtonChangeNumberOfWins_Click(object sender, RoutedEventArgs e)
        {
            if (amountOfWins.Set(newNumberOfWins))
            {
                if (MessageBox.Show(
                        "Number of wins updated! (Re)start getting over it for the changes to take effect?",
                        "Success", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    RestartGame();
                }
            }
            else
            {
                MessageBox.Show("Can't update number of wins!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBoxGravity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = (ComboBoxGravity.SelectedItem as ComboBoxItem).Content.ToString(); 

            switch (selectedItem.ToLower())
            {
                case "earth":
                    LabelGravityDescription.Content = "Restore gravity to default";
                    break;
                case "moon":
                    LabelGravityDescription.Content = "Float longer in the sky";
                    break;
                case "reverse":
                    LabelGravityDescription.Content = "Beat the game in less than 5 seconds!";
                    break;
            }
        }

        private void ButtonApplyGravityHack_Click(object sender, RoutedEventArgs e)
        {
            targetAssembly = Path.GetDirectoryName(TextBoxGameLocation.Text) + @"\GettingOverIt_Data\Managed\Assembly-CSharp.dll";

            GravityHack gravityHack = new GravityHack(targetAssembly);

            string selectedGravityHack = (ComboBoxGravity.SelectedItem as ComboBoxItem).Content.ToString();
            switch (selectedGravityHack.ToLower())
            {
                case "moon":
                    gravityHack.Apply(GravityTypes.Moon);
                    break;
                case "reverse":
                    gravityHack.Apply(GravityTypes.Reverse);
                    break;
                default:
                    gravityHack.Apply(GravityTypes.Earth);
                    break;
            }

            if (MessageBox.Show("The game has to be restarted for the changes to take effect. Restart now?",
                    "Game restart required", MessageBoxButton.YesNo, MessageBoxImage.Information,
                    MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                RestartGame();
            }
        }

        private void ButtonNewSave_Click(object sender, RoutedEventArgs e)
        {
            string saveName = TextBoxSaveName.Text;
            File.WriteAllBytes($"saves/{saveName}.xml", registryEditor.ReadRegistryKey<byte[]>("SaveGame1_h1867918427"));
            File.WriteAllBytes($"saves/{saveName}.xml", registryEditor.ReadRegistryKey<byte[]>("SaveGame0_h1867918426"));
            ListBoxSavesList.Items.Add(saveName);

            saveCounter++;
            TextBoxSaveName.Text = "Save_" + saveCounter;
        }

        private void ButtonRemoveSave_Click(object sender, RoutedEventArgs e)
        {
            if (ListBoxSavesList.SelectedIndex != -1)
            {
                string selectedItem = ListBoxSavesList.SelectedItem.ToString();
                string saveLocation = $"saves\\{selectedItem}.xml";
                if (File.Exists(saveLocation))
                {
                    File.Delete(saveLocation);
                }
                ListBoxSavesList.Items.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Please select a save to delete first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        private void ButtonLoadSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (ListBoxSavesList.SelectedIndex != -1)
            {
                string selectedSave = ListBoxSavesList.SelectedItem.ToString();
                if (File.Exists($"saves\\{selectedSave}.xml"))
                {
                    registryEditor.ChangeRegistryKey("SaveGame1_h1867918427", File.ReadAllBytes($"saves/{selectedSave}.xml"), RegistryValueKind.Binary);
                    registryEditor.ChangeRegistryKey("SaveGame0_h1867918426", File.ReadAllBytes($"saves/{selectedSave}.xml"), RegistryValueKind.Binary);
                }
                else
                {
                    MessageBox.Show("This save doesn't exist anymore. Please reload the application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a save first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //TODO: do something with this or nah?
        private void ButtonEditSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (ListBoxSavesList.SelectedIndex != -1)
            {
                string selectedSave = ListBoxSavesList.SelectedItem.ToString();
                string saveLocation = $"saves\\{selectedSave}.xml";
                if (File.Exists(saveLocation))
                {
                    WindowEditSave editSaveWindow = new WindowEditSave();
                    editSaveWindow.saveLocation = saveLocation;
                    editSaveWindow.Title = $"Edit {selectedSave}";
                    editSaveWindow.ShowDialog();
                    /*bool? dialogResult = editSaveWindow.ShowDialog();
                    if (dialogResult == true)
                    {
                        
                    }*/
                }
            }
            else
            {
                MessageBox.Show("Please select a save first!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ValueUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (newNumberOfWins <= 99999999)
            {
                newNumberOfWins++;
                TextBoxNumberOfWins.Text = newNumberOfWins.ToString();
            }
            else
            {
                MessageBox.Show("Value is way too high for 'number of wins' !", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }
        private void ValueDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (newNumberOfWins > 0)
            {
                newNumberOfWins--;
                TextBoxNumberOfWins.Text = newNumberOfWins.ToString();
            }
            else
            {
                MessageBox.Show("No negative value allowed for 'number of wins' !", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void TextBoxNumberOfWins_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (!int.TryParse(TextBoxNumberOfWins.Text, out result) || result > 99999999 || result < 0)
            {
                TextBoxNumberOfWins.Text = newNumberOfWins.ToString();
            }
            else
            {
                newNumberOfWins = Convert.ToInt32(TextBoxNumberOfWins.Text);
            }
        }

        #endregion

        #region functions

        private void RestartGame()
        {
            foreach (Process process in Process.GetProcessesByName("GettingOverIt"))
            {
                process.Kill();
            }
            Process.Start(targetExe);
        }

        private void UpdateListBoxSavesList()
        {
            if (!Directory.Exists("saves")) { Directory.CreateDirectory("saves"); }

            foreach (string file in Directory.GetFiles("saves", "*.xml", SearchOption.AllDirectories))
            {
                ListBoxSavesList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                saveCounter++;
            }
            TextBoxSaveName.AppendText("_" + saveCounter);
        }
        #endregion
    }
}
