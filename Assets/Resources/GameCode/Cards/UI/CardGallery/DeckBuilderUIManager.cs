using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Assets.GameCode.Cards.UI
{
    public class DeckBuilderUIManager : CardsUIManager, ICardPlacedHandler
    {
        public GameObject UnitCardPrefab, ExpandedUnitCardPrefab, CardPrefab, ExpandedCardPrefab;
        public GameObject ExpandedEffectCardPrefab;
        public GameObject EffectCardPrefab;

        public GameObject mLibraryTopUnitPanel;
        public GameObject mLibraryBottomUnitPanel;
        public GameObject mLibraryEffectPanel;
        public GameObject mDeckTopUnitPanel;
        public GameObject mDeckBottomUnitPanel;
        public GameObject mDeckEffectPanel;

        public GameObject mErrorMessageTextBox;

        private CardList mLibraryCards;
        private CardList mDeckCards;

        private CardCollection mCardCollection;

        private List<UICard> mUICards;
        private State.GameScene mLastScene;

        private bool mDeckIsValid;
        private string mCurrentErrorMessage;

        private static uint kMaxDeckSize = 30;
        private static uint kMinDeckSize = 20;
        private static uint kMinDeckUnits = 10;

        private static string kDeckTooLargeString = "Your deck is too large, maximum: ";
        private static string kDeckTooSmallString = "Your deck is too small, minimum: ";
        private static string kNotEnoughUnitsString = "Your deck does not have enough units, minimum: ";

        public DeckBuilderUIManager()
        {
            mUICards = new List<UICard>();
            mLastScene = State.GameScene.None;
        }

        public void Start()
        {
            State.DeckBuilderSetupState setupState =
                (State.DeckBuilderSetupState)State.StateHolder.StateManager.GetAndClearPassedState();
            if (setupState != null)
            {
                mCardCollection = setupState.mCardCollection;
                if (setupState.mCardCollection.mLibrary != null)
                {
                    mLibraryCards = Cards.Loading.CardLoading.ProduceDeck(State.StateHolder.StateManager.CardPool,
                        setupState.mCardCollection.mLibrary);
                }
                else
                {
                    mLibraryCards = new CardList();
                }

                if (setupState.mCardCollection.mDeck != null)
                {
                    mDeckCards = Cards.Loading.CardLoading.ProduceDeck(State.StateHolder.StateManager.CardPool,
                        setupState.mCardCollection.mDeck);
                }
                else
                {
                    mDeckCards = new CardList();
                }
                mLastScene = setupState.mLastScene;
            }

            VerifyDeck();
            UpdateUI();
        }

        public override void UpdateUI()
        {
            if (mDeckIsValid)
            {
                mErrorMessageTextBox.GetComponentInChildren<UnityEngine.UI.Text>().text = "";
            }
            else
            {
                mErrorMessageTextBox.GetComponentInChildren<UnityEngine.UI.Text>().text = mCurrentErrorMessage;
            }
            UpdateCards();
        }

        private void RemoveFromList(Entities.Entity entity)
        {
            mLibraryCards.RemoveCard(entity);
            mDeckCards.RemoveCard(entity);
        }

        public void CardPlaced(UICard PlacedCard, CardZoneType PlacedZone)
        {
            if (PlacedCard.GetEntity().IsUnit() == (PlacedZone.getType() == ZoneType.Field))
            {
                RemoveFromList(PlacedCard.GetEntity());
                // Hack? Use owner index to specify whether this
                // is a deck zone or a library zone.
                // 0 = Deck
                // 1 = Library
                if (PlacedZone.getOwnerIndex() == 0)
                {
                    mDeckCards.AddCard(PlacedCard.GetEntity());
                }
                else
                {
                    mLibraryCards.AddCard(PlacedCard.GetEntity());
                }

                if (VerifyDeck())
                {
                    UpdateCardCollection();
                }
            }
        }

        private void UpdateCards()
        {         
            foreach (UICard C in mUICards)
            {
                C.UnityCard.transform.SetParent(null, false);
            }
            int libraryUnitsPlaced = 0;
            foreach (Entities.Entity E in mLibraryCards.Cards)
            {
                Transform transform = null;
                if (E.IsUnit())
                {
                    if (libraryUnitsPlaced > 8)
                    {
                        transform = mLibraryBottomUnitPanel.transform;
                    }
                    else
                    {
                        transform = mLibraryTopUnitPanel.transform;
                    }
                    libraryUnitsPlaced++;
                }
                else
                {
                    transform = mLibraryEffectPanel.transform;
                }

                UpdateCard<UICard, UnitDisplayCard, UnitExpandingCard,
                DisplayCard, ExpandingCard, EffectDisplayCard, ExpandingCard>(E, 
                    transform,
                    mUICards,
                    CardPrefab,
                    ExpandedCardPrefab,
                    UnitCardPrefab,
                    ExpandedUnitCardPrefab,
                    EffectCardPrefab,
                    ExpandedEffectCardPrefab);
            }

            int deckUnitsPlaced = 0;
            foreach (Entities.Entity E in mDeckCards.Cards)
            {
                Transform transform = null;
                if (E.IsUnit())
                {
                    if (deckUnitsPlaced > 8)
                    {
                        transform = mDeckBottomUnitPanel.transform;
                    }
                    else
                    {
                        transform = mDeckTopUnitPanel.transform;
                    }
                    deckUnitsPlaced++;
                }
                else
                {
                    transform = mDeckEffectPanel.transform;
                }

                UpdateCard<UICard, UnitDisplayCard, UnitExpandingCard,
                DisplayCard, ExpandingCard, DisplayCard, ExpandingCard>(E, 
                    transform,
                    mUICards,
                    CardPrefab,
                    ExpandedCardPrefab,
                    UnitCardPrefab,
                    ExpandedUnitCardPrefab,
                    EffectCardPrefab,
                    ExpandedEffectCardPrefab);
            }
        }

        private bool VerifyDeck()
        {
            int cardsInDeck = mDeckCards.Cards.Count;
            int unitCardsInDeck = 0;
            foreach (Entities.Entity E in mDeckCards.Cards)
            {
                if (E.IsUnit())
                {
                    unitCardsInDeck++;
                }
            }

            mDeckIsValid = true;
            if (cardsInDeck < kMinDeckSize)
            {
                mDeckIsValid = false;
                mCurrentErrorMessage = kDeckTooSmallString + kMinDeckSize.ToString();
            }
            else if (cardsInDeck > kMaxDeckSize)
            {
                mDeckIsValid = false;
                mCurrentErrorMessage = kDeckTooLargeString + kMaxDeckSize.ToString();
            }
            else if (unitCardsInDeck < kMinDeckUnits)
            {
                mDeckIsValid = false;
                mCurrentErrorMessage = kNotEnoughUnitsString + kMinDeckUnits.ToString();
            }
            return mDeckIsValid;
        }

        private void UpdateCardCollection()
        {
            mCardCollection.mDeck.Cards = new Dictionary<string, int>();
            foreach (Entities.Entity E in mDeckCards.Cards)
            {
                mCardCollection.mDeck.IncrementEntry(E.Name, 1);
            }
            mCardCollection.mLibrary.Cards = new Dictionary<string, int>();
            foreach (Entities.Entity E in mLibraryCards.Cards)
            {
                mCardCollection.mLibrary.IncrementEntry(E.Name, 1);
            }
        }

        public void BackButtonPressed()
        {
            if (mLastScene == State.GameScene.CardGame)
            {
                State.StateHolder.StateManager.SetPassedState(new State.CardsSetupState(false, false));
            }
            else if (mLastScene == State.GameScene.CampaignCardGame)
            {
                State.StateHolder.StateManager.SetPassedState(new State.CardsSetupState(true, false));
            }
            State.StateHolder.StateManager.MoveToNextScene(mLastScene);
        }


    }
}

