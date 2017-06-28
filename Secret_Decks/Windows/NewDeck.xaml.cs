using Secret_Decks.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Secret_Decks.Windows
{
    /// <summary>
    /// Interaction logic for NewDeck.xaml
    /// </summary>
    public partial class NewDeck : Window
    {
        public NewDeck()
        {
            InitializeComponent();
        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textNewDeck.Text))
            {
                System.Windows.MessageBox.Show("A deck name is required.", "Name Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Settings.Current.SelectedCharacter.Decks.Exists(deck=>deck.Name == textNewDeck.Text))
            {
                System.Windows.MessageBox.Show("A deck with that name already exists.", "Already Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var proc = Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
            if (proc == null)
            {
                System.Windows.MessageBox.Show("Secret Worlds Legends doesn't appear to be running.", "SWL Not Running", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
            {
                SendKeys.SendWait("{DIVIDE}gearmanager save Secret_Decks_" + textNewDeck.Text + "{ENTER}");
            }
            Settings.Current.SelectedCharacter.Decks.Add(new Deck() { Name = textNewDeck.Text });
            Settings.Current.Save();
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
