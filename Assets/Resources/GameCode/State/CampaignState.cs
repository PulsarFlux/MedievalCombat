using System;

namespace Assets.GameCode.State
{
    [Serializable()]
    public class CampaignState
    {
        public Cards.CardCollection mCurrentCollection;
        public Cards.Loading.DeckSpec mOpposingDeck;

        public CampaignState()
        {
            mCurrentCollection = new Assets.GameCode.Cards.CardCollection();
        }
    }
}

