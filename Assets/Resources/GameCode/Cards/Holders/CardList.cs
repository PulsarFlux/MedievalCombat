using System;
using System.Collections.Generic;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards
{   
    [Serializable()]
    // a general list for entities (not cards), used for decks, graveyards, hands etc
    public class CardList
    {
        public List<Entity> Cards;
        public CardList()
        {
            Cards = new List<Entity>();
        }

        public void AddCard(Entity C)
        {
            Cards.Add(C);
        }
        public void RemoveCard(Entity C)
        {
            Cards.Remove(C);
        }
        public void RemoveCard(Effects.Effect E)
        {
            Entity Temp = null;
            foreach (Entity C in Cards)
            {
                if (((Effect_Entity)C).GetEffect() == E)
                {
                    Temp = C;
                }
            }
            if (Temp != null)
            {
                Cards.Remove(Temp);
            }
        }
    }
}
