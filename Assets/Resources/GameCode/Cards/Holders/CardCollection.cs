using System;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class CardCollection
    {
        public Cards.Loading.DeckSpec mDeck;
        public Cards.Loading.DeckSpec mLibrary;

        public CardCollection()
        {
            mDeck = new Loading.DeckSpec();
            mLibrary = new Loading.DeckSpec();
        }
    }
}

