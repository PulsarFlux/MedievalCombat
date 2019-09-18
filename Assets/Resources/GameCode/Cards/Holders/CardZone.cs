using System;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class CardZone
    {

        // Represents a collection of cards that also make up part of the board
        public CardList List;
        public CardZoneType Type;
        public CardZone(int PlayerIndex, int Range, ZoneType Type)
        {
            List = new CardList();
            this.Type = new CardZoneType(Type, (Range)Range, PlayerIndex);
        }
    }
}
