using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.GameCode.Cards.Loading
{
    /*[Serializable()]
    public class CardEntry
    {
        public string mName;
        public int mNumber;
        public CardEntry(string Name, string Number)
        {
            mName = Name;
            int.TryParse(Number, out mNumber);
        }
        public CardEntry(string Name, int Number)
        {
            mName = Name;
            mNumber = Number;
        }
    }*/
    [Serializable()]
    public class DeckSpec
    {
        public Dictionary<string, int> Cards = new Dictionary<string, int>();
        public void SetEntry(string name, int number)
        {
            Cards[name] = number;
        }
        public void IncrementEntry(string name, int difference)
        {
            if (Cards.ContainsKey(name))
            {
                Cards[name] = Cards[name] + difference;
            }
            else
            {
                Cards[name] = difference;
            }
        }
    }
}
