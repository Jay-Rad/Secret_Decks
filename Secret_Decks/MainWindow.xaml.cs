using Secret_Decks.Models;
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

namespace Secret_Decks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = Settings.Current;
            Settings.Current.Load();
            Application.Current.Exit += (send, arg) =>
            {
                Settings.Current.Save();
            };
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            InitializeComponent();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var appData = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Secret_Decks\");
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            File.AppendAllText(appData.FullName + @"\Log.txt", serializer.Serialize(e.Exception) + Environment.NewLine);
            MessageBox.Show("An error has occurred and has been written to the log in %appdata%\\Secret_Decks\\Log.txt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboCharacters.Items.Refresh();
            comboCharacters.SelectedItem = Settings.Current.SelectedCharacter;
            comboCharacters.Text = Settings.Current.SelectedCharacter.Name;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private async void buttonAddCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(comboCharacters.Text))
            {
                MessageBox.Show("You must enter a character name above.", "Name Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Settings.Current.Characters.Exists(character=>character.Name == comboCharacters.Text))
            {
                MessageBox.Show("A character with that name already exists.", "Name Already Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newChar = new Models.Character() { Name = comboCharacters.Text };
            Settings.Current.Characters.Add(newChar);
            Settings.Current.SelectedCharacter = newChar;
            comboCharacters.Items.Refresh();
            Settings.Current.Save();
            var tooltip = new ToolTip() { Content = "Character created." };
            tooltip.PlacementTarget = comboCharacters;
            tooltip.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            tooltip.VerticalOffset = 20;
            tooltip.IsOpen = true;
            await Task.Delay(2000);
            tooltip.IsOpen = false;
        }

        private void buttonDeleteCharacter_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this character and its decks?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Settings.Current.Characters.Remove(Settings.Current.SelectedCharacter);
                comboCharacters.Items.Refresh();
                listDecks.Items.Refresh();
                Settings.Current.Save();
            }
        }

        private void comboCharacters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listDecks.Items.Refresh();
        }

        private void buttonAddDeck_Click(object sender, RoutedEventArgs e)
        {
            var win = new Windows.NewDeck();
            win.Owner = this;
            win.ShowDialog();
            listDecks.Items.Refresh();
        }

        private void buttonDeleteDeck_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this deck?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Settings.Current.SelectedCharacter.Decks.Remove(listDecks.SelectedItem as Deck);
                listDecks.Items.Refresh();
                Settings.Current.Save();
            }

        }

        private void buttonUpdateDeck_Click(object sender, RoutedEventArgs e)
        {
            if (listDecks.SelectedIndex == -1)
            {
                MessageBox.Show("You must select a deck to update.", "Deck Required", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
            if (proc == null)
            {
                MessageBox.Show("Secret Worlds Legends doesn't appear to be running.", "SWL Not Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
            {
                System.Windows.Forms.SendKeys.SendWait("~/gearmanager save SecretDecks" + listDecks.SelectedItem.ToString() + "~");
            }
            Settings.Current.Save();
        }

        private void listDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listDecks.SelectedItem == null)
            {
                return;
            }
            var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
            if (proc == null)
            {
                MessageBox.Show("Secret Worlds Legends doesn't appear to be running.", "SWL Not Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
            {
                System.Windows.Forms.SendKeys.SendWait("~/gearmanager use SecretDecks" + listDecks.SelectedItem.ToString() + "~");
            }
            Settings.Current.Save();
        }
    }
}
