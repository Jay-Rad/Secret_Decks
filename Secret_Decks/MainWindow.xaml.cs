﻿using Secret_Decks.Models;
using Secret_Decks.Windows;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

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
            WPF_Auto_Update.Updater.RemoteFileURI = "https://translucency.azurewebsites.net/Downloads/Secret%20Decks.exe";
            WPF_Auto_Update.Updater.ServiceURI = "https://translucency.azurewebsites.net/Services/VersionCheck.cshtml?Path=/Downloads/Secret%20Decks.exe";
            WPF_Auto_Update.Updater.UpdateTimeout = Duration.Forever;
            WPF_Auto_Update.Updater.CheckCommandLineArgs();
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var appData = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Secret_Decks\");
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var ex = e.Exception;
            while (ex != null)
            {
                var entry = new
                {
                    Type = "Error",
                    Timestamp = DateTime.Now,
                    Message = ex.Message,
                    Stack = ex.StackTrace,
                    Source = ex.Source
                };
                File.AppendAllText(appData.FullName + @"\Log.txt", serializer.Serialize(entry) + Environment.NewLine);
                ex = ex.InnerException;
            }
            
            MessageBox.Show("An error has occurred and has been written to the log in %appdata%\\Secret_Decks\\Log.txt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // *** Window Events *** //
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboCharacters.Items.Refresh();
            comboCharacters.SelectedItem = Settings.Current?.SelectedCharacter;
            comboCharacters.Text = Settings.Current?.SelectedCharacter?.Name;
            WPF_Auto_Update.Updater.CheckForUpdates(true);
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            var da = new DoubleAnimation(1, TimeSpan.FromSeconds(.5));
            borderMain.BeginAnimation(Border.OpacityProperty, da, HandoffBehavior.SnapshotAndReplace);

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            var da = new DoubleAnimation(.5, TimeSpan.FromSeconds(.5));
            borderMain.BeginAnimation(Border.OpacityProperty, da);
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        // *** Title Bar Buttons *** //
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            var win = new Minimized();
            win.Owner = this;
            win.ShowDialog();
        }
        private void buttonOptions_Click(object sender, RoutedEventArgs e)
        {
            buttonOptions.ContextMenu.IsOpen = !buttonOptions.ContextMenu.IsOpen;
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            var win = new About();
            win.Owner = this;
            win.ShowDialog();
        }

        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            var win = new Help();
            win.Owner = this;
            win.ShowDialog();
        }

        private void menuCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (menuCharacter.IsChecked)
            {
                gridCharacter.Visibility = Visibility.Visible;
            }
            else
            {
                gridCharacter.Visibility = Visibility.Collapsed;
            }
        }
        private void menuSlashOptions_Click(object sender, RoutedEventArgs e)
        {
            var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
            if (proc == null)
            {
                MessageBox.Show("Secret Worlds Legends doesn't appear to be running.", "SWL Not Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            var win=  new SlashCommands();
            win.Owner = this;
            win.ShowDialog();
        }

        // *** Character Events *** //
        private async void buttonAddCharacter_Click(object sender, RoutedEventArgs e)
        {
            var win = new NewCharacter();
            win.Owner = this;
            win.ShowDialog();
            comboCharacters.Items.Refresh();
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


        // *** Deck Events *** //

        private void buttonAddDeck_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.Current.SelectedCharacter == null)
            {
                MessageBox.Show("You must create or select a character first.", "Character Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var win = new NewDeck();
            win.Owner = this;
            win.ShowDialog();
            listDecks.Items.Refresh();
        }

        private async void buttonDeleteDeck_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this deck?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
                if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
                {
                    System.Windows.Forms.SendKeys.SendWait("{DIVIDE}gearmanager delete Secret_Decks_" + listDecks.SelectedItem.ToString());
                    await Task.Delay(10);
                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                }
                Settings.Current.SelectedCharacter.Decks.Remove(listDecks.SelectedItem as Deck);
                listDecks.Items.Refresh();
                Settings.Current.Save();
            }

        }

        private async void buttonUpdateDeck_Click(object sender, RoutedEventArgs e)
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
                System.Windows.Forms.SendKeys.SendWait("{DIVIDE}gearmanager save Secret_Decks_" + listDecks.SelectedItem.ToString());
                await Task.Delay(10);
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            }
            Settings.Current.Save();
        }

        private async void listDecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                System.Windows.Forms.SendKeys.SendWait("{DIVIDE}gearmanager use Secret_Decks_" + listDecks.SelectedItem.ToString());
                await Task.Delay(10);
                System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            }
            Settings.Current.Save();
        }

        
    }
}
