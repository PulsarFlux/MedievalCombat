using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.GameCode.Cards.Entities;

namespace Assets.GameCode.Cards
{
    [Serializable()]
    public class Player
    {
        const int MaxCP = 10;

        private int mPlayerIndex;
        private int mRoundVictories;
        private int mCommandPoints;

        private bool mSpentCP;
        private bool mPassed;
        private int mUnitCardsPlaced;
        private int mOtherCardsPlaced;

        private int mMulligansUsed;
        private int mMaxMulligans;

        public CardList mDeck = new CardList();
        public CardList mHand = new CardList();
        public CardList mGraveyard = new CardList();
        public CardList mMulliganedCards = new CardList();
        public Effects.EffectHolder mEffects;
        public PlayerBoard mBoard;

        public Player(int index, CardGameState GS)
        {
            mPlayerIndex = index;
            mCommandPoints = MaxCP;
            mBoard = new PlayerBoard(index);
            mEffects = new Effects.EffectHolder(GS, false, mPlayerIndex);
            mMaxMulligans = 5;
            mMulligansUsed = 0;
            mUnitCardsPlaced = 0;
            mOtherCardsPlaced = 0;
    }

        public void AddDeck(CardList CL)
        {
            mDeck = CL;
            foreach (Entity E in mDeck.Cards)
            {
                E.Owner = this;
            }
        }

        public CardList PickHand(Random inR, int num_cards)
        {
            int final_cards = mDeck.Cards.Count - num_cards;
            while (mDeck.Cards.Count > final_cards && mDeck.Cards.Count > 0)
            {
                int index = inR.Next(0, mDeck.Cards.Count);
                Entity temp = mDeck.Cards[index];
                mDeck.RemoveCard(temp);
                mHand.AddCard(temp);
            }
            return mHand;
        }

        public Entities.Entity MulliganCard(System.Random inR, Entity mulliganCard)
        {
            Entities.Entity newCard = null;
            if (mDeck.Cards.Any())
            {
                mHand.RemoveCard(mulliganCard);
                mMulliganedCards.AddCard(mulliganCard);
                newCard = mDeck.Cards[inR.Next(0, mDeck.Cards.Count)];
                mDeck.RemoveCard(newCard);
                mHand.AddCard(newCard);
                mMulligansUsed++;
            }
            return newCard;
        }
        public void FinishMulligan()
        {
            foreach (Entity E in mMulliganedCards.Cards)
            {
                mDeck.AddCard(E);
            }
            mMulliganedCards.Cards.Clear();
            mMulligansUsed = 0;
        }
        public void CardPlaced(Entities.Entity C)
        {
            if (C.IsUnit())
            {
                mUnitCardsPlaced += 1;
            }
            else
            {
                mOtherCardsPlaced += 1;
            }
            RemoveFromList(C);
        }
        public void RemoveFromList(Entity C)
        {
            mDeck.RemoveCard(C);
            mHand.RemoveCard(C);
            mGraveyard.RemoveCard(C);
            for (int i = 0; i < mBoard.RangeZones.Length; i++)
            {
                mBoard.RangeZones[i].List.RemoveCard(C);
            }
        }

        public int getIndex()
        {
            return mPlayerIndex;
        }
            
        public void NewRound()
        {
            mEffects.Clear();
            foreach (CardZone CZ in mBoard.RangeZones)
            {
                List<Entity> Temp = new List<Entity>(CZ.List.Cards);
                foreach (Entity E in Temp)
                {
                    UnitDied((Unit)E);
                }
            }
            mPassed = false;
            mCommandPoints = MaxCP;
        }
        public bool CanSpendCP(int cost)
        {
            if (mCommandPoints - cost >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether player has 'cost' command points to spend, spends them if true
        /// </summary>
        public bool SpendCP(int Cost)
        {
            if (mCommandPoints - Cost >= 0)
            {
                mCommandPoints -= Cost;
                mSpentCP = true;
                return true;
            }
            else
            {
                return false;
            }
        }  
        public bool HasSpentCP()
        {
            return mSpentCP;
        }
        public int GetCP()
        {
            return mCommandPoints;
        }

        public int GetVP()
        {
            int VP = 0;
            foreach (CardZone CZ in mBoard.RangeZones)
            {
                foreach (Entity E in CZ.List.Cards)
                {
                    VP += ((Unit)E).GetVP();
                }
            }
            return VP;
        }
        public bool WonRound(int RoundMax)
        {
            mRoundVictories += 1;
            if (mRoundVictories == RoundMax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int RoundsWon()
        {
            return mRoundVictories;
        }

        public void Pass()
        {
            mPassed = true;
        }
        public bool HasPassed()
        {
            return mPassed;
        }
        public bool WasUnitPlaced()
        {
            if (mUnitCardsPlaced > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool WasCardPlaced()
        {
            if (mUnitCardsPlaced + mOtherCardsPlaced > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void GetMulliganInfo(out int usedMulligans, out int maxMulligans)
        {
            usedMulligans = mMulligansUsed;
            maxMulligans = mMaxMulligans;
        }

        public void UnitDied(Unit Unit)
        {
            RemoveFromList(Unit);
            mGraveyard.AddCard(Unit);
        }
        public void CheckBlocks(Unit Attacker, Unit Target, Modules.Target.TargettingData TD)
        {
            foreach (CardZone CZ in mBoard.RangeZones)
            {
                foreach (Entity E in CZ.List.Cards)
                {
                    if (E.IsUnit()) { ((Unit)E).CheckBlockStatus(Attacker, Target, TD); }
                }
            }
            
        }
        public void NewTurn(CardGameState GS)
        {
            mEffects.NewTurn();
            mSpentCP = false;
            mUnitCardsPlaced = 0;
            mOtherCardsPlaced = 0;
            foreach (CardZone CZ in mBoard.RangeZones)
            {
                foreach (Entity E in CZ.List.Cards)
                {
                    E.NewTurn();
                }
            }
        }
        public void Update(CardGameState GS)
        {
            mEffects.UpdatePersistance();
            for (int i = 0; i < mBoard.RangeZones.Length; i++)
            {
                foreach (Entity E in mBoard.RangeZones[i].List.Cards)
                {
                    E.Update();
                }
            }
            mEffects.Update();
        }
        public virtual void TakeTurn(TurnInfo turnInfo, CardGameState gameState, CardGameManager manager)
        {
            
        }
    }
}
