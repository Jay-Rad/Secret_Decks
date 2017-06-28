using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secret_Decks.Models
{
    public class Settings
    {
        public static Settings Current { get; set; } = new Settings();
        public List<Character> Characters { get; set; } = new List<Character>();
        public Character SelectedCharacter { get; set; }
        public void Save()
        {
            var appData = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Secret_Decks\");
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            File.WriteAllText(appData.FullName + "Settings.json", serializer.Serialize(Current));
        }
        public void Load()
        {
            var appData = Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Secret_Decks\");
            if (File.Exists(appData.FullName + "Settings.json"))
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                var strSettings = File.ReadAllText(appData.FullName + "Settings.json");
                var jsonSettings = serializer.Deserialize<Settings>(strSettings);
                foreach (var prop in typeof(Settings).GetProperties())
                {
                    prop.SetValue(Current, prop.GetValue(jsonSettings));
                }
            }
        }
    }
}
