using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Loading;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class CardGameState
    {
        public Effects.EffectHolder SharedEffects;
        public Player[] Players = new Player[2];
        public CardPool mCardPool;
        public DeckSpec mDefaultDeckSpec;
        public TurnInfo mTurnInfo;

        private System.Random mRandom;

        const int kNumStartingCards = 10;

        public CardGameState()
        {
            mRandom = new System.Random();
            mCardPool = State.StateHolder.StateManager.CardPool;
            mDefaultDeckSpec = State.StateHolder.StateManager.DefaultDeckSpec;
            mTurnInfo = new TurnInfo();
        }
        /*public CardGameState(CardGameState toCopy)
        {
            SharedEffects = new Effects.EffectHolder(toCopy.SharedEffects, this);
            for (int i = 0; i < Players.Length; i++)
            {
         //       Players[i] = new Player(toCopy.Players[i]);
            }
         //   mRandom = new System.Random(toCopy.mRandom);
            mCardPool = State.StateHolder.StateManager.CardPool;
            mDefaultDeckSpec = State.StateHolder.StateManager.DefaultDeckSpec;
          //  mTurnInfo = new TurnInfo(toCopy.mTurnInfo);
        }*/

        public void Init()
        {
            SharedEffects = new Effects.EffectHolder(this, true, 0);
            Players[0] = new Player(0, this);
            for (int i = 1; i < Players.Length; i++)
            {
                Players[i] = new AIPlayer(i, this);
            }
        }

        public void GeneratePlayerDecks(DeckSpec playerOneDeck, DeckSpec playerTwoSpec)
        {
            DeckSpec deckSpec = playerOneDeck == null ? mDefaultDeckSpec : playerOneDeck;
            Players[0].AddDeck(CardLoading.ProduceDeck(mCardPool, deckSpec));
            deckSpec = playerTwoSpec == null ? mDefaultDeckSpec : playerTwoSpec;
            Players[1].AddDeck(CardLoading.ProduceDeck(mCardPool, deckSpec));
        }

        public Entities.Entity MulliganCard(Entities.Entity card)
        {
            return Players[card.getOwnerIndex()].MulliganCard(mRandom, card);
        }

        public CardList getCardList(CardZoneType CZ)
        {
            if (CZ.getType() == ZoneType.Hand)
            {
                return Players[CZ.getOwnerIndex()].mHand;
            }
            else if (CZ.getType() == ZoneType.Field)
            {
                return Players[CZ.getOwnerIndex()].mBoard.RangeZones[(int)CZ.getRange()].List;
            }
            else
            {
                return null;
            }
        }
        public void CardPlaced(CardZoneType CZ, Entities.Entity Card)
        {
            // The player will remove the card from its current
            // card list and Card.Placed will add it to the new one.
            Players[Card.Owner.getIndex()].CardPlaced(Card);
            Card.Placed(CZ, getCardList(CZ), this);
        }
        public void Update(TurnInfo TI)
        {
            SharedEffects.UpdatePersistance();
            foreach (Player P in Players)
            {
                P.Update(this);
            }
            SharedEffects.Update();
        }
        public void NewTurn(TurnInfo TI, CardGameManager manager)
        {
            Players[TI.GetCPI()].NewTurn(this);
            SharedEffects.NewTurn();
            Players[TI.GetCPI()].TakeTurn(TI, this, manager);
        }

        public void NewRound()
        {
            SharedEffects.Clear();
            foreach (Player P in Players)
            {
                P.NewRound();
            }
        }

        public void NewMatch()
        {
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].PickHand(mRandom, kNumStartingCards);
            }    
        }
    }
}
