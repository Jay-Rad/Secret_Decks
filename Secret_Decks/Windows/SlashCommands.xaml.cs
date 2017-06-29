using Secret_Decks.Controls;
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
using System.Windows.Shapes;
using System.Xml;

namespace Secret_Decks.Windows
{
    /// <summary>
    /// Interaction logic for SlashCommands.xaml
    /// </summary>
    public partial class SlashCommands : Window
    {
        public SlashCommands()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var prefsDir = Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Funcom", "SWL", "Prefs"));
            var prefs = prefsDir.GetFiles("Prefs_2.xml", SearchOption.AllDirectories);
            var currentPref = prefs.Aggregate((fi1, fi2) => {
                if (fi1.LastWriteTime > fi2.LastWriteTime)
                {
                    return fi1;
                }
                else if (fi2.LastWriteTime > fi1.LastWriteTime)
                {
                    return fi2;
                }
                else
                {
                    return null;
                }
            });
            if (currentPref == null)
            {
                MessageBox.Show("The currently loaded SWL character couldn't be determined.", "Unknown Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var reader = XmlReader.Create(currentPref.FullName, new XmlReaderSettings() { Async = true, IgnoreWhitespace = true });
            while (reader.GetAttribute("name") != "ability_bar")
            {
                await reader.ReadAsync();
            }
            string name;
            string val;
            while (reader.HasAttributes)
            {
                name = reader.GetAttribute("name");
                val = reader.GetAttribute("value");
                if (val == "true" || val == "false")
                {
                    var item = new ListItemToggle();
                    item.textItemName.Text = name;
                    if (val == "true")
                    {
                        item.toggleSwitch.IsOn = true;
                    }
                    else if (val == "false")
                    {
                        item.toggleSwitch.IsOn = false;
                    }
                    item.toggleSwitch.Switched += async (send, arg) =>
                    {
                        var newValue = item.toggleSwitch.IsOn.ToString().ToLower();
                        var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
                        if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
                        {
                            System.Windows.Forms.SendKeys.SendWait("{DIVIDE}option " + item.textItemName.Text + " " + newValue);
                            await Task.Delay(10);
                            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        }
                    };
                    stackCommands.Children.Add(item);
                }
                else
                {
                    var item = new ListItemValue();
                    item.textItemName.Text = name;
                    item.textValue.Text = val;
                    item.textValue.LostFocus += async (send, arg) =>
                    {
                        var newValue = item.textValue.Text;
                        var proc = System.Diagnostics.Process.GetProcesses().ToList().Find(process => process.MainWindowTitle == "Secret World Legends");
                        if (Win32.User32.SetForegroundWindow(proc.MainWindowHandle))
                        {
                            System.Windows.Forms.SendKeys.SendWait("{DIVIDE}option " + item.textItemName.Text + " " + newValue);
                            await Task.Delay(10);
                            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                        }
                    };
                    stackCommands.Children.Add(item);
                    
                }
                await reader.ReadAsync();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
