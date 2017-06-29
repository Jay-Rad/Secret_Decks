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
    /// Interaction logic for Minimized.xaml
    /// </summary>
    public partial class Minimized : Window
    {
        public Minimized()
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Current.HotSpot);
            while (this.Left < screen.Bounds.Right - 50 || this.Top < screen.Bounds.Bottom - 50)
            {
                if (this.Left < screen.Bounds.Right - 50)
                {
                    this.Left = Math.Min(screen.Bounds.Right - 50, this.Left + 30);
                }
                if (this.Top < screen.Bounds.Bottom - 50)
                {
                    this.Top = Math.Min(screen.Bounds.Bottom - 50, this.Top + 30);
                }
                await Task.Delay(1);
            }
        }
        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Visibility = Visibility.Visible;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
