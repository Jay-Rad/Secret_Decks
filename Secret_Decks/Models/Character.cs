using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secret_Decks.Models
{
    public class Character
    {
        public string Name { get; set; }
        public List<Deck> Decks { get; set; } = new List<Deck>();
        public override string ToString()
        {
            return Name;
        }
    }
}
