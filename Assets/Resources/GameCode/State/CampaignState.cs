using System;

namespace Assets.GameCode.State
{
    public class CampaignState
    {
        public Cards.Loading.DeckSpec mCurrentDeck;
        public Cards.Loading.DeckSpec mOpposingDeck;

        public CampaignState()
        {
            mCurrentDeck = null;
            mOpposingDeck = null;
        }
    }
}

