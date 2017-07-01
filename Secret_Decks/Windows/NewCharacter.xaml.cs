using Secret_Decks.Models;
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

namespace Secret_Decks.Windows
{
    /// <summary>
    /// Interaction logic for NewCharacter.xaml
    /// </summary>
    public partial class NewCharacter : Window
    {
        public NewCharacter()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(textCharacterName.Text))
            {
                System.Windows.MessageBox.Show("A character name is required.", "Name Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Settings.Current?.SelectedCharacter?.Decks?.Exists(deck => deck?.Name == textCharacterName?.Text) == true)
            {
                System.Windows.MessageBox.Show("A character with that name already exists.", "Already Exists", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            Settings.Current.Characters.Add(new Character() { Name = textCharacterName.Text });
            Settings.Current.Save();
            this.Close();
        }
    }
}
