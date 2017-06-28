using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secret_Decks.Models
{
    public class Deck
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
